# Sistema de Objetos Perdidos en Universidad

Plataforma web para registrar objetos perdidos o encontrados en una universidad, gestionar custodia, validar propiedad, registrar reclamaciones y formalizar devoluciones con trazabilidad.

## Funcionalidades

- Registro de objetos perdidos y encontrados.
- Consulta, detalle y filtros por estado, ubicacion, facultad y fecha.
- Reclamacion de objetos.
- Verificacion de propietario.
- Registro de devolucion.
- Estados: `Reported`, `InCustody`, `Claimed`, `Returned`.

## Arquitectura

- Backend ASP.NET Core Web API.
- Entity Framework Core con PostgreSQL.
- Frontend React + Vite + TypeScript.
- Docker para empaquetar el backend.
- Terraform para infraestructura Azure.
- GitHub Actions para infraestructura, calidad, seguridad, despliegue y documentacion.

## Estructura

```text
backend/                 API REST ASP.NET Core
frontend/                Aplicacion React + Vite
tests/                   Pruebas unitarias
docs/                    Diccionario y diagramas Mermaid
infra/                   Terraform
.github/workflows/       Automatizaciones CI/CD
```

## Backend

Configurar la cadena de conexion mediante variable de entorno:

```powershell
$env:POSTGRES_CONNECTION_STRING="Host=localhost;Port=5432;Database=lostfound;Username=postgres;Password=<password>"
dotnet run --project backend/LostAndFound.Api.csproj
```

OpenAPI queda disponible en desarrollo en:

```text
/openapi/v1.json
```

## Frontend

```powershell
cd frontend
npm install
$env:VITE_API_URL="http://localhost:5195"
npm run dev
```

## Endpoints

| Metodo | Ruta | Uso |
| --- | --- | --- |
| POST | `/lost-items` | Registrar objeto perdido. |
| POST | `/found-items` | Registrar objeto encontrado. |
| GET | `/items` | Listar objetos. |
| GET | `/items?status={estado}` | Filtrar por estado. |
| GET | `/items?location=&faculty=&date=` | Filtros simples. |
| GET | `/items/{id}` | Ver detalle. |
| POST | `/items/{id}/claim` | Reclamar objeto. |
| POST | `/items/{id}/verify-owner` | Verificar propietario. |
| POST | `/items/{id}/return` | Registrar devolucion. |

## Docker

```powershell
docker build -f backend/Dockerfile -t lost-found-api .
docker run -p 8080:8080 -e POSTGRES_CONNECTION_STRING="<connection-string>" lost-found-api
```

## Terraform

```powershell
cd infra
terraform init -backend=false
terraform validate
terraform plan
```

No ejecutar `terraform apply` sin revisar el plan y configurar secretos.

## Workflows

- `infra.yml`: valida y planifica infraestructura Terraform.
- `sonar.yml`: compila, prueba y analiza con Sonar.
- `snyk-semgrep.yml`: revisa codigo, dependencias e imagen Docker con Snyk y Semgrep.
- `deploy.yml`: construye imagen y despliega en Azure Web App.
- `generate-documentation.yml`: valida y publica documentacion como artifact.

El profesor menciona `generase-documentation.yml`; se usa `generate-documentation.yml` por consistencia gramatical y con la lista de entrega.

## Seguridad

- No se versionan tokens, contrasenas ni secretos reales.
- Las credenciales deben ir en GitHub Secrets.
- Sonar debe quedar sin bugs, vulnerabilidades ni security hotspots.
- Snyk y Semgrep revisan codigo e imagen de contenedor.

## Pruebas

```powershell
dotnet restore LostAndFoundUniversity.sln -m:1
dotnet build LostAndFoundUniversity.sln --no-restore -m:1
dotnet test LostAndFoundUniversity.sln --no-build -m:1
```

## Documentacion

- `docs/data-dictionary.md`
- `docs/entity-relationship.md`
- `docs/class-diagram.md`
- `docs/component-diagram.md`
- `docs/deployment-diagram.md`

## URLs de entrega

Aplicacion publicada:
https://lost-found-university-api.azurewebsites.net

Repositorio:
https://github.com/UPT-FAING-EPIS/si784-2026-ii-si784-2026-ii-examen-u1-anaesteban1

Sonar:
https://sonarcloud.io/dashboard?id=lost-found-university_si784-2026-ii-si784-2026-ii-examen-u1-anaesteban1
