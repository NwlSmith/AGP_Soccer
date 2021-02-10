using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    private List<Text> _allTexts = new List<Text>();
    public Text timeText { get; private set; }
    public Text redScoreText { get; private set; }
    public Text blueScoreText { get; private set; }
    public Text startText { get; private set; }
    public Text pauseText { get; private set; }
    public Text gameOverText { get; private set; }
    public Text gameOverSubtitleText { get; private set; }

    public UIManager()
    {
        timeText = Services.SceneObjectIndex.timeText;
        _allTexts.Add(timeText);
        redScoreText = Services.SceneObjectIndex.redScoreText;
        _allTexts.Add(redScoreText);
        blueScoreText = Services.SceneObjectIndex.blueScoreText;
        _allTexts.Add(blueScoreText);
        startText = Services.SceneObjectIndex.startText;
        _allTexts.Add(startText);
        pauseText = Services.SceneObjectIndex.pauseText;
        _allTexts.Add(pauseText);
        gameOverText = Services.SceneObjectIndex.gameOverText;
        _allTexts.Add(gameOverText);
        gameOverSubtitleText = Services.SceneObjectIndex.gameOverSubtitleText;
        _allTexts.Add(gameOverSubtitleText);


        Services.EventManager.Register<PauseEvent>(Pause);
        Services.EventManager.Register<UnpauseEvent>(Unpause);
    }

    public void OnDestroy()
    {
        // Unregister!
        Services.EventManager.Unregister<PauseEvent>(Pause);
        Services.EventManager.Unregister<UnpauseEvent>(Unpause);
    }

    private void HideUI()
    {
        foreach (Text text in _allTexts)
        {
            text.enabled = false;
        }
    }

    #region UI display and hiding
    public void StartMenu()
    {
        HideUI();
        startText.enabled = true;
    }

    public void StartPlay()
    {
        HideUI();
        timeText.enabled = true;
        redScoreText.enabled = true;
        blueScoreText.enabled = true;
    }

    public void Pause(NEvent e)
    {
        HideUI();
        pauseText.enabled = true;
    }

    public void Unpause(NEvent e)
    {
        StartPlay();
    }

    public void GameOver()
    {
        HideUI();
        gameOverText.enabled = true;
        gameOverSubtitleText.enabled = true;

        gameOverSubtitleText.text = Services.ScoreController.WhoWon();
    }
    #endregion

    public void UpdateTime(float time)
    {
        timeText.text = time.ToString();
    }

    public void UpdateScore(int redScore, int blueScore)
    {
        redScoreText.text = redScore.ToString();
        blueScoreText.text = blueScore.ToString();
    }
}
