

# C# Advanced - Arkitekturreview



### Del 1 – Kartlegg systemet


Prosjektet mitt er et backend-system for et simpelt rollespill. Brukeren kan opprette karakterer, hente/lage monstre og sverd, starte kamper, få XP, gå opp i level, utstyre sverd og hvile ved camp.

Systemet består av:

- `ConsoleApp`: Klienten brukeren skriver kommandoer i
- ASP.NET Core Web API: Tar imot HTTP-requests
- Controllers: Håndterer HTTP-endepunkter
- Services: Inneholder spillogikk og databaseoperasjoner
- Entity Framework Core: Kommuniserer med databasen
- PostgreSQL: Lagrer karakterer, monstre, sverd og kampdata
- JWT: Identifiserer innlogget bruker

### Arkitekturdiagram

$User(Bruker) --> Console (ConsoleApp)$

$Console --> HTTP POST /auth/login, Auth (AuthController)$
$Auth --> JWT-token, Console$

$Console --> Bearer token + HTTP requests, API (ASP.NET Core Web API)$


$API --> Characters (StoreCharactersController)$
$API --> Monsters (StoreMonstersController)$
$API --> Swords (StoreSwordsController)$
$API --> Fight (FightController)$

$Characters --> CharacterService (CharacterService)$
$Monsters --> MonsterService (StoreMonstersService)$
$Swords --> SwordService (SwordService)$
$Fight --> GameService (GameService)$

$CharacterService --> EF (Entity Framework Core)$
$MonsterService --> EF$
$SwordService --> EF$
$GameService --> EF$

$EF --> Database (PostgreSQL)$


### Hvor kjører komponentene?

Lokalt kjører ConsoleApp og API-et som to separate .NET-applikasjoner.

API-et kjører på en lokal adresse, for eksempel `http://localhost:5209`

ConsoleApp sender HTTP-requests til API-et. API-et kommuniserer videre med PostgreSQL-databasen gjennom Entity Framework Core.

I deployment kan API-et kjøres i Docker på en server. `docker-compose.yaml`, `Dockerfile` og Terraform-konfigurasjonen kan brukes til å sette opp infrastrukturen. 

### Hvordan kommuniserer komponentene?

ConsoleApp kommuniserer med API-et gjennom HTTP og JSON.

Eksempel: 

`POST /auth/login`
`POST /StoreCharacters`
`GET /StoreMonsters`
`POST /Fight`


Når brukeren logger inn, returnerer API-et et JWT-token. ConsoleApp sender deretter tokenet i HTTP-headeren:

`Authorization: Bearer <jwt-token>`


### Hvor lagres data?

Data lagres i PostgreSQL-databasen.

Databasen inneholder blant annet: 
- Karakterer
- Monstre
- Sverd
- Fremgang mot monstre
- Bruker-ID på karakterer

Entity Framework Core brukes som ORM mellom C#-kodene og databasen.

### Hvordan identifiseres brukeren?

Brukeren logger inn med brukernavn og passord på:

`POST /auth/login`

API-et lager et JWT-token med claims. Tokenet inneholder blant annet brukerens identitet.

Når brukeren gjør et nytt API-kall, leser API-et bruker-ID-en fra JWT-tokenet. Character-data filtreres på denne bruker-ID-en, slik at brukeren bare får tilgang til sine egne karakterer.

### Hvilke komponenter er avhengige av hverandre?
- ConsoleApp er avhengig av at API-et kjører
- Controllers er avhengige av services
- Services er avhengige av database-contexten
- Database-contexten er avhengig av PostgreSQL
- Beskyttede API-endepunkter er avhengige av gyldig JWT-token
- GameService er avhengig av FightService og databasen. 


### Del 2 – Finn svakhetene

#### Svakhet 1: Sensitiv informasjon ligger i konfigurasjonsfiler

Problem:

Connection stringen inneholder databasebruker og passord:

`"DefaultConnection": "Host=localhost;Port=5432;Database=database;Username=postgres;Password=postgres"`

JWT-nøkkelen kan også ligge i appsettings.Development.json

Konsekvens:

Hvis konfigurasjonsfilen committes til GitHub, kan andre få tilgang til databasen eller bruke JWT-nøkkelen til å lage falske tokens. 

Løsning:

Jeg ville flyttet sensitive verdier til environment variables eller et secret management-system

Lokalt kan connection stringen for eksempel settes med:

`$env:ConnectionStrings__DefaultConnection = "..."`
`$env:Jwt__Key = "..."`

#### Svakhet 2: Controllerne inneholder for mye logikk

Problem:

