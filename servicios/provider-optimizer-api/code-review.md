# Code Review: Análisis de código defectuoso

## Código bajo revisión

```csharp
[HttpPost("assign")]
public async Task<IActionResult> AssignProvider(Request request)
{
    var providers = _db.Providers.ToList();
    var selected = providers.FirstOrDefault();
    if(selected == null) return BadRequest("No providers");

    selected.IsBusy = true;
    _db.SaveChanges();

    return Ok(selected);
}
```

---

## Problemas identificados

### 1) No real async usage

El método se declara `async Task` pero **nunca usa `await`**. Todas las operaciones son síncronas:

- `_db.Providers.ToList()` bloquea el hilo mientras lee la base de datos.
- `_db.SaveChanges()` bloquea hasta persistir.

**Riesgo**: No hay beneficio de escalabilidad; bajo carga se agotan hilos del pool y se degrada el rendimiento.

---

### 2) No transaction control

No hay transacción que agrupe “leer proveedores”, “elegir uno” y “marcar como ocupado + guardar”. Si `SaveChanges()` falla a mitad de proceso o hay excepciones, el estado puede quedar inconsistente (por ejemplo, proveedor marcado en memoria pero no persistido).

**Riesgo**: Pérdida de consistencia y comportamientos impredecibles en fallos.

---

### 3) Concurrency issue

Dos peticiones pueden ejecutar el flujo a la vez:

1. Ambas leen la misma lista de proveedores.
2. Ambas eligen al mismo (por ejemplo, el primero).
3. Ambas ponen `IsBusy = true` y llaman a `SaveChanges()`.

Resultado: un mismo proveedor queda “asignado” a dos clientes. No hay bloqueo ni operación atómica “reservar proveedor”.

**Riesgo**: Condiciones de carrera, doble asignación y datos incorrectos en producción.

---

### 4) Incorrect provider selection logic

`providers.FirstOrDefault()` devuelve **el primero que devuelve la base de datos**, sin orden definido. No se aplica ningún criterio de negocio (distancia al cliente, valoración, tipo de servicio, carga de trabajo).

**Riesgo**: Asignaciones arbitrarias, mala experiencia de usuario y uso ineficiente de recursos.

---

### 5) No validation

- No se comprueba si `request` es null.
- No se validan propiedades del request (coordenadas, tipo de servicio, etc.).
- No se verifica que el proveedor elegido siga disponible (`IsBusy == false`) antes de actualizarlo.

**Riesgo**: NullReferenceException, datos inválidos y respuestas 500 en lugar de 400 con mensajes claros.

---

### 6) Violates SOLID

- **S**: El controlador hace acceso a datos, lógica de selección, cambio de estado y respuesta HTTP (más de una responsabilidad).
- **O/C**: Cualquier cambio de política de selección o de persistencia obliga a tocar este método.
- **D**: Dependencia directa de un `DbContext` concreto; no hay abstracciones para sustituir en tests o en otros entornos.

**Riesgo**: Código difícil de probar, mantener y extender.

---

### 7) No DTO usage

Se devuelve la **entidad** `selected` (modelo de EF) directamente. El contrato de la API queda acoplado al modelo de persistencia; cualquier cambio interno (navegaciones, campos) puede afectar al cliente.

**Riesgo**: Fuga de detalles de implementación, posibles referencias circulares y contrato inestable.

---

### 8) No provider selection algorithm

No existe un algoritmo de selección: ni por distancia (p. ej. Haversine), ni por puntuación (rating), ni por carga de trabajo. La “selección” es implícita y arbitraria.

**Riesgo**: No se cumple la regla de negocio de “mejor proveedor” para el cliente.

---

## Resumen de riesgos

| Área           | Riesgo principal                                      |
|----------------|--------------------------------------------------------|
| Rendimiento    | Bloqueo de hilos por uso síncrono de I/O              |
| Consistencia   | Estado inconsistente por falta de transacciones       |
| Concurrencia   | Doble asignación del mismo proveedor                  |
| Negocio        | Selección arbitraria, sin criterios definidos         |
| Robustez       | Fallos por falta de validación y manejo de null        |
| Mantenibilidad | Violación de SOLID y acoplamiento alto                |
| API            | Contrato inestable por exponer entidades              |

---

## Implementación corregida

Se propone una solución que usa **async EF**, **transacciones**, **algoritmo de puntuación (scoring)** para proveedores, **DTOs** y **validaciones** adecuadas.

### 1. DTOs

```csharp
// DTOs/AssignProviderRequest.cs
public record AssignProviderRequest
{
    public double ClientLat { get; init; }
    public double ClientLng { get; init; }
    public int ServiceTypeId { get; init; }
}

// DTOs/AssignProviderResponse.cs
public record AssignProviderResponse
{
    public string ProviderId { get; init; }
    public string ProviderName { get; init; }
    public double EstimatedMinutes { get; init; }
}
```

### 2. Validaciones

En el controlador (o con FluentValidation):

