# RPG Api
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

## How To Use?
1. Register as a new player
2. Log in
3. Create a character
4. Try things out

**NOTE!** To see advanced features, log in with "admin" as username and "qwerty" as password.
