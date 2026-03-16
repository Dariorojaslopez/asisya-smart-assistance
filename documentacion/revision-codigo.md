# Política de Revisión de Código

1. Objetivo

El objetivo de la revisión de código es asegurar la calidad, seguridad y mantenibilidad del software desarrollado por el squad.

El proceso de revisión permite:

- detectar errores tempranamente
- compartir conocimiento entre los miembros del equipo
- mantener consistencia en el código
- garantizar el cumplimiento de los estándares técnicos

---

2. Flujo de Revisión

Todo cambio en el código debe seguir el siguiente flujo:
Developer
   ↓
Pull Request
   ↓
Code Review
   ↓
Aprobación
   ↓
Merge a develop

3. Creación de Pull Request

Antes de crear un Pull Request el desarrollador debe verificar:

el código compila correctamente

no existen errores de linting

la funcionalidad ha sido probada localmente

se cumplen los estándares definidos por el squad

El Pull Request debe incluir:

descripción del cambio

referencia a la tarea o historia de usuario

evidencia de pruebas realizadas

4. Criterios de Revisión

Durante la revisión de código se evalúan los siguientes aspectos:

Calidad del código

claridad en nombres de variables y métodos

estructura lógica del código

ausencia de código duplicado

Buenas prácticas

cumplimiento de estándares del proyecto

separación adecuada de responsabilidades

uso correcto de patrones de diseño

Seguridad

no exposición de credenciales

validación adecuada de entradas

uso correcto de autenticación y autorización

Rendimiento

evitar consultas innecesarias

uso eficiente de recursos

5. Reglas de Aprobación

Un Pull Request requiere:

al menos 1 aprobación de otro miembro del squad

resolución de todos los comentarios críticos

ejecución exitosa del pipeline de CI

No se permite realizar merge directo a la rama principal sin revisión.

6. Buenas Prácticas del Squad

Para facilitar las revisiones de código se recomienda:

crear Pull Requests pequeños

incluir comentarios claros en el código

dividir funcionalidades complejas en módulos

documentar cambios relevantes

7. Herramientas Utilizadas

El squad utiliza las siguientes herramientas para el proceso de revisión:

GitHub Pull Requests

GitHub Actions para validación automática

herramientas de análisis estático cuando aplica

8. Beneficios del Proceso

La revisión de código permite:

mejorar la calidad del software

reducir errores en producción

fomentar aprendizaje dentro del equipo

mantener consistencia en la arquitectura

9. Conclusión

La revisión de código es una práctica fundamental para asegurar que el software desarrollado por el squad cumpla con los estándares de calidad definidos y pueda evolucionar de forma segura y mantenible.

