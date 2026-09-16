# XP og Level REST API

Dette prosjektet er laget som en TDD-oppgave

Systemet lar brukeren:

- Opprette karakterer
- Opprette monstre
- Starte kamper
- Få XP etter vunnet kamp
- Gå opp i level
- Equipe sverd
- Hvile ved camp

## Kjøre prosjektet

Kjør API-et med:

dotnet run

API-et starter med SQLite-databasen `monsters.db`.

## Kjøre testene

Kjør alle tester med:

dotnet test

## TDD

Noe av domenelogikken ble utviklet med Red-Green-Refactor:

- Jeg startet først med å skrive tester, men etterhvert som jeg starter å kode ble jeg oppslukt i å gjøre spillet mer og mer avansert og glemte helt ut at jeg skulle gjøre TDD^^
- Etter jeg innså dette, fjernet jeg all kode og startet opp et nytt prosjekt, og kjørte deretter testene en og en, for så å legge inn koden jeg trengte fra det gamle prosjektet for å få testene til å bli grønne. Håper det går greit at jeg løste det slik. Skal ikke glemme meg ut neste gang...

## Dokumentasjon

API-kontrakten finnes i `api.md`.

MVP-skissen finnes i `skisse til programflyt.excalidraw`.
