using ConsoleApp.Demo.Middleware;
using Microsoft.AspNetCore.Builder;

namespace ConsoleApp.Demo.Extensions
{
    //通过 IApplicationBuilder 的扩展方法来暴露给中间件的使用者
    public static class RequestLoggerExtensions
    {
        public static IApplicationBuilder UserRequestLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggerMiddleware>();
        }
    }
}