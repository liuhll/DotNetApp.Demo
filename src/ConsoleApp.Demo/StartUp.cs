using ConsoleApp.Demo.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace ConsoleApp.Demo
{
    public class StartUp
    {
        public StartUp(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration {  get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ILoggerFactory, LoggerFactory>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            // 第一个 App.Run 委托中断了管道。在下面的例子中，只有第一个委托（“Hello, World!”）会被运行
            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("Hello, World, Again!");
            //});


            // 将多个请求委托彼此链接在一起；next 参数表示管道内下一个委托。
            // 通过 不 调用 next 参数，你可以中断（短路）管道。
            //你通常可以在执行下一个委托之前和之后执行一些操作，如下例所示

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Hello World!</br>");
                await next.Invoke();
                await context.Response.WriteAsync("Hello New Year</br>");
            });

            app.UserRequestLogger();

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello, World, Again!</br>");
            });
        }
    }
}