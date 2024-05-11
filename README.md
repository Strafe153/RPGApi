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
* `Asp.Versioning.Mvc.ApiExplorer` for API versioning
* `AspNetCore.HealthChecks.NpgSql` for PostgreSQL health check
* `AspNetCore.HealthChecks.Redis` for Redis health checks
* `AspNetCore.HealthChecks.UI.Client` for detailed health checks information
* `AutoFixture` for test fixtures
* `AutoFixture.AutoNSubstitute` for NSubstitute support with AutoFixture
* `Bogus` for fake data generation
* `Dapper` for object-relational mapping
* `FluentAssertions` for assertions
* `Microsoft.AspNetCore.Authentication.JwtBearer` for JWT authentication
* `Microsoft.AspNetCore.Http` for default HTTP feature implementations
* `Microsoft.AspNetCore.Http.Abstractions` for object model for HTTP requests and responses
* `Microsoft.AspNetCore.Mvc.NewtonsoftJson` for MVC features that use Newtonsoft.Json
* `Microsoft.Extensions.Caching.Abstractions` for caching abstractions
* `Microsoft.Extensions.Configuration` for key-value based configuration
* `Microsoft.Extensions.Configuration.Abstractions` for configuration abstractions
* `Microsoft.Extensions.Caching.StackExchangeRedis` for Redis distributed cache implementation
* `Microsoft.Extensions.DependencyInjection.Abstractions` for DI abstractions
* `Microsoft.Extensions.Identity.Core` for identity membership system
* `Microsoft.Extensions.Logging.Abstractions` for logging abstractions
* `Microsoft.IdentityModel.Tokens` for security tokens
* `Microsoft.NET.Test.Sdk` for .NET SDK for testing
* `Newtonsoft.Json` for JSON serialization
* `NLog` for logging
* `NLog.Web.AspNetCore` for ASP.NET Core logging
* `Npgsql` for PostgreSQL
* `Npgsql.EntityFrameworkCore.PostgreSQL` for PostgreSQL exception
* `NSubstitute` for mocking
* `NUnit` for unit-tests
* `NUnit3TestAdapter` for adapting tests in Visual Studio
* `Swashbuckle.AspNetCore` for Swagger support
* `System.IdentityModel.Tokens.Jwt` for JWT

## How To Use?
1. Register as a new player
2. Log In
3. Create a character
4. Try things out