I den opprinnelige løsningen gjorde controllerne flere ting samtidig:
- Leste JWT-token manuelt
- Hentet bruker-ID
- Gjorde databasekall
- Utførte spilleregler
- Oppdaterte data
- Returnerte HTTP-respons

For eksempel lå camp-logikken direkte i `StoreCharactersController`

Konsekvens:

Controllerne ble store og vanskelige å teste. Endringer i spillereglene måtte gjøres direkte i controlleren. Det ble også lett å kopiere samme kode flere steder

Løsning:

Jeg flyttet logikk til services:

`Controller --> Service --> Database`


Eksempler på services er:
- `CharacterService`
- `SwordService`
- `StoreMonstersService`
- `GameService`

Controllerne skal nå hovedsakelig:
1. Motta request
2. Hente bruker-ID
3. Kalle riktig service
4. Returnere en HTTP-respons


#### Svakhet 3: JWT-bruker-ID ble hentet manuelt og kopiert

Problem:

JWT-tokenet ble tidligere lest manuelt i flere controllere med kode som dette:

`var authorization = Request.Headers.Authorization.ToString();`
`var token = authorization["Bearer ".Length..].Trim();`

`var handler = new JwtSecurityTokenHandler();`
`var jwt = handler.ReadJwtToken(token);`

Den samme logikken lå flere steder

Konsekvens:
- Samme kode måtte vedlikeholdes flere steder
- Det var mulig at controllerne behandlet tokens forskjellig
- `ReadJwtToken()` leser tokenet, men validerer ikke nødvendigvis signaturen
- Koden ble mer komplisert enn nødvendig

Løsning:

Jeg satte opp JWT Bearer Authentication i ASP.NET Core og brukte `[Authorize]`

Bruker-ID kan deretter hentes fra den allerede validerte brukeren:

`var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);`

Jeg samlet også bruker-ID-oppslag i en felles extension-metode



#### Svakhet 4: Ikke alle endepunkter var beskyttet mot uautoriserte brukere

Problem: 

Character-endepunktene sjekket bruker-ID, men monster- og sword-endepunktene hadde ikke nødvendigvis samme beskyttelse.

Det kunne derfor være mulig for en bruker å opprette eller slette felles spilldata.

Konsekvens:

En vanlig bruker kunne potensielt endre data som burde være beskyttet eller administrert.

Løsning:

Jeg bruker `[Authorize]` på endepunkter som krever innlogging:

`[Authorize]`
`[HttpPost]`
`public async Task<IActionResult> Post(CreateSwordDTO dto)`
`{`
    `...`
`}`

Senere kan skriveoperasjoner begrenses til administratorer med roller: 

`[Authorize(Roles = "Admin")]`



#### Svakhet 5: Feilhåndteringen var ikke konsekvent

Problem: 

Noen metoder returnerte forklarende feilmeldinger:

`return NotFound("Character not found.");`

Andre returnerte bare:

`return NotFound();`


I tillegg kunne exceptions fra service-laget bli til `500 Internal Server Error`, selv om feilen egentlig skyldtes ugyldige brukerdata.

Eksempel:

`throw new ArgumentException(`
    `"Page must be greater than 0.");`

Konsekvens:
- Klienten fikk ikke alltid vite hva som var galt
- Feil ble vanskeligere å feilsøke
- API-et ga ikke alltid konsekvente svar

Løsning:

Jeg sjekker HTTP-statusen i ConsoleApp før jeg leser responsen som vellykket data.

Eksempel:

`if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(
        $"Feil {(int)response.StatusCode}: " +
        await response.Content.ReadAsStringAsync());

    continue;
}`

Jeg bruker også tydelige statuskoder:
- `400 Bad Request` ved ugyldige data
- `401 Unauthorized` uten gyldig token
- `403 Forbidden` uten riktige rettigheter
- `404 Not Found` når data ikke finnes
- `500 Internal Server Error` ved uventede feil


## Del 3 – Arkitekturbeslutning

Jeg valgte å dokumentere beslutningen om å flytte logikk fra controllere til services.

Problem:

Controllerne inneholdt både HTTP-logikk, databasekall, autentiseringslogikk og spilleregler.

For eksempel håndterte `StoreCharactersController` både henting av karakterer, opprettelse av karakterer, camp-regler og oppdatering av databasen.

Dette gjorde controllerne store og vanskeligere å teste og vedlikeholde.

Alternativene:

Alternativ A: Beholde logikken i controllerne

Fordeler:
- Krever minst mulig kodeendring
- Enkelt å følge i et lite prosjekt
- Ingen nye service-klasser trengs

