using ArdNet.Messaging;
using ArdNet.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using TankSim.GameHost.CLI.Extensions;
using TIPC.Core.Tools;
using TIPC.Core.Tools.Extensions;

namespace TankSim.GameHost.CLI
{
    class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IConfiguration Configuration { get; private set; }


        static void Main()
        {
            var configBuilder =
                new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("config.json", optional: false, reloadOnChange: true);
            Configuration = configBuilder.Build();

            var serviceCollection = new ServiceCollection();
            _ = serviceCollection
                .AddMessageHubSingleton()
                .AddIpResolver()
                .AddArdNet(Configuration.GetSection("ArdNet"))
                .AddServerScoped()
                .AutoStart()
                .AddTankSimConfig();
            _ = serviceCollection
                .AddScoped<OperatorCmdFacade>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            //get player count as int
            Range playerCountRange = 1..6;
            int playerCount = -1;
            do
            {
                Console.Write("How many players? ");
            } while (!int.TryParse(Console.ReadLine(), out playerCount) || !playerCountRange.Contains(playerCount));
            var roleSets = OperatorRoleSets.GetDistributionSets(playerCount);

        GameStart:
            //application scope
            using (var appScope = ServiceProvider.CreateScope())
            {
                var randRoleSets = roleSets.ToList();
                randRoleSets.Randomize();
                CountdownEvent playerWaiter = new CountdownEvent(randRoleSets.Count);
                var server = appScope.ServiceProvider.GetRequiredService<IArdNetServer>();
                var gameID = server.NetConfig.UDP.AppID.Split('.')[^1];
                Console.WriteLine($"Game ID: {gameID}");

                server.TcpQueryReceived += (sender, e) =>
                {
                    if (e.Request == Constants.Queries.ControllerInit.GetOperatorRoles)
                    {
                        lock (randRoleSets)
                        {
                            var roleSet = randRoleSets.Pop();
                            server.SendTcpQueryResponse(e, roleSet.ToString());
                        }
                        _ = playerWaiter.Signal();
                    }
                };

                //wait for all players to join
                playerWaiter.Wait();
                Console.WriteLine("Game Started.");

                using var cmdFacade = appScope.ServiceProvider.GetRequiredService<OperatorCmdFacade>();
                cmdFacade.DriverCmdReceived += (sender, e) => Console.WriteLine($"{sender.Endpoint}: Drive.{e.Direction} ({(HighResolutionDateTime.UtcNow - e.InitTime).TotalMilliseconds})");
                cmdFacade.FireControlCmdReceived += (sender, e) => Console.WriteLine($"{sender.Endpoint}: Fire.{e.WeaponType}");
                cmdFacade.GunLoaderCmdReceived += (sender, e) => Console.WriteLine($"{sender.Endpoint}: Loader.{e.LoaderType}");
                cmdFacade.GunRotationCmdReceived += (sender, e) => Console.WriteLine($"{sender.Endpoint}: GunRot.{e.Direction}");
                cmdFacade.NavigatorCmdReceived += (sender, e) => Console.WriteLine($"{sender.Endpoint}: Nav.{e.Direction}");
                cmdFacade.RangeFinderCmdReceived += (sender, e) => Console.WriteLine($"{sender.Endpoint}: Range.{e.Direction}");

                while (true)
                {
                    Thread.Sleep(10);
                    if (server.ConnectedClientCount < playerCount)
                        goto GameStart;
                }
            }


        }
    }
}
