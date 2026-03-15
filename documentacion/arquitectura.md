# Arquitectura del Sistema

## 1. Introducción

El sistema propuesto permite la **asignación inteligente de proveedores de asistencia vehicular** (grúa, cerrajería, batería) cuando un usuario realiza una solicitud desde una aplicación móvil.

El objetivo del sistema es identificar el proveedor más adecuado considerando factores como:

- Distancia al cliente
- Tiempo estimado de llegada (ETA)
- Disponibilidad del proveedor
- Calificación del proveedor

La solución debe ser **escalable, resiliente y segura**, permitiendo procesar múltiples solicitudes simultáneamente y ofreciendo seguimiento en tiempo real al usuario.

---

# 2. Flujo general del sistema

El flujo funcional del sistema es el siguiente:

1. El usuario solicita una asistencia desde la aplicación móvil.
2. La solicitud es enviada al API Gateway.
3. El servicio de solicitudes registra el caso.
4. Se obtiene la ubicación del cliente.
5. Se consultan proveedores disponibles.
6. Se ejecuta el algoritmo de optimización para seleccionar el proveedor más adecuado.
7. Se envía una notificación al proveedor seleccionado.
8. Se registra el estado del servicio.
9. El usuario puede visualizar el seguimiento en tiempo real.

---

# 3. Estilo de arquitectura

La solución sigue los siguientes principios arquitectónicos:

- Arquitectura de microservicios
- Domain Driven Design (DDD)
- Clean Architecture
- Comunicación basada en eventos
- Procesamiento asíncrono
- Escalabilidad horizontal

Esto permite que cada componente del sistema evolucione de manera independiente y soporte altos volúmenes de tráfico.

---

# 4. Arquitectura en la nube (AWS)

La solución se diseña sobre infraestructura AWS utilizando servicios administrados para garantizar alta disponibilidad, seguridad y escalabilidad.

## Componentes principales

### Cliente

- Aplicación móvil
- Aplicación web (React)

### CDN

AWS CloudFront se utiliza para distribuir contenido estático del frontend.

### Frontend Hosting

El frontend es desplegado en:

- AWS S3

### API Gateway

AWS API Gateway expone los servicios backend y gestiona:

- autenticación
- control de acceso
- limitación de tráfico (rate limiting)

### Autenticación

El sistema utiliza autenticación basada en:

- JWT
- OAuth2

Los tokens se validan en el API Gateway antes de acceder a los microservicios.

### Microservicios

Los microservicios se ejecutan en contenedores usando:

- AWS ECS Fargate

Cada microservicio es independiente y se comunica mediante eventos.

### Broker de Mensajes

Para comunicación asíncrona entre servicios se utiliza:

- AWS SQS

Esto permite desacoplar los servicios y mejorar la resiliencia.

### Base de Datos

El sistema utiliza:

- AWS RDS PostgreSQL

para almacenamiento transaccional.

### Cache

Para optimizar consultas frecuentes se utiliza:

- AWS ElastiCache Redis

### Observabilidad

Para monitoreo y registro de eventos se utilizan:

- AWS CloudWatch
- métricas
- logs
- alertas

### Seguridad

La plataforma implementa controles de seguridad mediante:

- AWS IAM Roles
- AWS WAF
- AWS Secrets Manager
- Auditoría de accesos

---

# 5. Microservicios del sistema

El sistema se compone de los siguientes microservicios principales:

## AssistanceRequestService

Responsable de gestionar las solicitudes de asistencia generadas por los usuarios.

Funciones:

- crear solicitudes
- actualizar estado de asistencia
- almacenar información del caso

Entidad principal:

AssistanceRequest

Eventos generados:

- AssistanceRequested
- AssistanceAssigned

---

## ProviderOptimizerService

Responsable de seleccionar el proveedor óptimo para atender la solicitud.

El algoritmo considera:

- distancia
- tiempo estimado de llegada (ETA)
- disponibilidad
- calificación del proveedor

Este microservicio es el componente crítico del sistema.

Eventos consumidos:

- AssistanceRequested

Eventos generados:

- ProviderAssigned

---

## LocationService

Encargado de obtener y procesar información geográfica.

Funciones:

- cálculo de distancia
- cálculo de ETA
- geolocalización

Puede integrarse con servicios externos como Google Maps.

---

## NotificationService

Encargado de enviar notificaciones a:

- proveedores
- usuarios

Canales de comunicación:

- notificaciones push
- SMS
- correo electrónico

Eventos consumidos:

- ProviderAssigned

---

# 6. Comunicación entre servicios

La comunicación entre microservicios sigue dos patrones:

### Comunicación síncrona

A través de APIs REST expuestas mediante API Gateway.

### Comunicación asíncrona

A través de eventos enviados mediante SQS.

Ejemplo de flujo de eventos:

AssistanceRequested → ProviderOptimizerService  
ProviderAssigned → NotificationService

---

# 7. Estrategia de escalabilidad

La arquitectura permite escalar automáticamente mediante:

- Auto Scaling en ECS
- Procesamiento asíncrono mediante SQS
- Balanceadores de carga
- Cache Redis

Esto permite manejar picos de solicitudes sin afectar la disponibilidad del sistema.

---

# 8. Estrategia de seguridad

El sistema implementa múltiples capas de seguridad:

- Autenticación con JWT
- Protección mediante AWS WAF
- Control de acceso mediante IAM Roles
- Gestión segura de credenciales con Secrets Manager
- Validación de entrada para prevenir vulnerabilidades OWASP

---

# 9. Observabilidad

La solución incorpora monitoreo completo mediante:

- AWS CloudWatch
- métricas de aplicación
- trazabilidad de solicitudes
- alertas operativas

Esto permite detectar problemas de rendimiento o fallas del sistema en tiempo real.