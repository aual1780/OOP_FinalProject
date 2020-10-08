using ArdNet.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using TankSim.Client.Services;

namespace TankSim.Client.CLI.Services
{
    public class GameScopeService : IGameScopeService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _connectionTimeout = TimeSpan.FromSeconds(3);

        public GameScopeService(IServiceProvider ServiceProvider)
        {
            _serviceProvider = ServiceProvider;
        }


        public async Task<IServiceScope> GetValidGameScope()
        {
            while (true)
            {
                Console.Write("Enter GameID: ");
                var gameID = Console.ReadLine();
                var isValid = GameIdGenerator.Validate(gameID);
                if (!isValid)
                {
                    Console.Write("Invalid GameID.  ");
                    continue;
                }



                var scope = _serviceProvider.CreateScope();
                var idService = scope.ServiceProvider.GetRequiredService<GameIdService>();
                idService.GameID = gameID;
                var ardClient = scope.ServiceProvider.GetRequiredService<IArdNetClient>();

                using (var tokenSrc = new CancellationTokenSource(_connectionTimeout))
                {
                    try
                    {
                        var endptTask = ardClient.ConnectAsync(tokenSrc.Token);
                        Console.WriteLine("Connecting...");
                        var endpt = await endptTask;
                        if (endpt != null && !tokenSrc.IsCancellationRequested)
                        {
                            Console.WriteLine("Connected.");
                            return scope;
                        }
                        else
                        {
                            Console.Write("Cannot connect to the target host. ");
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        //noop
                        //continue search
                        Console.Write("Cannot connect to the target host. ");
                    }

                }
            }
        }
    }
}
