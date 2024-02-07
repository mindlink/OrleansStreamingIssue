using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace OrleansStreamingIssue
{
    internal class Program
    {
        const string ConnectionString = "Server=.;Initial Catalog=OrleansStreamingIssue;Integrated Security=SSPI";

        static async Task Main(string[] args)
        {
            var silo = CreateSqlHost();

            await silo.StartAsync();

            var grainFactory = (IGrainFactory)silo.Services.GetService(typeof(IGrainFactory))!;

            var grainAId = Guid.NewGuid();

            var grainA = grainFactory.GetGrain<IGrainA>(grainAId);
            var otherGrainA = grainFactory.GetGrain<IGrainA>(Guid.NewGuid());
            var grainB = grainFactory.GetGrain<IGrainB>(Guid.NewGuid());

            await otherGrainA.Subscribe(grainAId);
            await grainB.Subscribe(grainAId);

            await grainA.SendMessage("hi");

            await silo.StopAsync();
        }

        static ISiloHost CreateSqlHost()
        {
            var siloHostBuilder = new SiloHostBuilder();

            siloHostBuilder.UseLocalhostClustering()
                .AddAdoNetGrainStorage(
                    "PubSubStore",
                    options =>
                    {
                        options.Invariant = "Microsoft.Data.SqlClient";
                        options.ConnectionString = ConnectionString;
                    })
                .AddSimpleMessageStreamProvider("SimpleMessaging")
                .Configure<ClusterOptions>(
                    options =>
                    {
                        options.ClusterId = "test";
                    });

            return siloHostBuilder.Build();
        }
    }
}
