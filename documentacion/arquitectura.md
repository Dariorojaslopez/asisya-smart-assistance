# Arquitectura del Sistema

## 1. Visión General

El sistema **Provider Optimizer** es una plataforma diseñada para seleccionar el proveedor de asistencia más adecuado en función de la ubicación del cliente y el tipo de asistencia requerida.

La solución implementa una arquitectura moderna basada en microservicios ligeros y separación de responsabilidades entre frontend, backend y base de datos.

El sistema permite:

- Autenticación mediante JWT
- Selección de tipo de asistencia
- Ingreso de ubicación manual o mediante mapa
- Optimización de proveedores basada en proximidad
- Visualización de resultados en tabla y mapa interactivo

---

# 2. Arquitectura General

La arquitectura se compone de tres capas principales:
React Frontend
      │
      │ HTTP REST API
      ▼
ASP.NET Core API
      │
      │ Entity Framework
      ▼
PostgreSQL Database

### Frontend

Tecnología:

- React
- React Router
- Axios
- Leaflet (mapa interactivo)

Responsabilidades:

- Interfaz de usuario
- Selección de ubicación
- Consumo de API REST
- Visualización de resultados
- Gestión de autenticación JWT

---

### Backend

Tecnología:

- ASP.NET Core (.NET 8)
- Entity Framework Core
- JWT Authentication

Responsabilidades:

- Autenticación de usuarios
- Gestión de usuarios (CRUD)
- Catálogo de tipos de asistencia
- Optimización de proveedores
- Exposición de API REST

---

### Base de Datos

Motor:

PostgreSQL

Tablas principales:

- Users
- Providers
- AssistanceTypes

Responsabilidades:

- Persistencia de usuarios
- Persistencia de proveedores
- Catálogo de asistencias

---

# 3. Flujo del Sistema

El flujo principal del sistema es el siguiente:

1. El usuario inicia sesión mediante autenticación JWT.
2. El usuario selecciona el tipo de asistencia.
3. El usuario define la ubicación manualmente o mediante el mapa.
4. El frontend invoca el endpoint de optimización.
5. El backend calcula el proveedor óptimo.
6. Los resultados se muestran en tabla y en el mapa.

---

# 4. Arquitectura Backend

El backend sigue principios de **Clean Architecture**, separando las responsabilidades en diferentes capas:
Controllers
    │
Services
    │
Domain
    │
Infrastructure

### Controllers

Exponen los endpoints REST.

Ejemplo:

- AuthController
- ProvidersController
- UsersController
- CatalogController

---

### Services

Contienen la lógica de negocio:

- Optimización de proveedores
- Validaciones
- Transformación de datos

---

### Domain

Define entidades del sistema:

- User
- Provider
- AssistanceType

---

### Infrastructure

Responsable de:

- acceso a base de datos
- configuración de Entity Framework
- migraciones

---

# 5. Infraestructura y Contenedores

El sistema utiliza **Docker Compose** para orquestar los servicios:

Servicios desplegados:

- frontend
- backend API
- PostgreSQL

Esto permite levantar el sistema completo mediante:
docker compose up --build

---

# 6. Seguridad

El sistema implementa:

- Autenticación JWT
- Protección de endpoints mediante `[Authorize]`
- Contraseñas almacenadas con **BCrypt**
- Manejo de secretos mediante variables de entorno

---

# 7. Escalabilidad

La arquitectura permite escalar:

- frontend independiente
- backend horizontalmente
- base de datos gestionada externamente

La separación de capas facilita la evolución del sistema hacia una arquitectura de microservicios en el futuro.

---