using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreHandler : MonoBehaviour
{
    GameController _gameController;
    // Start is called before the first frame update
    void Start()
    {
        _gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackClicked()
    {
        if (_gameController != null)
        {
            _gameController.GoBackToMainMenu();
        }
    }
}
