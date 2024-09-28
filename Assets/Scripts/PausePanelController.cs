using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanelController : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameController gameController;
    public void ShowPausePanel()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ContiniueGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
    public void backToMenu()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        gameController.BackToMainMenu();
    }
    public void RestartGame()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
