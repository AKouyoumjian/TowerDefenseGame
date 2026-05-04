using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuBehavior : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public AudioClip pauseSFX;

    bool isGamePaused = false;

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

            // play pauseSFX
            if (pauseSFX)
            {
                AudioSource.PlayClipAtPoint(pauseSFX, Camera.main.transform.position);
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

    public void ExitGame()
    {
        Debug.Log("Exiting the Game");
        Application.Quit();
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
