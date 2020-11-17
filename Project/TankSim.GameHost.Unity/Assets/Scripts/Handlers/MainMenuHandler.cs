using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{

    public InputField gameNameIF;
    public InputField playerAmountIF;

    private GameController _gameController;


    // Start is called before the first frame update
    void Start()
    {
        _gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void CreateLobbyClicked()
    {

        string gameName = gameNameIF.text;
        string playerStr = playerAmountIF.text;


        if (!int.TryParse(playerStr, out var playerCount))
        {
            Debug.LogError("player count: [" + playerStr + "] is not a number");
            playerAmountIF.text = "";
            return;
        }

        //check if player count is out of range
        if (playerCount < 1 || playerCount > 6)
        {
            playerAmountIF.text = "";
            return;
        }

        //check if game name is empty
        if (gameName == "")
        {
            return;
        }

        _gameController.CreateLobby(playerCount, gameName);
    }

    public void QuitClicked()
    {
        Application.Quit();
    }
}
