using Application;
using DataAccess;
using FluentValidation;
using GrpcProductServiceApi.Interceptors;
using GrpcProductServiceApi.Services;

namespace GrpcProductServiceApi;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}