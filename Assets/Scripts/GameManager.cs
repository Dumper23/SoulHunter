using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public int playerPoints;
    public Text pointText;
    //public Slider soulBar;
    public int maxPoints = 125;

    [Header("Pause Menu")]
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public string mainMenuName = "MainMenu";
    public GameObject buttonToSelect;

    private bool inventory = false;


    public static GameManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            //soulBar.maxValue = maxPoints;
            //soulBar.value = playerPoints;
        }
    }


    public string getCurrentLevelName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void addPoints(int points)
    {
        if (playerPoints + points < 125)
        {
            playerPoints += points;
            pointText.text = playerPoints.ToString();
        }
        else
        {
            playerPoints = 125;
            pointText.text = playerPoints.ToString();
        }
    }

    public void nextLevel(string nl, playerController playerController)
    {
        playerController.setCurrentLevelName(nl);
        PlayerSave.SavePlayer(playerController);
        SceneManager.LoadScene(nl);
    }

    public void loadPoints(int p)
    {
        playerPoints = p;
        pointText.text = playerPoints.ToString();
    }

    public int getPoints()
    {
        return playerPoints;
    }

    private void Update()
    {
        //soulBar.value = playerPoints;
        if (Input.GetButtonDown("Pause") && !inventory)
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

    public void changeInventory(bool b)
    {
        inventory = b;
    }

    public bool isPaused()
    {
        return GameIsPaused;
    }

    public int getMaxPoints()
    {
        return maxPoints;
    }

    #region Pause Menu
    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
    }

    void Pause()
    {
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
        Time.timeScale = 0f;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
        SceneManager.LoadScene(mainMenuName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
