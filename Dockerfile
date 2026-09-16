# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Kopier hele solution
COPY . .

# Gå inn i API-prosjektet


# Restore + publish
RUN dotnet restore "OppgaveUkeEnModul3.csproj"
RUN dotnet publish "OppgaveUkeEnModul3.csproj" -c Release -o /app/publish --no-restore

# ---------- RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OppgaveUkeEnModul3.dll"]
