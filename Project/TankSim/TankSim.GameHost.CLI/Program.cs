using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TankSim.GameHost.CLI.Extensions;
using TankSim.TankSystems;
using TIPC.Core.Channels;
using static System.Console;

namespace TankSim.GameHost.CLI
{
    class Program
    {
        static async Task<int> Main()
        {
            Console.Title = Constants.GameName;
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
                Write("How many players? ");
            } while (!int.TryParse(ReadLine(), out playerCount) || !playerCountRange.Contains(playerCount));

            Write("Keep dead games alive? (Y|N)? ");
            bool persistDeadGames = string.Equals(ReadLine(), "y", StringComparison.OrdinalIgnoreCase);

            Write("Bind local controller (Y|N)? ");
            bool bindLocalController = string.Equals(ReadLine(), "y", StringComparison.OrdinalIgnoreCase);

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
                    using var commState = await TankSimCommService.Create(ardServ, playerCount);
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
                    WriteLine($"Game ID: {commState.GameID}");

                    //wait for all players to join
                    await commState.GetConnectionTask();
                    WriteLine("Game Started.");

                    //setup async command event watchers
                    //any inbound events will trigger the associated handler
                    var cmdFacade = commState.CmdFacade;
                    cmdFacade.MovementChanged += (s, e) => WriteLine($"{s.Endpoint}: Dir: {e}");
                    cmdFacade.AimChanged += (s, e) => WriteLine($"{s.Endpoint}: Aim.{e}");
                    cmdFacade.PrimaryWeaponFired += (s, e) =>
                    {
                        if (e == PrimaryWeaponFireState.Valid)
                            WriteLine($"{s.Endpoint}: Fire.Primary");
                        else if (e == PrimaryWeaponFireState.Misfire)
                            WriteLine($"{s.Endpoint}: Fire.Primary (MISFIRE)");
                        else if (e == PrimaryWeaponFireState.Empty)
                            WriteLine($"{s.Endpoint}: Fire.Primary (EMPTY)");
                    };
                    cmdFacade.SecondaryWeaponFired += (s) => WriteLine($"{s.Endpoint}: Fire.Secondary");
                    cmdFacade.PrimaryGunLoaded += (s) => WriteLine($"{s.Endpoint}: Loader.Load");
                    cmdFacade.PrimaryAmmoCycled += (s) => WriteLine($"{s.Endpoint}: Loader.Cycle");
                    while (true)
                    {
                        Thread.Sleep(10);
                        //if player count drops, then restart outer loop
                        if (persistDeadGames)
                        {
                            _ = ReadLine();
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
