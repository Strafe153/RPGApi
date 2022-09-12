# RPG Api

## About the project 
The api is the implementation of a typical RPG game. Data operations are performed via Dtos and custom mapping. JWT-token authorization is present. Data operations are documented by NLog.

* Authentication and authorization
* Players management
* Characters administration
* Weapons manipulation
* Spells operations
* Work with mounts

## Dependencies
* `FluentValidation` for DTO validation
* `NLog` for logging
* `NUnit` for unit-tests
* `NSubstitute` for mocking
* `AutoFixture` for test fixtures
* `FluentAssertions` for assertions

## Endpoints
### Players
* `GET api/players`
* `GET api/players/{id}`
* `POST api/players/register`
* `POST api/players/login`
* `PUT api/players/{id}`
* `PUT api/players/{id}/changePassword`
* `PUT api/players/{id}/changeRole`
* `DELETE api/players/{id}`

### Characters
* `GET api/characters`
* `GET api/characters/{id}`
* `POST api/characters`
* `PUT api/characters/{id}`
* `PUT api/characters/add/weapon`
* `PUT api/characters/remove/weapon`
* `PUT api/characters/add/spell`
* `PUT api/characters/remove/spell`
* `PUT api/characters/add/mount`
* `PUT api/characters/remove/mount`
* `PATCH api/characters/{id}`
* `DELETE api/characters/{id}`

### Weapons
* `GET api/weapons`
* `GET api/weapons/{id}`
* `POST api/weapons`
* `PUT api/weapons/{id}`
* `PUT api/weapons/hit`
* `PATCH api/weapons/{id}`
* `DELETE api/weapons/{id}`

### Spells
* `GET api/spells`
* `GET api/spells/{id}`
* `POST api/spells`
* `PUT api/spells/{id}`
* `PUT api/spells/hit`
* `PATCH api/spells/{id}`
* `DELETE api/spells/{id}`

### Mounts
* `GET api/mounts`
* `GET api/mounts/{id}`
* `POST api/mounts`
* `PUT api/mounts/{id}`
* `PATCH api/mounts/{id}`
* `DELETE api/mounts/{id}`

## How To Use?
1. Register as a new player
2. Log in
3. Create a character

**NOTE!** To see advanced features, log in with "admin" as username and "qwerty" as password.
