using ArdNet.DependencyInjection;
using ArdNet.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using TankSim.GameHost.CLI.Extensions;
using TIPC.Core.Tools.Extensions;

namespace TankSim.GameHost.CLI
{
    class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }


        static void Main()
        {
            var serviceCollection = new ServiceCollection();
            _ = serviceCollection
                .AddScoped<IArdNetServer>((sp) => ArdNetFactory.GetArdServer())
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
            var roleLock = new object();

        GameStart:
            //application scope
            using (var appScope = ServiceProvider.CreateScope())
            {
                var gameStarted = false;
                var randRoleSets = roleSets.ToList();
                randRoleSets.Randomize();
                CountdownEvent playerWaiter = new CountdownEvent(randRoleSets.Count);
                var server = appScope.ServiceProvider.GetRequiredService<IArdNetServer>();
                var gameID = server.NetConfig.UDP.AppID.Split('.')[^1];
                Console.WriteLine($"Game ID: {gameID}");

                server.TcpEndpointConnected += (sender, sys) =>
                {
                    Console.WriteLine($"Connected: {sys.Endpoint}");
                    var state = new TankControllerState();
                    sys.UserState = state;
                    lock (sys.SyncRoot)
                    {
                        state.Name = $"Anon{sys.Endpoint.Port}";
                    }

                };
                server.TcpEndpointDisconnected += (sender, sys) =>
                {
                    var state = (TankControllerState)sys.UserState;

                    lock (sys.SyncRoot)
                    {
                        if (state.IsReady && !gameStarted)
                        {
                            playerWaiter.AddCount();
                        }
                    }
                    Console.WriteLine($"Disconnected: {sys.Endpoint}");
                };

                //set client name on system state
                server.TcpCommandRequestProcessor.RegisterProcessor(
                    Constants.Commands.ControllerInit.SetClientName,
                    (sender, request) =>
                    {
                        var system = request.RequestArg.ConnectedSystem;
                        var state = (TankControllerState)system.UserState;
                        lock (system.SyncRoot)
                        {
                            if ((request.RequestArg.RequestArgs?.Length ?? 0) > 0)
                            {
                                state.Name = request.RequestArg.RequestArgs[0];
                            }
                        }
                        Console.WriteLine($"Hi {state.Name} ({request.RequestArg.Endpoint})");
                    });

                server.TcpQueryRequestProcessor.RegisterProcessor(
                    Constants.Queries.ControllerInit.GetOperatorRoles,
                    (sender, request) =>
                    {
                        var system = request.RequestArg.ConnectedSystem;
                        var state = (TankControllerState)system.UserState;
                        lock (roleLock)
                        {
                            var roleSet = randRoleSets.Pop();
                            request.Respond(roleSet.ToString());
                            state.Roles = roleSet;
                        }
                        lock (system.SyncRoot)
                        {
                            _ = playerWaiter.Signal();
                            state.IsReady = true;
                        }
                    });

                //wait for all players to join
                playerWaiter.Wait();
                gameStarted = true;
                Console.WriteLine("Game Started.");

                using var cmdFacade = appScope.ServiceProvider.GetRequiredService<OperatorCmdFacade>();
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


        }
    }
}
