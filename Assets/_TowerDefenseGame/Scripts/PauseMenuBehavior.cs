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
        // SceneManager.LoadScene(0);
        // just reload this scene for now
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting the Game");
        Application.Quit();

        // just reload this scene for now
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
