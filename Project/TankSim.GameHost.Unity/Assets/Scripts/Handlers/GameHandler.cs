using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{

    private Tank _tank;
    private GameController _gameController;

    public GameObject canvas;
    public GameObject gameOverUIPrefab;

    public int score { get; private set; } = 0;

    private bool hasSpawnedGameOverUI = false;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        _tank = FindObjectOfType<Tank>();
        _gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_tank.health <= 0 && !hasSpawnedGameOverUI)
        {
            hasSpawnedGameOverUI = true;

            Instantiate(gameOverUIPrefab, canvas.transform);
        }
    }


    public void AddPoints(int points)
    {
        if (points <= 0)
        {
            Debug.LogWarning("Trying to reduce score");
            return;
        }
        score += points;
    }

    public void SubmitScoreClicked()
    {
        if (_gameController == null)
        {
            Debug.LogWarning("Unable to submit score. Game controller does not exist");
            return;
        }

        _gameController.GoToHighScoreScene(score);
    }
}