- Comprobar `request != null`.
- Validar rangos de `ClientLat`, `ClientLng` y que `ServiceTypeId` exista en catálogo.
- Devolver 400 con mensaje claro cuando falle la validación.

### 3. Servicio de asignación con algoritmo de scoring

El servicio aplica un **provider scoring algorithm**: puntúa proveedores disponibles (por ejemplo por distancia y rating), ordena por puntuación y elige el mejor. La reserva del proveedor se hace de forma atómica en infraestructura.

```csharp
// Application/Services/IProviderAssignmentService.cs
public interface IProviderAssignmentService
{
    Task<AssignProviderResult> AssignBestProviderAsync(AssignProviderRequest request, CancellationToken ct = default);
}

public record AssignProviderResult(bool Success, AssignProviderResponse? Response, string? ErrorCode);

// Application: lógica de scoring (ejemplo)
// Score = f(distancia, calificación) — por ejemplo menor distancia mejor; a igual distancia mayor rating
// Se delega en un IProviderRepository que devuelve disponibles y en un IProviderScoringService que puntúa y ordena
```

### 4. Repositorio con transacción y reserva atómica (async EF)

La capa de infraestructura usa **async** y **transacciones** para “reservar” un proveedor de forma atómica y evitar concurrencia:

```csharp
// Infrastructure/Repositories/ProviderRepository.cs
public async Task<Provider?> ReserveBestProviderAsync(double clientLat, double clientLng, CancellationToken ct = default)
{
    await using var transaction = await _db.Database.BeginTransactionAsync(ct);
    try
    {
        // 1. Consulta async: solo disponibles, ordenados por scoring (distancia + rating)
        var candidates = await _db.Providers
            .Where(p => !p.IsBusy)
            .ToListAsync(ct);

        var best = ApplyScoringAlgorithm(candidates, clientLat, clientLng); // distancia + rating
        if (best == null) return null;

        // 2. Bloquear y actualizar de forma atómica (evitar doble asignación)
        best.IsBusy = true;
        await _db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return best;
    }
    catch
    {
        await transaction.RollbackAsync(ct);
        throw;
    }
}
```

Alternativa más segura en alta concurrencia: usar una única sentencia SQL de actualización que seleccione y marque (p. ej. `UPDATE ... SET IsBusy = true WHERE Id = (SELECT Id FROM ... WHERE NOT IsBusy ORDER BY ... LIMIT 1)`), para que la “reserva” sea atómica a nivel de base de datos.

### 5. Consultas EF async

En todo el flujo:

- `ToListAsync(ct)` en lugar de `ToList()`.
- `SaveChangesAsync(ct)` en lugar de `SaveChanges()`.
- `BeginTransactionAsync` / `CommitAsync` / `RollbackAsync` con `CancellationToken` donde aplique.

### 6. Controlador fino (SOLID, DTOs, validación)

```csharp
[HttpPost("assign")]
public async Task<IActionResult> AssignProvider([FromBody] AssignProviderRequest? request, CancellationToken ct = default)
{
    // Validación
    if (request == null)
        return BadRequest("Request body is required.");
    if (!IsValidCoordinates(request.ClientLat, request.ClientLng))
        return BadRequest("Invalid coordinates.");
    if (!_catalogService.IsValidServiceType(request.ServiceTypeId))
        return BadRequest("Invalid service type.");

    var result = await _assignmentService.AssignBestProviderAsync(request, ct);

    if (!result.Success)
        return result.ErrorCode == "NO_PROVIDERS_AVAILABLE"
            ? NotFound("No providers available.")
            : BadRequest(result.ErrorCode);

    return Ok(result.Response); // DTO, no entidad
}
```

- **Transacciones**: gestionadas dentro del repositorio/servicio de aplicación.
- **Concurrencia**: resuelta con transacción + actualización atómica (o UPDATE atómico en BD).
- **Selección**: algoritmo de scoring (distancia + rating) en un servicio dedicado.
- **Validación**: request y reglas de negocio validadas antes de llamar al servicio.
- **SOLID**: controlador solo orquesta; lógica en servicios e interfaces.
- **DTOs**: entrada y salida son DTOs; no se exponen entidades.
- **Async**: todo el flujo usa `async`/`await` y consultas EF async.

---

## Checklist de la implementación corregida

| Requisito                      | Cumplido en la propuesta                                      |
|--------------------------------|----------------------------------------------------------------|
| Uso real de async              | `ToListAsync`, `SaveChangesAsync`, `BeginTransactionAsync`     |
| Control transaccional          | Transacción en repositorio (commit/rollback)                   |
| Evitar concurrencia            | Transacción + reserva atómica (o UPDATE atómico en BD)         |
| Lógica de selección correcta   | Algoritmo de scoring (distancia + rating)                      |
| Validación                     | Request no nulo, coordenadas, tipo de servicio                |
| Respeto a SOLID                | Controller fino; servicios e interfaces                       |
| Uso de DTOs                    | Request/Response DTOs; no devolver entidades                   |
| Algoritmo de selección         | Provider scoring por distancia y calificación                 |
