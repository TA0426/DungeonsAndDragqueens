# Dungeons & Dragqueens

A browser-based, text-first RPG backed by ASP.NET Core, PostgreSQL, and a small JavaScript client. Players can create an account, build characters, fight monsters, collect XP, equip swords, and rest at camp.

## Run with Docker Compose

Start the browser game, API, and PostgreSQL:

## In Docker terminal:

```
cd "folder-for-git-repo"
then run
docker compose up --build
then open probably: http://localhost:8080
```

Open [http://localhost:8080](http://localhost:8080). PostgreSQL data is kept in the `postgres_data` volume. `docker compose down` keeps that data; `docker compose down -v` deletes it.

The Compose file contains development credentials and a development-only JWT key. Replace them before using this setup outside your own development machine.

## Run locally

Start PostgreSQL first, then run the API from the repository root:

```powershell
docker compose up -d db
dotnet run --project .\DungeonsAndDragqueens.csproj
```

Open [http://localhost:5209](http://localhost:5209). The app applies EF Core migrations and adds starter monsters and swords if those catalogs are empty. Local PostgreSQL uses `Host=localhost`; the API container uses `Host=db`.

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
