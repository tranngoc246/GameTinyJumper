using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameoverDialog : Dialog
{
    public TextMeshProUGUI bestScoreText;
    bool m_replayBtnClicked;

    public override void Show(bool isShow)
    {
        base.Show(isShow);

        if (bestScoreText)
        {
            bestScoreText.text = Prefs.bestScore.ToString();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Replay()
    {
        m_replayBtnClicked = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void BackToHome()
    {
        GameUIManager.Ins.ShowGameGui(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        if (m_replayBtnClicked)
        {
            GameUIManager.Ins.ShowGameGui(true);
            GameManager.Ins.PlayGame();
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
