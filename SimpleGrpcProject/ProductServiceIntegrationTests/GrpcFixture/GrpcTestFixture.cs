using Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;

namespace ProductServiceIntegrationTests.GrpcFixture
{
    public class GrpcTestFixture<TStartup> : IDisposable where TStartup : class
    {
        public Mock<IProductRepository> ProductRepositoryFake { get; } = new(MockBehavior.Strict);
        private TestServer? _server;
        private IHost? _host;
        private HttpMessageHandler? _handler;

        private void EnsureServer()
        {
            if (_host == null)
            {
                var builder = new HostBuilder()
                    .ConfigureWebHostDefaults(webHost =>
                    {
                        webHost
                            .UseTestServer()
                            .UseStartup<TStartup>();
                    })
                    .ConfigureServices(services =>
                    {
                        services.Replace(new ServiceDescriptor(typeof(IProductRepository),
                            ProductRepositoryFake.Object));
                    });
                _host = builder.Start();
                _server = _host.GetTestServer();
                _handler = _server.CreateHandler();
            }
        }

        public HttpMessageHandler Handler
        {
            get
            {
                EnsureServer();
                return _handler!;
            }
        }

        public void Dispose()
        {
            _handler?.Dispose();
            _host?.Dispose();
            _server?.Dispose();
        }
    }
}