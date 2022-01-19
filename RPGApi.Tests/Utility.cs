global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.JsonPatch;
global using RPGApi.Dtos;
global using RPGApi.Dtos.Spells;
global using RPGApi.Dtos.Mounts;
global using RPGApi.Dtos.Players;
global using RPGApi.Dtos.Weapons;
global using RPGApi.Dtos.Characters;
global using RPGApi.Models;
global using RPGApi.Controllers;
global using RPGApi.Repositories;
global using Moq;
global using Xunit;
global using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Security.Claims;

namespace RPGApi.Tests
{
    internal static class Utility
    {
        internal static void MockObjectModelValidator(ControllerBase controller)
        {
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(), It.IsAny<string>(), It.IsAny<Object>()));

            controller.ObjectValidator = objectValidator.Object;
        }

        internal static void MockUserIdentityName(ControllerBase controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ClaimTypes.Name, "identity_name") }, "mock"));

            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
        }
    }
}
