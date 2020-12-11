using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    private GameHandler _gameHandler;
    private Text _text;

    // Start is called before the first frame update
    void Start()
    {
        _gameHandler = FindObjectOfType<GameHandler>();
        _text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameHandler != null)
        {
            _text.text = "Score: " + _gameHandler.Score;
        }
    }
}
