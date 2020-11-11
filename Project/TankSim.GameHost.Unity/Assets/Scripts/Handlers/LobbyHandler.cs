using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviour
{

    public Text lobbyCodeText;
    private GameController gc;

    private bool hasSetLobbyCode = false;

    public Button startGameButton;
    public GameObject playersPanel;
    public GameObject playerPanelPrefab;

    private Text[] playersTexts;

    // Start is called before the first frame update
    void Start()
    {
        gc = FindObjectOfType<GameController>();
        if (gc == null)
        {
            Debug.LogError("GameController is not here but should be");
            return;
        }
        LoadLobbyUI(gc.expectedPlayerCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasSetLobbyCode)
        {
            string code = gc.GetLobbyCode();
            if (code == null)
            {
                lobbyCodeText.text = "Lobby Code: loading...";
            }
            else
            {
                lobbyCodeText.text = "Lobby Code: " + code;
                hasSetLobbyCode = true;
            }
            return;
        }

        UpdateLobbyUI();
    }


    private void LoadLobbyUI(int players)
    {
        playersTexts = new Text[players];
        for (int i = 0; i < players; ++i)
        {
            playersTexts[i] = Instantiate(playerPanelPrefab, playersPanel.transform).GetComponentInChildren<Text>();
            if (playersTexts[i] != null)
            {
                playersTexts[i].text = "Player " + (i + 1) + ": Not Ready";
            }
            else
            {
                print("wat");
            }
            
        }
    }

    private void UpdateLobbyUI()
    {
        int playersReady = gc.GetCurrentConnectedPlayers();
        if (playersReady > playersTexts.Length)
        {
            Debug.LogError("This should not happen. more players Ready then expected");
            return;
        }
        string state = "";
        if (gc.AllPlayersReady())
        {
            startGameButton.GetComponentInChildren<Text>().text = "Start Game";
            state = "Ready";
        }
        else
        {
            startGameButton.GetComponentInChildren<Text>().text = "Waiting...";
            state = "Joined";
        }
        for (int i = 0; i < playersReady; ++i)
        {
            playersTexts[i].text = "Player " + (i + 1) + ": " + state;
        }
    }


    public void StartGameClicked()
    {
        gc.StartGame();
    }

    public void ExitLobbyClicked()
    {
        gc.GoBackToMainMenu();
    }
}
