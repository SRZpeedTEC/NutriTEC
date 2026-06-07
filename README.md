# NutriTEC

NutriTEC es una plataforma para la gestión nutricional de usuarios, pacientes y nutricionistas. El sistema permite registrar clientes, asociarlos con nutricionistas, crear planes alimenticios, registrar consumo diario, gestionar productos y recetas, controlar medidas corporales, aprobar productos y generar reportes administrativos.

Este proyecto corresponde al Proyecto #2 del curso Bases de Datos CE3101 del Instituto Tecnológico de Costa Rica.

## Objetivo del proyecto

Desarrollar una solución web, móvil y de servicios que permita administrar la información nutricional de clientes y nutricionistas, utilizando una base de datos relacional en SQL Server y una base de datos no relacional en MongoDB.

## Funcionalidades principales

### Vista Administrador

* Inicio de sesión unificado.
* Aprobación manual de productos creados por clientes o nutricionistas.
* Reporte de cobro agrupado por tipo de pago.
* Exportación de reportes a PDF.
* Gestión de datos administrativos del sistema.

### Vista Cliente / Paciente

* Registro e inicio de sesión.
* Registro de meta diaria de calorías.
* Registro diario de consumo por tiempo de comida.
* Búsqueda de productos por nombre o código de barras.
* Gestión de productos.
* Gestión de recetas a partir de productos existentes.
* Registro de medidas corporales.
* Reporte de avance por periodo.
* Exportación del reporte de avance a PDF.

### Vista Nutricionista

* Registro e inicio de sesión.
* Asociación de clientes como pacientes.
* Gestión de productos y platillos.
* Creación de planes alimenticios.
* Asignación de planes a pacientes por fecha o periodo.
* Seguimiento de pacientes.
* Retroalimentación tipo foro almacenada en MongoDB.

### App móvil

* Inicio de sesión.
* Registro diario de consumo.
* Gestión de recetas.

## Arquitectura general

El proyecto utiliza una arquitectura por capas para separar responsabilidades y facilitar el mantenimiento del sistema.

```text
NutriTEC/
│
├── src/
│   ├── backend/
│   │   ├── NutriTEC.Api/
│   │   ├── NutriTEC.Application/
│   │   ├── NutriTEC.Domain/
│   │   ├── NutriTEC.Infrastructure/
│   │   ├── NutriTEC.MongoApi/
│   │   ├── NutriTEC.MongoApplication/
│   │   ├── NutriTEC.MongoDomain/
│   │   ├── NutriTEC.MongoInfrastructure/
│   │   └── NutriTEC.sln
│   │
│   ├── frontend/
│   │   └── nutritec-web/
│   │
│   └── mobile/
│       └── nutritec-mobile/
│
├── database/
│   ├── sqlserver/
│   │   ├── schema/
│   │   ├── stored-procedures/
│   │   ├── functions/
│   │   ├── triggers/
│   │   ├── views/
│   │   └── seed/
│   │
│   └── mongodb/
│       ├── collections/
│       ├── indexes/
│       └── seed/
│
├── docs/
│   ├── planning/
│   ├── installation/
│   ├── user-manual/
│   ├── technical-documentation/
│   ├── diagrams/
│   └── minutes/
│
├── deployment/
│   ├── docker/
│   ├── azure/
│   └── aws/
│
├── tests/
│   ├── backend/
│   └── mongo-api/
│
├── .github/
│   └── workflows/
│
├── .gitignore
├── README.md
└── docker-compose.yml
```

## Componentes del backend relacional

### NutriTEC.Api

API principal del sistema. Expone los endpoints relacionados con autenticación, usuarios, clientes, nutricionistas, productos, recetas, planes, medidas, consumo diario, reportes y administración.

### NutriTEC.Application

Contiene la lógica de aplicación, servicios, DTOs, validaciones y contratos de repositorio.

### NutriTEC.Domain

Contiene las entidades principales del dominio, enumeraciones y reglas base del modelo.

### NutriTEC.Infrastructure

Implementa el acceso a SQL Server mediante Entity Framework, repositorios, configuración del contexto, ejecución de procedimientos almacenados y servicios externos.

## Componentes del backend no relacional

### NutriTEC.MongoApi

API independiente para gestionar la retroalimentación tipo foro entre nutricionistas y pacientes.

### NutriTEC.MongoApplication

Contiene los servicios, DTOs y contratos relacionados con los foros de retroalimentación.

### NutriTEC.MongoDomain

Contiene los modelos documentales principales, como hilos de retroalimentación, mensajes y respuestas.

### NutriTEC.MongoInfrastructure

Implementa la conexión con MongoDB, repositorios documentales, configuración de colecciones e índices.

## Bases de datos

### SQL Server

SQL Server almacena la información estructurada del sistema:

* Usuarios.
* Clientes.
* Nutricionistas.
* Administradores.
* Productos.
* Recetas.
* Planes alimenticios.
* Asignaciones de planes.
* Medidas corporales.
* Consumo diario.
* Reportes administrativos.

También incluye:

* Procedimientos almacenados.
* Triggers.
* Vistas.
* Funciones.
* Script de creación de base de datos.
* Script de población inicial.

### MongoDB

MongoDB almacena la información no estructurada o semiestructurada relacionada con la retroalimentación de nutricionistas:

* Hilos de retroalimentación.
* Mensajes.
* Respuestas.
* Historial de conversación entre nutricionista y paciente.
* Metadatos del foro.

## Tecnologías propuestas

* C#.
* .NET.
* ASP.NET Core Web API.
* Entity Framework Core.
* SQL Server.
* MongoDB.
* React o Angular.
* Bootstrap.
* HTML5.
* CSS.
* JavaScript / TypeScript.
* Docker.
* Azure o AWS.
* GitHub Actions.

## Requerimientos de base de datos

El proyecto debe incluir:

* Base de datos relacional en SQL Server.
* Base de datos no relacional en MongoDB.
* Mínimo cuatro procedimientos almacenados.
* Mínimo dos triggers.
* Mínimo tres vistas.
* Scripts de creación y población de base de datos.
* Separación clara entre lógica relacional y lógica documental.

## Documentación requerida

El repositorio debe incluir documentación técnica y funcional:

* Manual de usuario.
* Documento de instalación.
* Documento de evidencia de la solución.
* Plan de proyecto.
* Minutas.
* Modelo conceptual.
* Modelo relacional.
* Descripción de tablas.
* Descripción de procedimientos almacenados.
* Descripción de triggers.
* Descripción de vistas.
* Descripción de arquitectura.
* Problemas encontrados.
* Problemas conocidos.
* Conclusiones.
* Recomendaciones.

## Ejecución local

### Backend SQL Server

```bash
cd src/backend
dotnet restore
dotnet build
dotnet run --project NutriTEC.Api
```

### Backend MongoDB

```bash
cd src/backend
dotnet run --project NutriTEC.MongoApi
```

### Frontend

```bash
cd src/frontend/nutritec-web
npm install
npm run dev
```

### App móvil

```bash
cd src/mobile/nutritec-mobile
npm install
npm start
```

## Estado del proyecto

Proyecto en fase inicial de diseño, planificación y creación de estructura base del repositorio.
