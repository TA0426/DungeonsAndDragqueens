# API-kontrakt

## Opprett karakter

POST /StoreCharacters

Request:

{
"name": "Anna"
}

Response:

- 201 Created
- Returnerer den opprettede karakteren

## Hent alle karakterer

GET /StoreCharacters

Response:

- 200 OK
- Returnerer en liste med karakterer

## Opprett monster

POST /StoreMonsters

Request:

{
"name": "Goblin",
"quantity": 1,
"typeOfMonster": "Goblin",
"hp": 10,
"damage": 1,
"xpReward": 10,
"description": "Et svakt monster"
}

Response:

- 201 Created
- Returnerer det opprettede monsteret

## Hent alle monstre

GET /StoreMonsters

Response:

- 200 OK
- Returnerer en liste med monstre

## Lagre og hente sverd

GET /StoreSwords

Response:

- 200 OK
- Returnerer liste med sverd
- 404 NotFound
- Ingen sverd funnet

Post /StoreSwords

Request:
{
Name = dto.Name,
Damage = dto.Damage,
Description = dto.Description
}

Response:

- 201 created
- Returnerer det opprettede sverdet

## Start kamp

POST /StartFight

Request:

{
"characterId": "character-guid",
"monsterId": "monster-guid"
}

Response:

- 200 OK
- Returnerer resultatet fra kampen

Eksempel på response:

{
"characterName": "Anna",
"monsterName": "Goblin",
"characterHpAfterFight": 299,
"monsterHpAfterFight": 0,
"characterWon": true,
"enemiesBeforeFight": 1,
"enemiesAfterFight": 0
}

## Utstyr karakter med sverd

PUT /StoreCharacters/{characterId}/equipment/{swordId}

Response:

- 200 OK
- 404 Not Found dersom karakteren eller sverdet ikke finnes

## Hvil

POST /StoreCharacters/{characterId}/camp

Response:

- 200 OK dersom karakteren kan hvile
- 400 Bad Request dersom det ikke har gått nok runder
- 404 Not Found dersom karakteren ikke finnes

## Slett monster

DELETE /StoreMonsters/{id}

Response:

- 204 No Content
- 404 Not Found dersom monsteret ikke finnes

## Slett sverd

DELETE /StoreSwords/{id}

Response:

- 204 No Content
- 404 Not Found dersom sverdet ikke finnes
