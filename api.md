# API contract

The browser client is served by the API at `/`. Protected routes require `Authorization: Bearer <token>`.

## Accounts

### Register

`POST /auth/register`

```json
{
	"userName": "example_player",
	"password": "a-password-with-5-chars"
}
```

Returns `200 OK` and a JWT. Usernames must be 3–32 letters, digits, `_` or `-`; passwords must be 5–128 characters. Duplicate usernames return `409 Conflict`.

### Log in

`POST /auth/login` with the same JSON shape. Returns a JWT or `401 Unauthorized`.

## Game

All routes below require a valid JWT.

| Method   | Route                                                | Purpose                                  |
| -------- | ---------------------------------------------------- | ---------------------------------------- |
| `GET`    | `/StoreCharacters`                                   | List the signed-in player's characters   |
| `GET`    | `/StoreCharacters/{id}`                              | Get one of the player's characters       |
| `POST`   | `/StoreCharacters`                                   | Create a character: `{ "name": "Anna" }` |
| `PUT`    | `/StoreCharacters/{characterId}/equipment/{swordId}` | Equip a sword                            |
| `POST`   | `/StoreCharacters/{id}/camp`                         | Rest after the required rounds           |
| `GET`    | `/StoreMonsters?page=1&pageSize=100`                 | Browse monsters                          |
| `GET`    | `/StoreSwords`                                       | Browse swords                            |
| `POST`   | `/Fight`                                             | Fight a monster                          |
| `POST`   | `/StoreMonsters`                                     | Add a monster to the shared catalog      |
| `DELETE` | `/StoreMonsters/{id}`                                | Remove a monster from the shared catalog |
| `POST`   | `/StoreSwords`                                       | Add a sword to the shared catalog        |
| `DELETE` | `/StoreSwords/{id}`                                  | Remove a sword from the shared catalog   |

Fight request:

```json
{
	"characterId": "character-guid",
	"monsterId": "monster-guid"
}
```

Character routes only return or change characters owned by the JWT's user ID. Monsters and swords are shared game catalog entries.
