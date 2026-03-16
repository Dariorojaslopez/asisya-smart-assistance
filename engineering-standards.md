# Engineering Standards

Este documento describe los estándares de desarrollo del equipo para mantener consistencia, calidad y mantenibilidad en el código.

---

## 1) .NET coding conventions

- **Clean Architecture**: Organizar la solución en capas claras (Domain, Application, Infrastructure, API). El dominio no depende de infraestructura ni del framework; las dependencias apuntan hacia el núcleo.
- **Controllers thin**: Los controladores solo orquestan: validan entrada, llaman a servicios y devuelven DTOs y códigos HTTP. No deben contener lógica de negocio ni acceso directo a datos.
- **Services contain business logic**: Toda la lógica de negocio vive en la capa de servicios (o Application). Los controladores y la infraestructura delegan en estos servicios.
- **DTOs for API responses**: Usar DTOs para las respuestas (y solicitudes) de la API. No exponer entidades de dominio ni modelos de EF directamente; mapear a DTOs para mantener un contrato estable y evitar acoplamiento.

---

## 2) React coding conventions

- **Component-based architecture**: Dividir la UI en componentes reutilizables y con una única responsabilidad. Preferir composición sobre herencia.
- **Hooks**: Usar Hooks (useState, useEffect, useContext, custom hooks) para estado y efectos. Evitar class components en código nuevo.
- **API clients separated**: Centralizar las llamadas a APIs en clientes o módulos dedicados (por ejemplo, servicios o capa `api/`). No dispersar fetch/axios en los componentes.

---

## 3) Docker standards

- **One service per container**: Un solo proceso o servicio por contenedor. No ejecutar varios servicios (API + DB, etc.) en la misma imagen.
- **Environment variables for secrets**: Configuración sensible (connection strings, API keys) mediante variables de entorno, no hardcodeada en Dockerfile ni en el código.
- **Images versioned**: Etiquetar las imágenes con versiones o tags identificables (por ejemplo, por commit, tag Git o versión semántica) para trazabilidad y despliegues reproducibles.

---

## 4) Branch strategy

Usar **GitFlow**:

| Rama | Uso |
|------|-----|
| **main** | Código en producción. Solo se actualiza mediante merges desde `develop` o `hotfix/*`. |
| **develop** | Rama de integración para la siguiente release. Las features se integran aquí. |
| **feature/*** | Nuevas funcionalidades. Se crean desde `develop` y se fusionan de vuelta en `develop` (por ejemplo, `feature/optimize-provider-selection`). |
| **hotfix/*** | Correcciones urgentes de producción. Se crean desde `main`, se fusionan en `main` y en `develop` (por ejemplo, `hotfix/fix-null-reference`). |

---

## 5) Code review policy

- **Minimum 1 reviewer**: Todo cambio que se integre en `develop` o `main` debe ser aprobado por al menos una persona distinta al autor.
- **CI must pass**: El pipeline de CI (build, tests, Docker build, etc.) debe estar en verde antes de aprobar el PR.
- **No direct commits to main**: No se hacen commits directos a `main`. Los cambios entran vía pull/merge request revisado y con CI pasado.

---

## 6) Definition of Done

Un ítem se considera terminado cuando se cumple:

- **Code compiled**: El proyecto compila sin errores en la configuración utilizada (por ejemplo, Release).
- **Tests passing**: Los tests unitarios e integración relevantes pasan (`dotnet test` o equivalente).
- **Docker build works**: La imagen Docker del servicio se construye correctamente y, si aplica, el stack (por ejemplo, `docker compose up`) funciona.
- **Swagger documentation updated**: Los endpoints nuevos o modificados están documentados en Swagger (comentarios XML, ejemplos, descripciones) cuando aplique.

---

## 7) Secret management

- **Use environment variables**: Contraseñas, connection strings, API keys y tokens se inyectan vía variables de entorno (o gestores de secretos) en tiempo de ejecución, no en el código ni en archivos versionados.
- **Never commit credentials**: No subir al repositorio archivos con credenciales (por ejemplo, `appsettings.Production.json` con secrets). Usar `appsettings.Development.json` con valores locales y mantenerlo fuera del commit si contiene datos sensibles, o usar placeholders y documentar el uso de env vars.

---

## 8) Technical debt policy

- **Track debt using backlog tasks**: Registrar la deuda técnica como tareas en el backlog (bugs, refactors, mejoras de diseño) con prioridad y criterios de aceptación cuando sea posible.
- **Periodic refactoring sprints**: Incluir en la planificación sprints o ventanas dedicadas a refactorizar y reducir deuda (por ejemplo, un porcentaje de capacidad o iteraciones específicas).
- **Evitar acumular deuda sin visibilidad**: Evitar dejar “para después” sin anotarlo; si se acepta deuda de forma explícita, debe quedar documentada o en el backlog.
