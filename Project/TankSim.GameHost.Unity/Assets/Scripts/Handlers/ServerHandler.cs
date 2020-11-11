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

    private MessageHub msgHub = new MessageHub();

    private int playerCount = -1;

    private IArdNetServer ardServ;
    private TankSimCommService commState;
    private OperatorCmdFacade cmdFacade;

    public bool serverRunning { get; private set; } = false;

    public bool serverBeingCreated { get; private set; } = false;

    public ServerHandler()
    {
        msgHub.Start();
    }

    public async void CreateServer(int playerCount)
    {
        serverBeingCreated = true;
        this.playerCount = playerCount;


        //create ardnet server
        ardServ = ArdNetFactory.GetArdServer(msgHub);

        //create game communincation manager
        //watches for clients
        //tracks command inputs
        commState = await TankSimCommService.Create(ardServ, playerCount);
        cmdFacade = commState.CmdFacade;
        serverRunning = true;
    }

    public void CloseServer()
    {
        if (ardServ != null)
        {
            ((IDisposable)ardServ).Dispose();
        }

        if (commState != null)
        {
            ((IDisposable)commState).Dispose();
        }
    }

    public bool HaveAllPlayersConnected()
    {
        return commState.PlayerCountCurrent == commState.PlayerCountTarget;
    }

    public int GetCurrentConnectedPlayers()
    {
        return commState.PlayerCountCurrent;
    }

    public string GetLobbyCode()
    {
        return commState.GameID;
    }



    //Func<IConnectedSystemEndpoint, MovementDirection> movementFunc
    public void AddMovementFunction(TankMovementCmdEventHandler movementFunc)
    {
        if (serverRunning)
        {
            cmdFacade.MovementChanged += movementFunc;
        }
        else
        {
            UnityEngine.Debug.LogError("Server is not running");
        }
    }

    public void AddAimFunction(TankMovementCmdEventHandler aimFunc)
    {
        if (serverRunning)
        {
            cmdFacade.AimChanged += aimFunc;
        }
        else
        {
            UnityEngine.Debug.LogError("Server is not running");
        }
    }

    public void AddPrimaryFireFunction(System.Action<IConnectedSystemEndpoint, PrimaryWeaponFireState> fireFunc)
    {
        if (serverRunning)
        {
            cmdFacade.PrimaryWeaponFired += fireFunc;
        }
        else
        {
            UnityEngine.Debug.LogError("Server is not running");
        }
    }

    public void AddSecondaryFireFunction(System.Action<IConnectedSystemEndpoint> fireFunc)
    {
        if (serverRunning)
        {
            cmdFacade.SecondaryWeaponFired += fireFunc;
        }
        else
        {
            UnityEngine.Debug.LogError("Server is not running");
        }
    }

    /*
     * for primary weapon
     */
    public void AddGunLoadFunction(System.Action<IConnectedSystemEndpoint> loadFunc)
    {
        if (serverRunning)
        {
            cmdFacade.PrimaryGunLoaded += loadFunc;
        }
        else
        {
            UnityEngine.Debug.LogError("Server is not running");
        }
    }


    /*
     * for primary weapon
     */
    public void AddAmmoFunction(System.Action<IConnectedSystemEndpoint> ammoFunc)
    {
        if (serverRunning)
        {
            cmdFacade.PrimaryAmmoCycled += ammoFunc;
        }
        else
        {
            UnityEngine.Debug.LogError("Server is not running");
        }
    }
}
