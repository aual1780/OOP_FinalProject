using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviour
{

    public Text LobbyCodeText { get; private set; }
    public Button StartGameButton { get; private set; }
    public GameObject PlayersPanel { get; private set; }
    public GameObject PlayerPanelPrefab { get; private set; }

    private Text[] _playersTexts;
    private bool _hasSetLobbyCode = false;
    private GameController _gameController;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = FindObjectOfType<GameController>();
        if (_gameController == null)
        {
            Debug.LogError("GameController is not here but should be");
            return;
        }
        LoadLobbyUI(_gameController.ExpectedPlayerCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_hasSetLobbyCode)
        {
            bool isValid = _gameController.TryGetLobbyCode(out var code);
            if (isValid)
            {
                LobbyCodeText.text = "Lobby Code: " + code;
                _hasSetLobbyCode = true;
            }
            else
            {
                LobbyCodeText.text = "Lobby Code: loading...";
            }
            return;
        }

        UpdateLobbyUI();
    }


    private void LoadLobbyUI(int players)
    {
        _playersTexts = new Text[players];
        for (int i = 0; i < players; ++i)
        {
            _playersTexts[i] = Instantiate(PlayerPanelPrefab, PlayersPanel.transform).GetComponentInChildren<Text>();
            if (_playersTexts[i] != null)
            {
                _playersTexts[i].text = "Player " + (i + 1) + ": Not Ready";
            }
            else
            {
                print("wat");
            }
            
        }
    }

    private void UpdateLobbyUI()
    {
        int playersReady = _gameController.GetCurrentConnectedPlayers();
        if (playersReady > _playersTexts.Length)
        {
            Debug.LogError("This should not happen. more players Ready then expected");
            return;
        }
        string state;
        if (_gameController.AllPlayersReady())
        {
            StartGameButton.GetComponentInChildren<Text>().text = "Start Game";
            state = "Ready";
        }
        else
        {
            StartGameButton.GetComponentInChildren<Text>().text = "Waiting...";
            state = "Joined";
        }
        for (int i = 0; i < playersReady; ++i)
        {
            _playersTexts[i].text = "Player " + (i + 1) + ": " + state;
        }
    }


    public void StartGameClicked()
    {
        _gameController.StartGame();
    }

    public void ExitLobbyClicked()
    {
        _gameController.GoBackToMainMenu();
    }
}
