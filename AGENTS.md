# AGENTS.md

## Proyecto

- Proyecto universitario SI784.
- Sistema de Objetos Perdidos en Universidad.
- Mantener una arquitectura sencilla, clara y facil de explicar.
- No desarrollar funcionalidades fuera del alcance del sistema sin autorizacion previa.

## Stack obligatorio

- Backend ASP.NET Core.
- API REST para el backend.
- Entity Framework Core para persistencia.
- PostgreSQL como base de datos relacional.
- Frontend React + Vite.
- Docker obligatorio para el backend.
- Terraform obligatorio para infraestructura.
- GitHub Actions obligatorio para automatizacion.

## Workflows obligatorios

- `.github/workflows/infra.yml` obligatorio.
- `.github/workflows/sonar.yml` obligatorio.
- `.github/workflows/snyk-semgrep.yml` obligatorio.
- `.github/workflows/deploy.yml` obligatorio.
- `.github/workflows/generate-documentation.yml` obligatorio.

## Calidad y pruebas

- Implementar pruebas unitarias.
- Implementar pruebas de integracion.
- Verificar compilacion y pruebas antes de terminar cada etapa.
- Aplicar validaciones en frontend y backend.
- Sonar debe quedar sin bugs, vulnerabilidades ni security hotspots.
- Snyk y Semgrep deben revisar codigo e imagen de contenedor.

## Seguridad

- No guardar contrasenas, tokens ni secretos en el repositorio.
- Usar variables de entorno o secretos de GitHub Actions para credenciales.
- No incluir datos sensibles en archivos de configuracion versionados.
- Revisar dependencias y contenedor con herramientas de seguridad.

## Documentacion

- Generar diccionario de datos.
- Generar diagrama entidad-relacion.
- Generar diagrama de clases.
- Generar diagrama de componentes.
- Generar diagrama de despliegue.
- Usar Mermaid para todos los diagramas.
- Mantener README y documentacion tecnica actualizados.

## Git

- NO hacer git commit automaticamente.
- NO hacer git push automaticamente.
- Solo se realizaran 4 commits importantes en todo el proyecto.
- Esperar autorizacion explicita antes de cada commit.
- No borrar archivos salvo autorizacion explicita.

