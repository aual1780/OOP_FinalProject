using ArdNet.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace TankSim.GameHost.CLI
{
    class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IConfiguration Configuration { get; private set; }


        static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            _ = configBuilder.AddJsonFile("config.json", optional: false, reloadOnChange: true);
            Configuration = configBuilder.Build();

            var serviceCollection = new ServiceCollection();
            _ = serviceCollection
                .AddMessageHubSingleton()
                .AddIpResolver()
                .AddArdNet(Configuration.GetSection("ArdNet"))
                .AddServerScoped()
                .AutoStart()
                .AddTankSimConfig();
            ServiceProvider = serviceCollection.BuildServiceProvider();

            //application scope
            using (var appScope = ServiceProvider.CreateScope())
            {
                var server = appScope.ServiceProvider.GetRequiredService<IArdNetServer>();

                var config = (ArdNetServerConfig)server.NetConfig;
                var gameID = config.UDP.AppID.Split('.')[^1];
                Console.WriteLine($"Game ID: {gameID}");
                Console.ReadLine();
            }


        }
    }
}
