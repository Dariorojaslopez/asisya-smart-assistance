# Estándares Técnicos del Squad

## 1. Objetivo

Este documento define los estándares técnicos utilizados por el squad para el desarrollo, mantenimiento y despliegue del sistema **Provider Optimizer**.

El objetivo es garantizar:

- consistencia en el código
- calidad en las entregas
- facilidad de mantenimiento
- seguridad en el manejo de la información
- colaboración eficiente entre desarrolladores

---

# 2. Convenciones de Código

## 2.1 Estándares para .NET

El backend del sistema está desarrollado en **ASP.NET Core (.NET 8)** siguiendo principios de **Clean Architecture**.

### Convenciones de nombres

- Controllers deben terminar en `Controller`

Ejemplo:
AuthController
UsersController
ProvidersController

- DTOs deben terminar en:
Request
Response

Ejemplo:
LoginRequest
LoginResponse
CreateUserRequest

- Servicios deben terminar en:
Ejemplo:
ProviderOptimizerService
AuthService

---

### Buenas prácticas

- Uso obligatorio de `async / await`
- Separación de lógica de negocio en servicios
- No colocar lógica en los controllers
- Validaciones en capa de aplicación
- Uso de DTOs para comunicación con el frontend

---

## 2.2 Estándares para React

El frontend está desarrollado en **React**.

### Convenciones de nombres

Componentes:
PascalCase

Ejemplo:
LoginPage
ProvidersPage
UsersPage
MapComponent

Hooks personalizados:
useNombreHook

Ejemplo:
useAuth
useProviders


---

### Estructura recomendada

frontend/
├ pages
├ components
├ services
├ hooks
├ styles


---

### Buenas prácticas

- Componentes pequeños y reutilizables
- Separación entre UI y lógica
- Consumo de API centralizado
- Uso de `Axios` para llamadas HTTP
- Manejo de estado mediante hooks

---

## 2.3 Estándares para Docker

El sistema utiliza **Docker** para garantizar entornos consistentes.

### Buenas prácticas

- Uso de **multi-stage builds**
- Uso de imágenes oficiales
- Variables sensibles mediante **environment variables**
- No almacenar secretos en el código

Ejemplo:
JWT_SECRET
POSTGRES_PASSWORD


---

# 3. Políticas del Squad

## 3.1 Estrategia de ramas (Branch Strategy)

El proyecto utiliza una estrategia basada en **GitFlow simplificado**.

Ramas principales:
main
develop

Tipos de ramas:
feature/*
bugfix/*
hotfix/*

Ejemplo:

feature/provider-map
feature/authentication
bugfix/login-error


---

## 3.2 Code Reviews

Todo cambio en el código debe pasar por **Pull Request**.

Reglas:

- mínimo **1 aprobación**
- revisión de calidad
- revisión de seguridad
- revisión de arquitectura

Aspectos revisados:

- legibilidad del código
- uso correcto de patrones
- ausencia de código duplicado
- cumplimiento de estándares

---

## 3.3 Integración Continua (CI)

El proyecto utiliza **GitHub Actions** para automatizar procesos.

Pipeline de CI:
1. Instalación de dependencias
2. Compilación
3. Ejecución de pruebas
4. Construcción de imagen Docker


Esto garantiza que el código sea validado antes de integrarse.

---

## 3.4 Definition of Done

Una tarea se considera terminada cuando cumple:

- código implementado
- código revisado
- pruebas ejecutadas
- documentación actualizada
- integración exitosa en `develop`

---

## 3.5 Manejo de secretos

Los secretos nunca deben almacenarse en el código fuente.

Se gestionan mediante:

- variables de entorno
- Docker environment variables
- GitHub Secrets en CI/CD

Ejemplos:
JWT_SECRET
DATABASE_URL


---

## 3.6 Arquitectura base del squad

El backend sigue **Clean Architecture**, separando responsabilidades:
Controllers
Services
Domain
Infrastructure


Esto permite:

- mayor mantenibilidad
- facilidad de pruebas
- desacoplamiento entre capas

---

## 3.7 Control de deuda técnica

La deuda técnica se gestiona mediante:

- tareas en backlog
- revisiones de código
- refactorizaciones planificadas

Se promueve:

- evitar código duplicado
- mantener cobertura de pruebas
- mejorar continuamente la arquitectura

---

# 4. Conclusión

La adopción de estos estándares permite al squad mantener un desarrollo consistente, escalable y seguro, facilitando la colaboración entre los miembros del equipo y garantizando la calidad del software entregado.