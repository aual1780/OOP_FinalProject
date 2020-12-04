using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Utf8Json;

public class HighScoresPanel : MonoBehaviour
{

    private List<Text> _scoreTexts = new List<Text>();
    private List<Text> _nameTexts = new List<Text>();
    private List<Image> _panels = new List<Image>();

    private ScoreData[] scores = new ScoreData[10];

    private const string _fileName = "Scores.json";
    private string _scoreFilePath;

    // Start is called before the first frame update
    void Start()
    {
        _scoreFilePath = Application.dataPath + "/" + _fileName;
        InitVariables();
        LoadScoreFile();
    }

    void InitVariables()
    {
        Image[] panels = GetComponentsInChildren<Image>();
        foreach (Image img in panels)
        {
            if (img.name == name)
            {
                //ignore parent
                continue;
            }
            _panels.Add(img);
            Text[] texts = img.GetComponentsInChildren<Text>();
            foreach(Text text in texts)
            {
                if (text.name == "NameText")
                {
                    _nameTexts.Add(text);
                }
                else if (text.name == "ScoreText")
                {
                    _scoreTexts.Add(text);
                }
            }
        }

        for (int i = 0; i < scores.Length; ++i)
        {
            scores[i] = new ScoreData("Scrub Lord", 0);
        }
    }

    private void LoadScoreFile()
    {
        ScoreDatas data = null;
        using (FileStream SourceStream = File.Open(_scoreFilePath, FileMode.OpenOrCreate))
        {
            if (new FileInfo(_scoreFilePath).Length != 0)
            {
                data = JsonSerializer.Deserialize<ScoreDatas>(SourceStream);
            }
        }


        for (int i = 0; i < scores.Length; ++i)
        {
            if (data != null && data.ScoreData.Length > i)
            {
                scores[i] = new ScoreData(data.ScoreData[i]);
            }
            else
            {
                scores[i] = new ScoreData("Scrub Lord", 0);
            }
        }
        LoadUIText(-1);
    }

    private void LoadUIText(int userPlacement)
    {
        for (int i = 0; i < _scoreTexts.Count && i < scores.Length; ++i)
        {
            _scoreTexts[i].text = scores[i].Score.ToString();
            _nameTexts[i].text = scores[i].Name + ":";
            if (userPlacement == i)
            {
                _panels[i].color = new Color(1, 0.843137f, 0);
            }
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    //checks if the score is a new high score
    //returns true 
    public bool CheckScore(int score, string name)
    {
        ScoreData playerScore = new ScoreData(name, score);

        bool newHighScore = false;
        int index = -1;

        ScoreData tmpScore = null;
        for (int i = 0; i < scores.Length; ++i)
        {
            if (playerScore.Score > scores[i].Score)
            {
                if (i == 0)
                {
                    newHighScore = true;
                }
                if (tmpScore == null)
                {
                    index = i;
                    tmpScore = scores[i];
                    scores[i] = playerScore;
                }
                else
                {
                    ScoreData tmp = scores[i];
                    scores[i] = tmpScore;
                    tmpScore = tmp;
                }
            }
        }
        SaveScoreFile();
        LoadUIText(index);
        return newHighScore;
    }

    private void SaveScoreFile()
    {
        using (FileStream SourceStream = File.Open(_scoreFilePath, FileMode.Open))
        {
            ScoreDatas data = new ScoreDatas(scores);
            JsonSerializer.Serialize(SourceStream, data);
        }
    }

}

[Serializable]
public class ScoreDatas
{
    public ScoreDatas()
    {

    }
    public ScoreDatas(ScoreData[] scoreDatas)
    {
        ScoreData = scoreDatas;
    }
    public ScoreData[] ScoreData;
}

[Serializable]
public class ScoreData
{
    public ScoreData()
    {
        Name = "Scrub Lord";
        Score = 0;
    }
    public ScoreData(ScoreData data)
    {
        Name = data.Name;
        Score = data.Score;
    }
    public ScoreData(string name, int score)
    {
        Name = name;
        Score = score;
    }
    public string Name;
    public int Score;
}
