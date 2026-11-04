# Dungeons & Dragqueens

A browser-based, text-first RPG backed by ASP.NET Core, PostgreSQL, and a small JavaScript client. Players can create an account, build characters, fight monsters, collect XP, equip weapons, and rest at camp.

## Run with Docker Compose

This starts PostgreSQL, the API, and the browser game. Install Docker Desktop and start it first, then run these commands from the repository root:

```powershell
docker compose up --build
```

Open [http://localhost:8080](http://localhost:8080). Press `Ctrl+C` to stop. PostgreSQL data stays in the `postgres_data` volume when you run `docker compose down`; `docker compose down -v` also deletes the database volume.

The Compose file currently supplies development credentials directly to its containers. This is only suitable for local development; do not reuse those values in a deployed environment.

## Run locally with .NET

This runs the API on your computer and PostgreSQL in Docker. It requires the .NET 10 SDK and Docker Desktop. Run commands from the repository root.

The project includes a `UserSecretsId` in `DungeonsAndDragqueens.csproj`. That ID must be committed, but each developer sets their own local values. Set the connection string to match the local PostgreSQL credentials configured for the Compose `db` service. Use `Host=localhost` when connecting from the API running on your computer:

```powershell
docker compose up -d db
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=database;Username=postgres;Password=YOUR_LOCAL_POSTGRES_PASSWORD"
```

Create a private JWT signing key on your computer and save it to User Secrets:

```powershell
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$bytes = New-Object byte[] 48
$rng.GetBytes($bytes)
$jwtKey = [Convert]::ToBase64String($bytes)
$rng.Dispose()
dotnet user-secrets set "Jwt:Key" $jwtKey
```

Replace `YOUR_LOCAL_POSTGRES_PASSWORD` with the password configured for your local PostgreSQL service. Do not commit your actual connection string or JWT key. Then start the API:

```powershell
dotnet run --project .\DungeonsAndDragqueens.csproj
```

Open [http://localhost:5209](http://localhost:5209). The app applies EF Core migrations and seeds starter game data on startup. For the local API, PostgreSQL uses `Host=localhost`; inside Docker Compose, the API connects to PostgreSQL using `Host=db`.

## Accounts

Create an account in the browser or call `POST /auth/register`. Usernames are 3–32 letters, digits, underscores, or hyphens. Passwords must be at least 12 characters. Passwords are stored as hashes. Login returns a short-lived JWT, and characters are scoped to the account that created them.

The browser keeps the token in `sessionStorage` for the current tab session. Log out to remove it. Use HTTPS and a private signing key outside development.

## Music

Put your own `.mp3`, `.ogg`, or `.wav` files in `wwwroot/audio`, then list them in `wwwroot/audio/tracks.json`. The browser waits for the player to press Play before starting audio.

## Projects

- `DungeonsAndDragqueens.csproj` — API and browser host
- `DungeonsAndDragqueens.Tests/` — automated tests
- `ConsoleApp/` — optional command-line client
- `Core/` — game models, DTOs, and combat rules
- `WebApi/` — controllers, database context, and application services
- `Migrations/` — PostgreSQL schema history
- `DungeonsAndDragqueens.Infrastructure/` — Terraform deployment files

API routes and example requests are in [api.md](api.md); the HTTP scratch file is [DungeonsAndDragqueens.http](DungeonsAndDragqueens.http).
