using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmitScoreButton : MonoBehaviour
{
    private GameHandler _gameHandler;
    void Awake()
    {
        _gameHandler = FindObjectOfType<GameHandler>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(_gameHandler.SubmitScoreClicked);
    }
}
