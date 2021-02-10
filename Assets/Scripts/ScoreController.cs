using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController
{
    private Text _redScoreText;
    private Text _blueScoreText;

    private int _redScore = 0;
    private int _blueScore = 0;

    public Text _timeText { get; private set; }

    // Time in seconds.
    private float _time;
    private float _startTime = 60f;

    public ScoreController(Text red, Text blue, Text time)
    {
        _time = _startTime;
        _redScoreText = red;
        _blueScoreText = blue;
        _timeText = time;

        // Register event for goal scored
        Services.EventManager.Register<GoalScored>(IncrementScore);

    }

    public void IncrementScore(NEvent e)
    {
        //bool blueScored = ((GoalScored)e).blueScored;

        if (true)//blueScored)
        {
            _blueScore++;
        }
        else
        {
            _redScore++;
        }

        UpdateScore();
    }

    public void UpdateTime()
    {
        if (_time <= 0f) return;
        _time -= Time.deltaTime;
        _timeText.text = _time.ToString();

        if (_time <= 0f)
        {
            // Fire times up event.
        }
    }

    private void UpdateScore()
    {
        _redScoreText.text = _redScore.ToString();
        _blueScoreText.text = _blueScore.ToString();
    }

    public void OnDestroy()
    {
        // Unregister event
        Services.EventManager.Unregister<GoalScored>(IncrementScore);
    }

}
