using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController
{

    private int _redScore = 0;
    private int _blueScore = 0;

    // Time in seconds.
    private float _time;
    private float _startTime = 20f;

    public ScoreController()
    {

        // Register event for goal scored
        Services.EventManager.Register<GoalScored>(IncrementScore);
        _time = _startTime;
    }

    public void IncrementScore(NEvent e)
    {
        bool blueScored = ((GoalScored)e).blueScored;
        Debug.Log("Goal Scored! in scorecontroller");

        if (blueScored)
        {
            _blueScore++;
        }
        else
        {
            _redScore++;
        }

        Services.UIManager.UpdateScore(_redScore, _blueScore);
    }

    public void UpdateTime()
    {
        if (_time <= 0f) return;
        _time -= Time.deltaTime;

        Services.UIManager.UpdateTime(_time);

        if (_time <= 0f)
        {
            // Fire times up event.
            Services.EventManager.Fire(new TimeUp());
        }
    }

    public string WhoWon()
    {
        if (_blueScore > _redScore)
        {
            return "Blue team won!";
        }
        else if (_blueScore < _redScore)
        {
            return "Red team won!";
        }
        else
        {
            return "It was a tie!";
        }
    }

    public void OnDestroy()
    {
        // Unregister event
        Services.EventManager.Unregister<GoalScored>(IncrementScore);
    }

}
