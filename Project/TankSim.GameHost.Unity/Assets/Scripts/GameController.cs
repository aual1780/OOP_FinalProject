using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TankSim.TankSystems;
using TankSim.GameHost;
using TIPC.Core.Channels;
using System.Dynamic;

public class GameController : MonoBehaviour
{

    public string gameName { get; private set; }

    public int expectedPlayerCount { get; private set; }

    private ServerHandler sh;
     

    // Start is called before the first frame update
    void Start()
    {
        //don't destroy this object
        DontDestroyOnLoad(gameObject);
        sh = new ServerHandler();
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CreateLobby(int players, string gameName)
    {
        SceneManager.LoadScene("LobbyScene");
        this.gameName = gameName;
        expectedPlayerCount = players;

        //using var ardServ = ArdNetFactory.GetArdServer(msgHub_);
        sh.CreateServer(players);
        //ServerHandler.Server(players);

    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");

        //close server because no server should be active in the main menu
        sh.CloseServer();

        //destroy this game object because it already exists in the main menu scene
        Destroy(gameObject);
    }


    /// <summary>
    /// returns null if server is not running
    /// </summary>
    public string GetLobbyCode()
    {
        if (sh.serverRunning)
        {
            return sh.GetLobbyCode();
        }
        else
        {
            return null;
        }
        
    }

    public int GetCurrentConnectedPlayers()
    {
        if (sh.serverRunning)
        {
            return sh.GetCurrentConnectedPlayers();
        }
        else
        {
            return -1;
        }
    }
}
