
# Calendar App

This is a calendar application built using React + .NET minimal application

## Tech Stack

**Client:** React, Zustand, Mui

**Server:** .Net , EntityFramwork, PostgreSql


## Requirements

.Net 8 SDK - https://dotnet.microsoft.com/en-us/download/dotnet/8.0

NodeJS - https://nodejs.org/en/download

Postgres (if running Locally) - https://www.postgresql.org/download/

    
## Run Locally

Clone the project

```bash
  git clone https://github.com/zaneris123/CalendarApp.git
```

Go to the server folder in the project

```bash
  cd CalendarApp/CalendarApp.Server
```

setup connection string

```bash
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=CalendarApp;Username=postgres;Password=*password*"
```

Migrate postgres Database with ef

```bash
  dotnet ef migrations add InitialCreate
  dotnet ef database update
```


Start the server

```bash
  dotnet run
```

