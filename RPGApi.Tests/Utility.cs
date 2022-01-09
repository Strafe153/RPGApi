global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.JsonPatch;
global using RPGApi.Dtos;
global using RPGApi.Models;
global using RPGApi.Controllers;
global using RPGApi.Repositories;
global using Moq;
global using Xunit;
global using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
    }
}
