using Grpc.Net.Client;
using GrpcProductServiceApi;

namespace ProductServiceIntegrationTests.GrpcFixture;

public class GrpcIntegrationTestBase : IClassFixture<GrpcTestFixture<Startup>>, IDisposable
{
    private GrpcChannel? _channel;
    protected GrpcTestFixture<Startup> Fixture { get; set; }

    protected GrpcChannel Channel => _channel ??= CreateChannel();

    protected GrpcChannel CreateChannel()
    {
        return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
        {
            HttpHandler = Fixture.Handler
        });
    }

    public GrpcIntegrationTestBase(GrpcTestFixture<Startup> fixture)
    {
        Fixture = fixture;
    }

    public void Dispose()
    {
        _channel = null;
    }
}