using Application;
using DataAccess;
using FluentValidation;
using GrpcProductServiceApi.Interceptors;
using GrpcProductServiceApi.Services;

namespace GrpcProductServiceApi;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc(o =>
        {
            o.Interceptors.Add<ExceptionInterceptor>();
            o.Interceptors.Add<LoggingInterceptor>();
            o.Interceptors.Add<ValidationInterceptor>();
        }).AddJsonTranscoding();
        services.AddGrpcSwagger();
        services.AddSwaggerGen();
        services.AddValidatorsFromAssemblyContaining(typeof(Program));
        services.AddRepositories();
        services.AddServices();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<ProductServiceGrpc>();
        });
    }
}