using Core.Exceptions;
using Core.Shared;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;
using System.Text.Json;

namespace WebApi.Middleware;

public class ExceptionsMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = GetHttpStatusCode(exception);
        int statusCodeAsInt = (int)statusCode;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCodeAsInt;

        var problemDetails = new ProblemDetails()
        {
            Type = GetRFCType(statusCode),
            Title = exception.Message,
            Status = statusCodeAsInt,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        var json = JsonSerializer.Serialize(problemDetails);

        return context.Response.WriteAsync(json);
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