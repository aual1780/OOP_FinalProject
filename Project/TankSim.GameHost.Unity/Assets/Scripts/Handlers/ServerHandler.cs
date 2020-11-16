using ArdNet;
using ArdNet.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    private int _playerCount = -1;
    private readonly TaskCompletionSource<object> _serverstartupTask = new TaskCompletionSource<object>();

    private IArdNetServer ardServ;
    private TankSimCommService commState;
    private OperatorCmdFacade cmdFacade;
    public bool AreAllPlayersReady { get; private set; } = false;

    public bool serverRunning { get; private set; } = false;

    public bool serverBeingCreated { get; private set; } = false;

    public ServerHandler()
    {
        
    }

    public async void CreateServer(int playerCount)
    {
        serverBeingCreated = true;
        this._playerCount = playerCount;


        //create ardnet server
        ardServ = ArdNetFactory.GetArdServer(_msgHub);
        

        //create game communincation manager
        //watches for clients
        //tracks command inputs
        commState = await TankSimCommService.Create(ardServ, playerCount);
        cmdFacade = commState.CmdFacade;

        //cmdFacade.AimChanged += CmdFacade_AimChanged;

        //release task in new thread to guard against deadlocks
        await Task.Run(() => _serverstartupTask.SetResult(null));

        await commState.GetConnectionTask();
        AreAllPlayersReady = true;
    }

    public void CloseServer()
    {
        commState?.Dispose();
        ardServ?.Dispose();
    }

    public int GetCurrentConnectedPlayers() => commState.PlayerCountCurrent;

    public string GetLobbyCode() => commState.GameID;



    //Func<IConnectedSystemEndpoint, MovementDirection> movementFunc
    public async Task AddMovementFunction(TankMovementCmdEventHandler movementFunc)
    {
        _ = await _serverstartupTask.Task;
        cmdFacade.MovementChanged += movementFunc;
    }

    public async Task AddAimFunction(TankMovementCmdEventHandler aimFunc)
    {
        _ = await _serverstartupTask.Task;
        cmdFacade.AimChanged += aimFunc;
    }

    public async Task AddPrimaryFireFunction(Action<IConnectedSystemEndpoint, PrimaryWeaponFireState> fireFunc)
    {
        _ = await _serverstartupTask.Task;
        cmdFacade.PrimaryWeaponFired += fireFunc;
    }

    public async Task AddSecondaryFireFunction(Action<IConnectedSystemEndpoint> fireFunc)
    {
        _ = await _serverstartupTask.Task;
        cmdFacade.SecondaryWeaponFired += fireFunc;
    }

    /*
     * for primary weapon
     */
    public async Task AddGunLoadFunction(Action<IConnectedSystemEndpoint> loadFunc)
    {
        _ = await _serverstartupTask.Task;
        cmdFacade.PrimaryGunLoaded += loadFunc;
    }

    /*
     * for primary weapon
     */
    public async Task AddAmmoFunction(Action<IConnectedSystemEndpoint> ammoFunc)
    {
        _ = await _serverstartupTask.Task;
        cmdFacade.PrimaryAmmoCycled += ammoFunc;
    }
}
