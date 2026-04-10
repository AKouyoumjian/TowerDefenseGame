using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuBehavior : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    bool isGamePaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // remove wave announcement when paused, it was causing issues with pause buttons
            WaveSpawner spawner = FindFirstObjectByType<WaveSpawner>();
            if (spawner)
            {
                spawner.ClearWaveAnnouncement();
            }

            // toggle pause
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }

        }
    }

    public void ResumeGame()
    {
        Debug.Log("Resuming the Game");
        isGamePaused = false;
        Time.timeScale = 1f; // unfreeze game
        if (pauseMenuPanel)
        {
            // hide panel
            pauseMenuPanel.SetActive(false);
        }
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f; // freeze game
        if (pauseMenuPanel)
        {
            // show panel
            pauseMenuPanel.SetActive(true);
        }
    }

    public void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu Scene");
        // find WaveSpawner and call reset

        RestartGame();
    }

    public void ExitGame()
    {
        Debug.Log("Exiting the Game");
        // Application.Quit();

        RestartGame();
    }

    public void RestartGame()
    {
        Debug.Log("Restarting the Game");

        if (MoneyManager.Instance)
        {
            MoneyManager.Instance.ResetMoney();
        }

        // find WaveSpawner and call reset
        WaveSpawner spawner = FindFirstObjectByType<WaveSpawner>();
        if (spawner)
        {
            spawner.ResetWaveSpawner();
        }

        ResumeGame();

        // just reload this scene for now
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
