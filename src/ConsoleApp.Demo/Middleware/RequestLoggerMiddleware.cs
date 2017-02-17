using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace ConsoleApp.Demo.Middleware
{
    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        //依赖注入，对loggerFactory注入需要在ConfigureServices对日志工厂进行配置，否则无法注入
        public RequestLoggerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            loggerFactory
            .AddConsole()
            .AddDebug(LogLevel.Information);
            _logger = loggerFactory.CreateLogger<RequestLoggerMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("start handler request");
            //将请求上下文交付给下一个请求委托
            await _next.Invoke(context);
            _logger.LogInformation("end request");
        }
    }
}