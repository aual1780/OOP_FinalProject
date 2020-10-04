using ArdNet.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TankSim.Client.Services;

namespace TankSim.Client.CLI.Services
{
    public class GameScopeService : IGameScopeService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly GameIdService _idService;
        private readonly TimeSpan _connectionTimeout = TimeSpan.FromSeconds(3);

        public GameScopeService(IServiceProvider ServiceProvider, GameIdService IdService)
        {
            _serviceProvider = ServiceProvider;
            _idService = IdService;
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

                _idService.GameID = gameID;


                var scope = _serviceProvider.CreateScope();
                var ardClient = scope.ServiceProvider.GetRequiredService<IArdNetClient>();
                using (var tokenSrc = new CancellationTokenSource(_connectionTimeout))
                {
                    try
                    {
                        var endpt = await ardClient.ConnectAsync(tokenSrc.Token);
                        if(endpt != null && !tokenSrc.IsCancellationRequested)
                        {
                            return scope;
                        }
                    }
                    catch(OperationCanceledException)
                    {
                        //noop
                        //continue search
                    }

                }
            }
        }
    }
}
