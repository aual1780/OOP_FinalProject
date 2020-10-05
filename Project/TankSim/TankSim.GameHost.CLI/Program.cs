﻿using ArdNet.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using TankSim.Extensions;
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
                var config = (ArdNetServerConfig)server.NetConfig;
                var gameID = config.UDP.AppID.Split('.')[^1];
                Console.WriteLine($"Game ID: {gameID}");


                server.TcpQueryReceived += (sender, e) =>
                {
                    if (e.Request == Constants.Queries.ControllerInit.GetOperatorRoles)
                    {
                        lock (randRoleSets)
                        {
                            var roleSet = randRoleSets[^1];
                            randRoleSets.RemoveAt(randRoleSets.Count - 1);
                            server.SendTcpQueryResponse(e, roleSet.ToString());
                        }
                        _ = playerWaiter.Signal();
                    }
                };

                //wait for all players to join
                playerWaiter.Wait();
                Console.WriteLine("Game Started.");

                var cmdFacade = appScope.ServiceProvider.GetRequiredService<OperatorCmdFacade>();
                cmdFacade.DriverCommandReceived += (sender, e) => Console.WriteLine($"{sender}: {e.Direction}");

                while (true)
                {
                    Thread.Sleep(10);
                    if(server.ConnectedClientCount < playerCount)
                    goto GameStart;
                }
            }


        }
    }
}
