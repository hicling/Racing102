using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool GameIsFinnish = false;
    public GameObject pauseMenuUI;
    public GameObject racePanelUI;
    public GameObject finnishMenuUI;

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !GameIsFinnish)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        racePanelUI.SetActive(true);
        finnishMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        racePanelUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Finnish()
    {
        finnishMenuUI.SetActive(true);
        racePanelUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsFinnish = true;
    }

    public void LoadMenu()
    {
        GameIsFinnish = false;
        GameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("quit!");
        Application.Quit();
    }

}
