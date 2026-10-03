# Diccionario de datos

Base de datos relacional PostgreSQL para el Sistema de Objetos Perdidos en Universidad.

## Tabla: items

| Campo | Tipo | Restricciones | Descripcion |
| --- | --- | --- | --- |
| id | uuid | PK | Identificador del objeto. |
| name | varchar(120) | requerido | Nombre corto del objeto. |
| description | varchar(800) | requerido | Descripcion del objeto. |
| category | varchar(80) | requerido | Categoria del objeto. |
| location | varchar(120) | requerido | Lugar donde se perdio o encontro. |
| faculty | varchar(120) | requerido | Facultad asociada al reporte. |
| report_date | timestamptz | requerido | Fecha del reporte. |
| item_type | varchar(20) | requerido | Lost o Found. |
| status | varchar(20) | requerido | Reported, InCustody, Claimed o Returned. |
| photo_url | varchar(500) | opcional | URL de foto referencial. |
| characteristics | varchar(1000) | requerido | Rasgos que ayudan a validar propiedad. |
| created_at | timestamptz | requerido | Fecha de creacion del registro. |
| updated_at | timestamptz | requerido | Fecha de ultima actualizacion. |

## Tabla: claims

| Campo | Tipo | Restricciones | Descripcion |
| --- | --- | --- | --- |
| id | uuid | PK | Identificador de reclamacion. |
| item_id | uuid | FK items.id | Objeto reclamado. |
| claimant_name | varchar(160) | requerido | Nombre del reclamante. |
| claimant_email | varchar(200) | requerido, email | Correo del reclamante. |
| ownership_evidence | varchar(1000) | requerido | Evidencia de propiedad. |
| requested_at | timestamptz | requerido | Fecha de solicitud. |
| is_verified | boolean | requerido | Indica si se valido la propiedad. |
| verified_at | timestamptz | opcional | Fecha de verificacion. |

## Tabla: return_records

| Campo | Tipo | Restricciones | Descripcion |
| --- | --- | --- | --- |
| id | uuid | PK | Identificador de devolucion. |
| item_id | uuid | FK items.id | Objeto devuelto. |
| claim_id | uuid | FK claims.id, unico | Reclamo verificado usado para devolver. |
| responsible_person | varchar(160) | requerido | Persona que registra la devolucion. |
| return_date | timestamptz | requerido | Fecha de devolucion. |
| owner_confirmation | boolean | requerido | Confirmacion de conformidad del propietario. |
| notes | varchar(1000) | opcional | Observaciones de entrega. |
