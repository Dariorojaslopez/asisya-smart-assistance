# Provider Optimizer – Asisya Smart Assistance.

## 1. Descripción del Proyecto

**Provider Optimizer** es una aplicación diseñada para seleccionar el proveedor de asistencia más adecuado según la ubicación del cliente y el tipo de servicio solicitado.

La solución implementa una arquitectura moderna basada en **React + ASP.NET Core + PostgreSQL**, desplegada mediante **Docker Compose**.

El sistema permite:

* Autenticación mediante JWT
* Gestión de usuarios
* Selección de tipo de asistencia
* Ingreso de ubicación manual o mediante mapa
* Optimización de proveedores según proximidad
* Visualización de resultados en tabla y mapa interactivo

---

# 2. Arquitectura del Sistema

La solución está compuesta por tres componentes principales:

```
React Frontend
      │
      │ HTTP REST API
      ▼
ASP.NET Core Backend
      │
      │ Entity Framework
      ▼
PostgreSQL Database
```

## Frontend

Tecnologías utilizadas:

* React
* React Router
* Axios
* Leaflet (mapa interactivo)

Responsabilidades:

* interfaz de usuario
* selección de ubicación en mapa
* consumo de servicios REST
* visualización de proveedores

---

## Backend

Tecnologías utilizadas:

* ASP.NET Core (.NET 8)
* Entity Framework Core
* JWT Authentication

Responsabilidades:

* autenticación de usuarios
* gestión de usuarios (CRUD)
* catálogo de tipos de asistencia
* optimización de proveedores
* exposición de API REST

---

## Base de Datos

Motor:

PostgreSQL

Tablas principales:

* Users
* Providers
* AssistanceTypes

---

# 3. Funcionalidades Implementadas

## Autenticación

* login mediante JWT
* contraseñas almacenadas con **BCrypt**
* protección de endpoints con `[Authorize]`

---

## Optimización de Proveedores

El sistema permite encontrar el proveedor óptimo basado en:

* ubicación del cliente
* tipo de asistencia
* distancia estimada
* tiempo estimado de llegada (ETA)

Los resultados se visualizan:

* en una tabla
* en un mapa interactivo

---

## Selección de Ubicación

El usuario puede:

* ingresar **latitud y longitud manualmente**
* seleccionar la ubicación **directamente en el mapa**

---

## Visualización en Mapa

El mapa muestra:

* ubicación del cliente
* proveedores disponibles
* proveedor óptimo resaltado

---

# 4. Estructura del Proyecto

```
asisya-smart-assistance

frontend/                 # Aplicación React
servicios/                # Backend ASP.NET Core
documentacion/            # Documentación técnica
diagramas/                # Diagramas de arquitectura

docker-compose.yml        # Orquestación de servicios
README.md
```

---

# 5. Requisitos

Para ejecutar el proyecto se requiere:

* Docker
* Docker Compose

No es necesario instalar dependencias adicionales.

---

# 6. Ejecución del Proyecto

Para levantar todo el sistema ejecutar:

```
docker compose up --build
```

Esto iniciará los siguientes servicios:

* frontend
* backend API
* PostgreSQL

---

# 7. Acceso al Sistema

Frontend:

```
http://localhost:3000
```

Swagger API:

```
http://localhost:8080/swagger
```

---

# 8. Usuario Inicial

El sistema crea automáticamente un usuario administrador al iniciar por primera vez.

Credenciales:

```
usuario: admin
password: admin
```

---

# 9. Endpoints Principales

### Autenticación

```
POST /auth/login
```

---

### Catálogo de Asistencias

```
GET /catalogos/tipos-asistencia
```

---

### Optimización de Proveedores

```
POST /optimize
```

---

### Gestión de Usuarios

```
GET /users
POST /users
PUT /users/{id}
DELETE /users/{id}
```

---

# 10. Infraestructura

El proyecto utiliza **Docker Compose** para ejecutar todos los servicios de forma integrada.

Servicios definidos:

* frontend
* backend
* base de datos PostgreSQL

Esto permite replicar el entorno completo con un solo comando.

---

# 11. Documentación Técnica

En la carpeta `documentacion` se incluyen los siguientes documentos:

* arquitectura del sistema
* estándares técnicos del squad
* políticas de revisión de código

---

# 12. Autor

Proyecto desarrollado como parte de una prueba técnica para el rol de **Software Engineer / Tech Lead**.
