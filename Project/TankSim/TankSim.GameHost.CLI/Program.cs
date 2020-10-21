using ArdNet.DependencyInjection;
using ArdNet.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TankSim.GameHost.CLI.Extensions;
using TIPC.Core.Tools.Extensions;

namespace TankSim.GameHost.CLI
{
    class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }


        static async Task<int> Main()
        {
            var serviceCollection = new ServiceCollection();
            _ = serviceCollection
                .AddScoped<IArdNetServer>((sp) => ArdNetFactory.GetArdServer());

            ServiceProvider = serviceCollection.BuildServiceProvider();

            //get player count as int
            Range playerCountRange = 1..6;
            int playerCount = -1;
            do
            {
                Console.Write("How many players? ");
            } while (!int.TryParse(Console.ReadLine(), out playerCount) || !playerCountRange.Contains(playerCount));


        GameStart:
            //application scope
            using (var appScope = ServiceProvider.CreateScope())
            {
                var server = appScope.ServiceProvider.GetRequiredService<IArdNetServer>();
                using var commState = new TankSimCommService(server, playerCount);
                Console.WriteLine($"Game ID: {commState.GameID}");

                //wait for all players to join
                await commState.GetConnectionTask();
                Console.WriteLine("Game Started.");

                var cmdFacade = commState.CmdFacade;
                cmdFacade.MovementChanged += (sender, e) => Console.WriteLine($"{sender.Endpoint}: Dir: {e}");
                cmdFacade.FireControlCmdReceived += (sender, e) => Console.WriteLine($"{sender.Endpoint}: Fire.{e.WeaponType} ({e.InitTime.GetTimeDiff()} ms)");
                cmdFacade.GunLoaderCmdReceived += (sender, e) => Console.WriteLine($"{sender.Endpoint}: Loader.{e.LoaderType} ({e.InitTime.GetTimeDiff()} ms)");
                cmdFacade.GunRotationCmdReceived += (sender, e) => Console.WriteLine($"{sender.Endpoint}: GunRot.{e.Direction} ({e.InitTime.GetTimeDiff()} ms)");
                cmdFacade.RangeFinderCmdReceived += (sender, e) => Console.WriteLine($"{sender.Endpoint}: Range.{e.Direction} ({e.InitTime.GetTimeDiff()} ms)");

                while (true)
                {
                    Thread.Sleep(10);
                    if (server.ConnectedClientCount < playerCount)
                        goto GameStart;
                }
            }

            return 0;
        }
    }
}
