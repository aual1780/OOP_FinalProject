using ArdNet;
using ArdNet.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TankSim;
using TankSim.GameHost;
using TankSim.OperatorDelegates;
using TankSim.TankSystems;
using TIPC.Core.Channels;
using UnityEngine;

public class ServerHandler
{
    private readonly MessageHub _msgHub = MessageHub.StartNew();
    private readonly TaskCompletionSource<OperatorCmdFacade> _serverstartupTask = new TaskCompletionSource<OperatorCmdFacade>(TaskCreationOptions.RunContinuationsAsynchronously);
    private LoggingMessageHubClient _msgHubLogger;
    private IArdNetServer _ardServ;
    private TankSimCommService _commState;

    public bool AreAllPlayersReady { get; private set; } = false;

    public bool IsServerRunning { get; private set; } = false;

    public bool IsServerBeingCreated { get; private set; } = false;

    public ServerHandler()
    {

    }

    public async void CreateServer(int playerCount)
    {
        IsServerBeingCreated = true;

        if (GlobalDebugFlag.LogTypes != MessageCategoryTypes.None)
        {
            _msgHubLogger = new LoggingMessageHubClient(_msgHub, GlobalDebugFlag.LogTypes);
            _msgHubLogger.ExceptionPushed += (s, e) => Debug.LogError(e.ToString());
            _msgHubLogger.MessagePushed += (s, e) => Debug.Log(e.ToString());
            _msgHubLogger.InformationPushed += (s, e) => Debug.Log(e.ToString());
            _msgHubLogger.LogPushed += (s, e) => Debug.Log(e.ToString());
            _msgHubLogger.Start();
        }

        //create ardnet server
        _ardServ = ArdNetFactory.GetArdServer(_msgHub);


        //create game communincation manager
        //watches for clients
        //tracks command inputs
        _commState = await TankSimCommService.Create(_ardServ, playerCount);

        //cmdFacade.AimChanged += CmdFacade_AimChanged;

        //release task in new thread to guard against deadlocks
        IsServerRunning = true;
        _serverstartupTask.SetResult(_commState.CmdFacade);

        await _commState.GetConnectionTask();
        AreAllPlayersReady = true;
    }

    public void CloseServer()
    {
        _commState?.Dispose();
        _ardServ?.Dispose();
        _msgHubLogger?.Dispose();
        _msgHub.Dispose();
    }

    public int GetCurrentConnectedPlayers() => _commState.PlayerCountCurrent;

    public string GetLobbyCode() => _commState.GameID;

    public IEnumerable<TankControllerState> GetPlayers() => _commState.ConnectedControllers;


    //Func<IConnectedSystemEndpoint, MovementDirection> movementFunc
    public async Task AddMovementFunction(TankMovementCmdEventHandler movementFunc)
    {
        var _cmdFacade = await _serverstartupTask.Task;
        _cmdFacade.MovementChanged += movementFunc;
    }

    public async Task AddAimFunction(TankMovementCmdEventHandler aimFunc)
    {
        var _cmdFacade = await _serverstartupTask.Task;
        _cmdFacade.AimChanged += aimFunc;
    }

    public async Task AddPrimaryFireFunction(Action<IConnectedSystemEndpoint, PrimaryWeaponFireState> fireFunc)
    {
        var _cmdFacade = await _serverstartupTask.Task;
        _cmdFacade.PrimaryWeaponFired += fireFunc;
    }

    public async Task AddSecondaryFireFunction(Action<IConnectedSystemEndpoint> fireFunc)
    {
        var _cmdFacade = await _serverstartupTask.Task;
        _cmdFacade.SecondaryWeaponFired += fireFunc;
    }

    /*
     * for primary weapon
     */
    public async Task AddGunLoadFunction(Action<IConnectedSystemEndpoint> loadFunc)
    {
        var _cmdFacade = await _serverstartupTask.Task;
        _cmdFacade.PrimaryGunLoaded += loadFunc;
    }

    /*
     * for primary weapon
     */
    public async Task AddAmmoFunction(Action<IConnectedSystemEndpoint> ammoFunc)
    {
        var _cmdFacade = await _serverstartupTask.Task;
        _cmdFacade.PrimaryAmmoCycled += ammoFunc;
    }
}
