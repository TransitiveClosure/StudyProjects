using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using TaskManagementSystem.Bll.Extensions;
using TaskManagementSystem.Dal.Extensions;
using TaskManagementSystem.Interceptors;
using TaskManagementSystem.Services;
using TaskManagementSystem.Utilities.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5213, o => o.Protocols = HttpProtocols.Http2);
});

var services = builder.Services;

services.AddGrpc(
    o => o.Interceptors.Add<RateLimiterInterceptor>()
    );

services.AddFluentValidation(conf =>
{
    conf.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
    conf.AutomaticValidationEnabled = true;
});

services
    .AddBllServices()
    .AddDalInfrastructure(builder.Configuration)
    .AddDalRepositories()
    .AddUtilities();

services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["DalOptions:RedisConnectionString"];
});

services.AddGrpcReflection();


var app = builder.Build();

app.MapGrpcService<TasksService>();
app.MapGrpcReflectionService();

app.MigrateUp();

app.Run();