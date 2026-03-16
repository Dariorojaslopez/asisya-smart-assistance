# ProviderOptimizerService

API REST en ASP.NET Core que selecciona el mejor proveedor de asistencia (grúa, batería, combustible, etc.) según la ubicación del cliente, la disponibilidad y la calificación del proveedor.

## Descripción del servicio de optimización

El servicio de optimización (`ProveedorOptimizerService`) elige al mejor proveedor siguiendo este criterio:

1. **Solo proveedores disponibles**: se consideran únicamente proveedores con `Disponible == true`.
2. **Menor distancia al cliente**: la distancia se calcula con la **fórmula de Haversine** entre las coordenadas del cliente y las de cada proveedor (en kilómetros).
3. **Mayor calificación**: a igual distancia, se elige el proveedor con mayor `Calificacion`.

El algoritmo: filtra por disponibilidad → calcula distancias → ordena por distancia ascendente → a igual distancia ordena por calificación descendente → devuelve el primero o `null` si no hay ninguno disponible.

## Arquitectura

- **Controllers**: exponen los endpoints (optimización y catálogos). Validan la entrada y delegan en servicios.
- **Services**: lógica de negocio (selección del mejor proveedor, cálculo de distancia Haversine).
- **Domain**: entidades y catálogos (Proveedor, Catalogos.TiposAsistencia).
- **DTOs**: modelos de entrada/salida de la API (OptimizeRequest).

El servicio `ProveedorOptimizerService` se registra por inyección de dependencias (Scoped). Los controladores usan `ILogger` para registrar solicitudes, proveedor seleccionado y ausencia de proveedores.

## Estructura del proyecto

```
ProviderOptimizerService
│
├── Controllers
│   ├── OptimizeController.cs
│   └── CatalogosController.cs
│
├── Services
│   └── ProveedorOptimizerService.cs
│
├── Domain
│   ├── Proveedor.cs
│   └── Catalogos.cs
│
├── DTOs
│   └── OptimizeRequest.cs
│
├── README.md
└── Program.cs
```

## Endpoints

| Método | Ruta | Descripción |
|--------|------|-------------|
| **POST** | `/optimize` | Recibe ubicación y tipo de asistencia; devuelve el proveedor óptimo. |
| **GET** | `/catalogos/tipos-asistencia` | Devuelve el catálogo de tipos de asistencia (id → nombre). |

### POST /optimize

- **Request body**: `OptimizeRequest` (JSON).
  - `Lat` (number): latitud del cliente.
  - `Lng` (number): longitud del cliente.
  - `TipoAsistencia` (number): id del tipo de asistencia (válidos en GET `/catalogos/tipos-asistencia`).
- **Respuestas**:
  - **200**: proveedor seleccionado (objeto Proveedor).
  - **400**: request nulo o `TipoAsistencia` no válido.
  - **404**: no hay ningún proveedor disponible.

### GET /catalogos/tipos-asistencia

- **Respuesta**: objeto con pares id → nombre de tipo de asistencia (ej. `{ "1": "Grua", "2": "Bateria", ... }`).

## Ejemplos de peticiones

### Obtener tipos de asistencia

```http
GET /catalogos/tipos-asistencia
```

### Solicitar proveedor óptimo

```http
POST /optimize
Content-Type: application/json

{
  "lat": 4.6533,
  "lng": -74.0836,
  "tipoAsistencia": 1
}
```

Ejemplo de respuesta 200:

```json
{
  "id": "P2",
  "latitud": 4.66,
  "longitud": -74.04,
  "calificacion": 4.8,
  "disponible": true
}
```

## Cómo ejecutar la API

1. Tener instalado [.NET 8 SDK](https://dotnet.microsoft.com/download).
2. En la raíz del proyecto (donde está `ProviderOptimizerService.csproj`), ejecutar:

```bash
dotnet run
```

3. La API quedará disponible (según `launchSettings`) en la URL configurada (por ejemplo `https://localhost:7xxx` o `http://localhost:5xxx`).
4. **Swagger** está disponible en: **/swagger** (en desarrollo).

Para generar documentación XML en compilación (opcional), habilita `<GenerateDocumentationFile>true</GenerateDocumentationFile>` en el `.csproj`.
