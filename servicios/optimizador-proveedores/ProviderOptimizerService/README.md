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

### Opción 1: Con Docker (PostgreSQL + API)

Desde la raíz del proyecto (donde están `Dockerfile` y `docker-compose.yml`):

```bash
docker compose up --build
```

- Se levantan dos contenedores: **postgres** (PostgreSQL 15) y **provider_optimizer_api** (.NET 8).
- La API espera a que PostgreSQL esté listo antes de arrancar y aplica las migraciones EF Core al inicio.
- **API**: `http://localhost:8080`
- **Swagger**: `http://localhost:8080/swagger` (disponible porque la API corre en entorno Development).
- PostgreSQL queda expuesto en el puerto **5432** por si necesitas conectarte con un cliente.

### Opción 2: Local con .NET SDK

1. Tener instalado [.NET 8 SDK](https://dotnet.microsoft.com/download) y PostgreSQL (con la base `provider_optimizer_db` creada).
2. En la raíz del proyecto (donde está `ProviderOptimizerService.csproj`), ejecutar:

```bash
dotnet run
```

3. La API quedará disponible (según `launchSettings`) en la URL configurada (por ejemplo `https://localhost:7xxx` o `http://localhost:5xxx`).
4. **Swagger** está disponible en: **/swagger** (en desarrollo). Con Docker: **http://localhost:8080/swagger**.

Para generar documentación XML en compilación (opcional), habilita `<GenerateDocumentationFile>true</GenerateDocumentationFile>` en el `.csproj`.

---

## Pipeline CI/CD (GitHub Actions)

El proyecto incluye un workflow de integración continua en `.github/workflows/ci.yml` que se ejecuta en cada **push** y en cada **pull request**.

### Pasos del pipeline

| Paso | Descripción |
|------|-------------|
| **Trigger** | `on: push` y `on: pull_request` |
| **Setup .NET 8 SDK** | Instalación del SDK de .NET 8 en el runner (ubuntu-latest). |
| **Restore dependencies** | `dotnet restore` sobre el proyecto de tests (restaura también la API). |
| **Build project** | `dotnet build --no-restore -c Release` sobre el proyecto de tests. |
| **Run tests** | `dotnet test --no-build -c Release` sobre el proyecto de tests. |
| **Build Docker image** | `docker build -t provider-optimizer-service` con contexto en la carpeta de la API. |

Las rutas usadas son relativas a la raíz del repositorio: `servicios/optimizador-proveedores/ProviderOptimizerService` (API) y `servicios/optimizador-proveedores/ProviderOptimizerService.Tests` (tests).

### Publicar imagen en AWS ECR (opcional)

El workflow incluye en comentarios un **ejemplo de configuración** para subir la imagen Docker a **Amazon ECR**. No es obligatorio ni se usan credenciales reales para que el pipeline pase.

Para activarlo:

1. Descomentar el job `push-ecr` y los pasos en `.github/workflows/ci.yml`.
2. Configurar en GitHub **Settings → Secrets and variables → Actions** los secretos:
   - `AWS_ACCESS_KEY_ID`
   - `AWS_SECRET_ACCESS_KEY`
   - `AWS_REGION` (ej. `us-east-1`)
   - Opcional: `AWS_ECR_REGISTRY` si el registro tiene otra URL.
3. Crear en ECR un repositorio llamado `provider-optimizer-service` (o ajustar `ECR_REPOSITORY` en el workflow).

El job opcional suele ejecutarse solo en `push` a `main` y después de que `build-and-test` termine correctamente.
