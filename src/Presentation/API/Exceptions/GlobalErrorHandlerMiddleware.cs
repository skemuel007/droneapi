using System.Net;
using System.Text.Json;
using Application.Exceptions;
using Application.Responses;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Exceptions;

public class GlobalErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

        /// <summary>
        /// Global error handler request method
        /// </summary>
        /// <param name="next"></param>
        public GlobalErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

            } catch (Exception e)
            {
                if( e.InnerException != null )
                {
                    // TODO: write logs to Log service
                } else
                {
                    // TODO: write the logs to Log service
                }
                await HandleErrorAsync(context, e);
            }
        }

        public static Task HandleErrorAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            switch (exception)
            {
                // not found error
                case ValidationException e:
                    context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    statusCode = HttpStatusCode.UnprocessableEntity;
                    break;
                case KeyNotFoundException e:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    statusCode = HttpStatusCode.NotFound;
                    break;
                case DbUpdateException e:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case InvalidOperationException e:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            // create response
            var response = new BaseCommandResponse
            {
                Success = false,
                Message = exception.InnerException != null ? exception.InnerException.Message : exception.Message,
                StatusCode = statusCode
            };
            var payload = JsonConvert.SerializeObject(response);
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(payload);
        }
}