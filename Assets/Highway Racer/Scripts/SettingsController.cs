using UnityEngine;
using System.Collections;

public class SettingsController : MonoBehaviour
{
    public GameObject pausedMenu;
    public GameObject pausedButtons;
    public GameObject optionsMenu;

    void OnEnable()
    {
        GameHandler.OnPaused += OnPaused;
        GameHandler.OnResumed += OnResumed;
        Invoke("ChangeCamera", 1f);
    }

    public void ResumeGame()
    {
        GameHandler.Instance.Paused();
    }

    public void RestartGame()
    {
        GameHandler.Instance.RestartGame();
    }

    public void MainMenu()
    {
        GameHandler.Instance.MainMenu();
    }

    public void OptionsMenu(bool open)
    {
        if (open)
        {
            optionsMenu.SetActive(true);
            pausedButtons.SetActive(false);
        }
        else
        {
            optionsMenu.SetActive(false);
            pausedButtons.SetActive(true);
        }
    }

    void OnPaused()
    {
        pausedMenu.SetActive(true);
        pausedButtons.SetActive(true);

        AudioListener.pause = true;
        Time.timeScale = 0;
    }

    public void OnResumed()
    {
        pausedMenu.SetActive(false);
        pausedButtons.SetActive(false);

        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    public void ChangeCamera()
    {
        if (GameObject.FindObjectOfType<CarCameraController>())
            GameObject.FindObjectOfType<CarCameraController>().ChangeCamera();
    }

    void OnDisable()
    {
        GameHandler.OnPaused -= OnPaused;
        GameHandler.OnResumed -= OnResumed;
    }
}