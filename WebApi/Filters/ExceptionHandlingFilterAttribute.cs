using Domain.Exceptions;
using Domain.Shared;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;
using System.Net.Mime;

namespace WebApi.Filters;

public class ExceptionHandlingFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var statusCode = GetHttpStatusCode(context.Exception);
        var statusCodeAsInt = (int)statusCode;

        context.HttpContext.Response.ContentType = MediaTypeNames.Application.Json;
        context.HttpContext.Response.StatusCode = statusCodeAsInt;

        var rfcType = GetRFCType(statusCode);

        ProblemDetails problemDetails = new()
        {
            Type = rfcType,
            Status = statusCodeAsInt,
            Detail = context.Exception.Message,
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new JsonResult(problemDetails)
        {
            StatusCode = statusCodeAsInt
        };

        base.OnException(context);
    }

    private static HttpStatusCode GetHttpStatusCode(Exception exception) => exception switch
    {
        NullReferenceException
            or ItemNotFoundException => HttpStatusCode.NotFound,
        NotEnoughRightsException
            or InvalidTokenException => HttpStatusCode.Forbidden,
        IncorrectPasswordException
            or NameNotUniqueException
            or OperationCanceledException
            or PostgresException => HttpStatusCode.BadRequest,
        _ => HttpStatusCode.InternalServerError
    };

    private static string GetRFCType(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.BadRequest => RFCType.BadRequest,
        HttpStatusCode.Forbidden => RFCType.Forbidden,
        HttpStatusCode.NotFound => RFCType.NotFound,
        _ => RFCType.InternalServerError
    };
}
