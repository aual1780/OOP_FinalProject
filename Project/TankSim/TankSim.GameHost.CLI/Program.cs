using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TankSim.GameHost.CLI.Extensions;
using TIPC.Core.Channels;

namespace TankSim.GameHost.CLI
{
    class Program
    {
        static async Task<int> Main()
        {
            TraceListener t = new ConsoleTraceListener();
            _ = Trace.Listeners.Add(t);

            //create local message broker
            //used to pass messages between ardnet feature nodes
            //can also be hooked by other systems
            using var msgHub = new MessageHub();
            msgHub.Start();

            //get player count as int
            Range playerCountRange = 1..6;
            int playerCount = -1;
            do
            {
                Console.Write("How many players? ");
            } while (!int.TryParse(Console.ReadLine(), out playerCount) || !playerCountRange.Contains(playerCount));

            Console.Write("Keep dead games alive? (Y|N)? ");
            bool persistDeadGames = string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase);

            Console.Write("Bind local controller (Y|N)? ");
            bool bindLocalController = string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase);

            //application scope
            while (true)
            {
                GamepadService gamepadSvc = null;
                try
                {
                    //create ardnet server
                    using var ardServ = ArdNetFactory.GetArdServer(msgHub);
                    //create game communincation manager
                    //watches for clients
                    //tracks command inputs
                    using var commState = new TankSimCommService(ardServ, playerCount);
                    //create gamepad watcher
                    //hook into server event stream
                    //bind controls for all operator roles
                    if (bindLocalController)
                    {
                        gamepadSvc = new GamepadService(ardServ);
                        _ = gamepadSvc.TrySetControllerIndex(0);
                        gamepadSvc.SetRoles(OperatorRoles.All);
                    }

                    //print game ID so clients know where to connect
                    Console.WriteLine($"Game ID: {commState.GameID}");

                    //wait for all players to join
                    await commState.GetConnectionTask();
                    Console.WriteLine("Game Started.");

                    //setup async command event watchers
                    //any inbound events will trigger the associated handler
                    var cmdFacade = commState.CmdFacade;
                    cmdFacade.MovementChanged += (sender, e) => Console.WriteLine($"{sender.Endpoint}: Dir: {e}");
                    cmdFacade.AimChanged += (sender, e) => Console.WriteLine($"{sender.Endpoint}: Aim.{e}");
                    cmdFacade.FireControlCmdReceived += (sender, e) => Console.WriteLine($"{sender.Endpoint}: Fire.{e.WeaponType} ({e.InitTime.GetTimeDiff()} ms)");
                    cmdFacade.GunLoaderCmdReceived += (sender, e) => Console.WriteLine($"{sender.Endpoint}: Loader.{e.LoaderType} ({e.InitTime.GetTimeDiff()} ms)");

                    while (true)
                    {
                        Thread.Sleep(10);
                        //if player count drops, then restart outer loop
                        if (persistDeadGames)
                        {
                            _ = Console.ReadLine();
                            break;
                        }
                        else if (ardServ.ConnectedClientCount < playerCount)
                            break;
                    }
                }
                finally
                {
                    gamepadSvc?.Dispose();
                }
            }

        }
    }
}
