TuAsador
========

Plataforma de conexion entre asadores y clientes.


Stack
-----

- Backend: .NET 9 ASP.NET Core Web API + Entity Framework Core + SQL Server
- Frontend: Vue 3 + Vite + TypeScript + Vue Router + Axios


Requisitos
----------

- .NET 9 SDK (https://dotnet.microsoft.com/download/dotnet/9.0)
- Node.js (https://nodejs.org/) (v18 o superior)
- SQL Server (local o remoto)


Configuracion inicial
---------------------

1. Base de datos

  Opcion A - Scripts SQL manuales

  Ejecutar los scripts en orden desde SQL Server Management Studio (SSMS) o sqlcmd:

    sqlcmd -S YOUR_SERVER -d master -i database\schema.sql
    sqlcmd -S YOUR_SERVER -d TuAsadorDev -i database\seed.sql

  - database/schema.sql -> crea la base de datos y todas las tablas
  - database/seed.sql -> inserta datos de prueba (5 usuarios, 2 asadores, 3 eventos, etc.)

  La contrasena de todos los usuarios de prueba es: TuAsador123!

  Opcion B - Auto-migracion con EF Core (recomendada)

  Configurar la connection string en src/TuAsador.Api/appsettings.json:

    "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER;Database=TuAsadorDev;Trusted_Connection=True;TrustServerCertificate=True"
    }

2. Backend

    cd src/TuAsador.Api
    dotnet restore
    dotnet run

  La API se levanta en http://localhost:5254.

  La base de datos se crea con migraciones de EF Core y los datos de prueba se
  insertan automaticamente al iniciar (solo la primera vez).

3. Frontend

    cd web
    npm install
    npm run dev

  La web se levanta en http://localhost:5173. Las llamadas a /api y /uploads se
  redirigen al backend automaticamente.

4. Compilar frontend para produccion

    cd web
    npm run build

  Los archivos compilados se generan en web/dist/.
