using System.IO;
using ConsoleApp.Demo.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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
            services.AddSingleton<ILoggerFactory, LoggerFactory>();

            //调用 AddDirectoryBrowser 扩展方法来增加直接访问目录所需服务
            services.AddDirectoryBrowser();

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
            loggerFactory
             .AddConsole()
             .AddDebug(LogLevel.Information);

            //UseDefaultFiles 必须在 UseStaticFiles 之前调用。UseDefaultFiles 只是重写了 URL，而不是真的提供了这样一个文件。
            // 必须开启静态文件中间件（UseStaticFiles）来提供这个文件
            app.UseDefaultFiles();

            // 配置静态文件服务,
            // 通过配置静态文件中间件加入到管道中，即可配置静态文件服务
            app.UseStaticFiles();

            // 静态文件的位于 web root 的外部的配置
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"anothorstaticfiles")),
                RequestPath = new PathString("/staticfiles")
            });

            // 允许直接浏览目录的配置
            app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),@"wwwroot\images")),
                RequestPath = new PathString("/imagesdir")
            });

       

            // 将多个请求委托彼此链接在一起；next 参数表示管道内下一个委托。
            // 通过 不 调用 next 参数，你可以中断（短路）管道。
            //你通常可以在执行下一个委托之前和之后执行一些操作，如下例所示

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Hello World!</br>");
                await next.Invoke();
                await context.Response.WriteAsync("Hello New Year</br>");
            });

            // 使用自定义的中间件
            app.UserRequestLogger();

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello, World, Again!</br>");
            });
        }
    }
}