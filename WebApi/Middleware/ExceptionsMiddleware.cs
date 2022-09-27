using Core.Exceptions;
using Core.Models;
using System.Net;

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
        catch (NullReferenceException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.NotFound,
                $"{ex.Message}. Path:{context.Request.Path}.");
        }
        catch (ItemNotFoundException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.NotFound,
                $"{ex.Message}. Path:{context.Request.Path}.");
        }
        catch (IncorrectPasswordException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest,
                $"{ex.Message}. Path:{context.Request.Path}.");
        }
        catch (NameNotUniqueException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest,
                $"{ex.Message}. Path:{context.Request.Path}.");
        }
        catch (NotEnoughRightsException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.Forbidden,
                $"{ex.Message}. Path:{context.Request.Path}.");
        }
        catch (OperationCanceledException ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest,
                $"{ex.Message}. Path:{context.Request.Path}.");
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError,
               $"{ex.Message}. Path:{context.Request.Path}.");
        }
    }

    private Task HandleExceptionAsync(HttpContext context, HttpStatusCode code, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(
            new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            }.ToString());
    }
}