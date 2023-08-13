using Core.Exceptions;
using Core.Shared;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;

namespace WebApi.Filters;

public class ExceptionHandlingFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var statusCode = GetHttpStatusCode(context.Exception);
        int statusCodeAsInt = (int)statusCode;

        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.StatusCode = statusCodeAsInt;

        var problemDetails = new ProblemDetails
        {
            Type = GetRFCType(statusCode),
            Title = context.Exception.Message,
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

    private static HttpStatusCode GetHttpStatusCode(Exception exception)
    {
        return exception switch
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
    }

    private static string GetRFCType(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => RFCType.BadRequest,
            HttpStatusCode.Forbidden => RFCType.Forbidden,
            HttpStatusCode.NotFound => RFCType.NotFound,
            _ => RFCType.InternalServerError
        };
    }
}
