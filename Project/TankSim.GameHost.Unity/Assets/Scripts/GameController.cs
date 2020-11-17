﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TankSim.TankSystems;
using TankSim.GameHost;
using TIPC.Core.Channels;
using System.Dynamic;
using TankSim.OperatorDelegates;
using ArdNet;
using System.Threading.Tasks;
using System;

public class GameController : MonoBehaviour
{
    private ServerHandler _serverHandler;

    public string GameName { get; private set; }
    public int ExpectedPlayerCount { get; private set; }

     

    // Start is called before the first frame update
    void Start()
    {
        //don't destroy this object
        DontDestroyOnLoad(gameObject);
        _serverHandler = new ServerHandler();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CreateLobby(int players, string gameName)
    {
        SceneManager.LoadScene("LobbyScene");
        this.GameName = gameName;
        ExpectedPlayerCount = players;

        //using var ardServ = ArdNetFactory.GetArdServer(msgHub_);
        _serverHandler.CreateServer(players);
        //ServerHandler.Server(players);

    }

    public void StartGame()
    {
        if (_serverHandler.AreAllPlayersReady)
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");

        //close server because no server should be active in the main menu
        _serverHandler.CloseServer();

        //destroy this game object because it already exists in the main menu scene
        Destroy(gameObject);
    }


    /// <summary>
    /// Attempt to get server lobby code
    /// </summary>
    /// <returns>True if the server is running, false otherwise</returns>
    public bool TryGetLobbyCode(out string LobbyCode)
    {
        if (_serverHandler.IsServerRunning)
        {
            LobbyCode = _serverHandler.GetLobbyCode();
            return true;
        }
        else
        {
            LobbyCode = null;
            return false;
        }
    }

    public int GetCurrentConnectedPlayers()
    {
        if (_serverHandler.IsServerRunning)
        {
            return _serverHandler.GetCurrentConnectedPlayers();
        }
        else
        {
            return -1;
        }
    }

    public bool AllPlayersReady()
    {
        return _serverHandler.AreAllPlayersReady;
    }


    private void OnApplicationQuit()
    {
        if (_serverHandler.IsServerRunning)
        {
            _serverHandler.CloseServer();
        }
    }


    public async Task AddTankFunctions(TankMovementCmdEventHandler movementFunc, TankMovementCmdEventHandler aimFunc,
        Action<IConnectedSystemEndpoint, PrimaryWeaponFireState> fireFunc, 
        Action<IConnectedSystemEndpoint> secondaryFireFunc,
        Action<IConnectedSystemEndpoint> loadFunc, 
        Action<IConnectedSystemEndpoint> ammoFunc)
    {
        var t1 = _serverHandler.AddMovementFunction(movementFunc);
        var t2 = _serverHandler.AddAimFunction(aimFunc);
        var t3 = _serverHandler.AddPrimaryFireFunction(fireFunc);
        var t4 = _serverHandler.AddSecondaryFireFunction(secondaryFireFunc);
        var t5 = _serverHandler.AddGunLoadFunction(loadFunc);
        var t6 = _serverHandler.AddAmmoFunction(ammoFunc);
        await Task.WhenAll(t1, t2, t3, t4, t5, t6);
    }
}