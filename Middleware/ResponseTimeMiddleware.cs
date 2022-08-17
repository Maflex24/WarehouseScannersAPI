using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseScannersAPI.Middleware
{
    public class ResponseTimeMiddleware : IMiddleware
    {
        private readonly ILogger<ResponseTimeMiddleware> _logger;

        public ResponseTimeMiddleware(ILogger<ResponseTimeMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            await next.Invoke(context);
            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            var message =
                $"Request [{context.Request.Method}] {context.Request.Path} took {elapsedMilliseconds} ms";

            if (elapsedMilliseconds > 1000)
                _logger.LogWarning(message);
            else
                _logger.LogInformation(message);

        }
    }
}
