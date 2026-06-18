# NutriTEC

NutriTEC es un proyecto academico de bases de datos para preparar una plataforma nutricional con persistencia relacional y no relacional.

## Arquitectura

El repositorio queda organizado por capas y superficies de ejecucion:

- `src/backend`: solucion .NET con una API para SQL Server y una API separada para MongoDB.
- `src/frontend`: aplicacion web Vite con React y TypeScript.
- `src/mobile`: placeholder inicial para la futura aplicacion movil.
- `database`: carpetas para scripts de SQL Server y MongoDB.
- `docs`: documentacion tecnica, diagramas, manuales y minutas.
- `deployment`: archivos de despliegue futuros.
- `tests`: proyectos y carpetas de pruebas.

## Tecnologias principales

- C# y ASP.NET Core
- SQL Server
- MongoDB
- React
- TypeScript
- Vite
- Node.js

## Backend

Restaurar y compilar la solucion:

```bash
dotnet restore src/backend/NutriTEC.sln /p:RestoreUseStaticGraphEvaluation=true
dotnet build src/backend/NutriTEC.sln --no-restore /m:1
```

Ejecutar la API principal para SQL Server:

```bash
dotnet run --project src/backend/NutriTEC.Api/NutriTEC.Api.csproj
```

Ejecutar la API separada para MongoDB:

```bash
dotnet run --project src/backend/NutriTEC.MongoApi/NutriTEC.MongoApi.csproj
```

Ambas APIs incluyen endpoints de salud y placeholders OpenAPI. La configuracion real de bases de datos se agregara cuando existan los modelos y casos de uso.

## Frontend

```bash
cd src/frontend/nutritec-web
npm install
npm run dev
```

## Mobile

```bash
cd src/mobile/nutritec-mobile
npm run start
```

## Estado actual

Esta es una estructura inicial lista para empezar. La logica de negocio, entidades, DTOs, servicios, repositorios, controladores de funcionalidades y scripts reales de base de datos se implementaran despues.
