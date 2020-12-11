using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private Tank _tank;
    private GameController _gameController;

    public GameObject canvas;
    public GameObject gameOverUIPrefab;

    public int Score { get; private set; } = 0;

    private bool _hasSpawnedGameOverUI = false;

    // Start is called before the first frame update
    void Start()
    {
        Score = 0;
        _tank = FindObjectOfType<Tank>();
        _gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_tank.Health <= 0 && !_hasSpawnedGameOverUI)
        {
            _hasSpawnedGameOverUI = true;

            _ = Instantiate(gameOverUIPrefab, canvas.transform);
        }
    }


    public void AddPoints(int points)
    {
        if (points <= 0)
        {
            Debug.LogWarning("Trying to reduce score");
            return;
        }
        Score += points;
    }

    public void SubmitScoreClicked()
    {
        if (_gameController == null)
        {
            Debug.LogWarning("Unable to submit score. Game controller does not exist");
            return;
        }

        _gameController.GoToHighScoreScene(Score);
    }
}