Ulemper:
- Controllerne blir store
- Databasekall og spilleregler blir blandet med HTTP-logikk
- Vanskeligere å teste
- Samme logikk kan bli kopiert flere steder

Kompleksitet:

Lav i starten, men øker når systemet vokser

Vedlikehold:

Dårligere fordi flere typer ansvar ligger i samme klasse.


Alternativ B: Flytte logikk til services

Fordeler:
- Controllerne blir enklere
- Spilleregler kan testes separat
- Databasekall samles i services
- Mindre risiko for duplisert kode
- Tydeligere oppdeling av ansvar

Ulemper:
- Krever flere filer
- Litt mer oppsett i starten
- Man må følge dataflyten mellom controller og service

Kompleksitet:
- Middels?

Vedlikehold:

Bedre fordi hver klasse får et tydeligere ansvar.

Valg:

Jeg valgte alternativ B.

Jeg oppretter eller brukte blant annet:
- 'CharacterService'
- 'SwordService'
- `StoreMonstersService`
- `GameService`

Controllerne brukes nå hovedsakelig som HTTP-lag.


Hvorfor?

Jeg valgte denne løsningen fordi systemet allerede har flere typer funksjonalitet. Når karakterer, sverd, monstre og kamper får mer logikk, blir det vanskelig å vedlikeholde alt i controllerne.

Service-laget gjør det enklere å:
- Gjenbruke logikk
- Skrive tester
- Endre spilleregler
- Holde controllerne små
- Skille HTTP fra database og domenelogikk

Løsningen er mer strukturert, men fortsatt enkel nok for prosjektets størrelse.


Konsekvenser:

Positive konsekvenser:
- Controllerne er lettere å lese
- Databasekall ligger i services
- Spillogikk kan testes mer isolert
- Ansvarsfordelingen er tydeligere
- Det blir mindre kopiert kode

Negative Konsekvenser:
- Prosjektet får flere klasser og filer
- Det tar litt lenger tid å finne hvor logikken ligger
- Det finnes mer struktur å forstå

For dette prosjektet mener jeg fordelene er større enn ulempene.

### Del 4 – Implementer forbedringen

Jeg har implementert flere arkitektoniske forbedringer.

JWT-innlogging:

Jeg laget et login-endepunkt:

`POST /auth/login`

ConsoleApp sender brukernavn og passord:

`{
  "username": "player",
  "password": "password123"
}`

API-et returnerer er JWT-token. ConsoleApp lagrer tokenet i `HttpClient`:

`client.DefaultRequestHeaders.Authorization =`
    `new AuthenticationHeaderValue("Bearer", login.Token);`

Alle videre API-kall sender derfor tokenet automatisk.

Beskyttede endepunkter

Jeg bruker JWT Bearer Authentication i API-et og `[Authorize]` på beskyttede controllere.

Eksempel: 

`[Authorize]`
`public class StoreCharactersController : ControllerBase`
`{`
`}`

Character-data filtreres på bruker-ID-en fra JWT-tokenet.

Bedre oppdeling

Jeg flyttet logikk ut av controllerne og inn i services.

Den nye strukturen er:

`StoreCharactersController`
    `↓`
`CharacterService`
    `↓`
`StoreMonstersContext`
    `↓`
`PostgreSQL`

Det samme mønsteret brukes for sverd, monstre og kamper.

Bedre feilhåndtering i ConsoleApp

ConsoleApp sjekker nå om API-kallet var vellykket før responsen leses som data.

Eksempel:

`if (!response.IsSuccessStatusCode)`
`{`
    `Console.WriteLine(`
        `$"Feil {(int)response.StatusCode}: " +`
        `await response.Content.ReadAsStringAsync());`

    `continue;`
`}`

Dette gjør at brukeren kan se om feilen skyldes manglende innlogging, ugyldige data eller at data ikke finnes.

Hva som kan forbedres senere

Det finnes fortsatt forbedringer som kan gjøres:
- Lagre brukere i databasen i stedet for en midlertidig testbruker
- Hashe passord før de lagres
- Flytte connection string og JWT-nøkkel til environment variables
- Bruke response-DTO-er konsekvent i alle API-endepunkter
- Lage en felles global exception handler
- Legge til rollebasert authorization
- Lage flere tester for service-laget

Konklusjon

Systemet fungerer, men arkitektur-reviewen viste at enkelte deler av tett koblet sammen. Den viktigste forbedringen var å skille controller-, service- og databaselogikk.

Dette gjør systemet lettere å forstå, teste og videreutvikle. JWT-innlogging og bedre feilhåndtering gjør også kommunikasjonen mellom ConsoleApp og API-et tydeligere. 



