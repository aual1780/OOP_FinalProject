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
        gc.CreateLobby(1, "yeet");
    }

    public void QuitClicked()
    {
        Application.Quit();
    }
}
