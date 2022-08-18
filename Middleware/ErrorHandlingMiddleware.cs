using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagerAPI.Exceptions;

namespace WarehouseManagerAPI.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            
            catch (BadRequestException badRequest)
            {
                _logger.LogError(badRequest, badRequest.Message);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(badRequest.Message);
            }
            
            catch (InvalidPasswordException invalidPassword)
            {
                _logger.LogError(invalidPassword, invalidPassword.Message);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(invalidPassword.Message);
            }

            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong");
            }
        }
    }
}
