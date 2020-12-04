using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreHandler : MonoBehaviour
{
    GameController _gameController;

    private Text _userScoreText;
    private Text _scoreTypeText;
    private HighScoresPanel _highScoresPanel;

    private bool _hasSetText = false;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = FindObjectOfType<GameController>();
        InitVariables();

    }


    private void InitVariables()
    {
        Canvas canvas = FindObjectOfType<Canvas>();

        Text[] sceneText = canvas.GetComponentsInChildren<Text>();
        foreach (var text in sceneText)
        {
            if (text.name == "ScoreTypeText")
            {
                _scoreTypeText = text;
            }
            else if (text.name == "ScoreText")
            {
                _userScoreText = text;
            }
        }

        _highScoresPanel = canvas.GetComponentInChildren<HighScoresPanel>();
    }

    private void SetText()
    {
        int score = _gameController.Score;
        if (score < 0)
        {
            _scoreTypeText.text = "";
            _userScoreText.text = "";
        }
        else
        {
            string teamName = "";
            foreach(var name in _gameController.PlayerNames)
            {
                teamName += name + ", ";
            }
            teamName = teamName.Remove(teamName.Length - 2, 2);

            if (_highScoresPanel.CheckScore(score, teamName))
            {
                _scoreTypeText.text = "New High Score:";
            }
            else
            {
                _scoreTypeText.text = "Your Score:";
            }
            _userScoreText.text = score.ToString();
        }

        
    }


    // Update is called once per frame
    void Update()
    {
        if (!_hasSetText)
        {
            _hasSetText = true;
            //making sure panel's start has run is ready
            SetText();
        }
    }

    public void BackClicked()
    {
        if (_gameController != null)
        {
            _gameController.GoBackToMainMenu();
        }
    }
}
