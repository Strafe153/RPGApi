# RPGApi

### The API is the implementation of a typical RPG game with player accounts, characters, items such as weapons, spells and mounts.

## What Can It Do?
* Authentication and authorization
* Players management
* Characters administration
* Weapons manipulation
* Work with spells 
* Mounts creation, editing and deletion

## Dependencies
* `Npgsql.EntityFrameworkCore.PostgreSQL` for PostgreSQL
* `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` for API versioning
* `Dapper` for object-relational mapping
* `FluentValidation` for DTO validation
* `AspNetCore.HealthChecks.UI.Client` for detailed health checks information
* `AspNetCore.HealthChecks.NpgSql` for PostgreSQL health check
* `AspNetCore.HealthChecks.Redis` for Redis health checks
* `NLog` for logging
* `NUnit` for unit-tests
* `NSubstitute` for mocking
* `AutoFixture` for test fixtures
* `FluentAssertions` for assertions
* `Bogus` for fake data generation

## How To Use?
1. Register as a new player
2. Log In
3. Create a character
4. Try things out

> **NOTE!** To see advanced features, log in with "admin" as username and "qwerty" as password.
