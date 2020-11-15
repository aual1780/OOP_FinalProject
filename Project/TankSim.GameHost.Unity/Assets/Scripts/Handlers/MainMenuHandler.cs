using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{

    public InputField gameNameIF;
    public InputField playerAmountIF;

    private GameController gc;


    // Start is called before the first frame update
    void Start()
    {
        gc = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CreateLobbyClicked()
    {

        string gameName = gameNameIF.text;
        string playerStr = playerAmountIF.text;

        int playerCount = 0;
        try
        {
            playerCount = Int32.Parse(playerStr);
        }
        catch (FormatException)
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

        gc.CreateLobby(playerCount, gameName);
    }

    public void QuitClicked()
    {
        Application.Quit();
    }
}
