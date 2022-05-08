using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class mainMenu : MonoBehaviour
{

    public AudioMixer audioMixer;
    public Dropdown resolutionDropDown;
    public GameObject popUp;
    public GameObject popUp2;
    public EventSystem es;
    public GameObject buttonNo;

    public GameObject buttonPlay;
    public GameObject levelSelection;
    public GameObject quitButton;
    public GameObject optionsButton;
    public GameObject deleteButton;

    public GameObject buttonBack;
    public GameObject buttonLevel1;

    public GameObject levels;

    private bool cheatCode = false;
    private Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        popUp.SetActive(false);
        resolutionDropDown.ClearOptions();

        PlayerData d = PlayerSave.LoadPlayer();
        if(d != null)
        {
            if (d.hasEndedGame)
            {

                cheatCode = true;
            }
        }


        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.M) && !cheatCode)
        {
            cheatCode = true;
            levelSelection.SetActive(true);
        }

        if (cheatCode)
        {
            levelSelection.SetActive(true);
        }
    }

    public void options()
    {
        EventSystem.current.SetSelectedGameObject(buttonBack);
    }

    public void back()
    {
        EventSystem.current.SetSelectedGameObject(buttonPlay);
    }

    public void playGame()
    {
        PlayerData data = PlayerSave.LoadPlayer();
        if (data != null)
        {
            SceneManager.LoadScene(data.currentLevel);
        }
        else
        {
            SceneManager.LoadScene("Intro");
        }
    }

    public void cancelDelete()
    {
        popUp2.SetActive(false);
        EventSystem.current.SetSelectedGameObject(buttonPlay);
    }

    public void deleteSavedGame()
    {
        popUp2.SetActive(true);
        EventSystem.current.SetSelectedGameObject(buttonNo);
    }

    public void confirmDeleteSavedGame()
    {
        popUp.SetActive(true);
        popUp2.SetActive(false);
        Invoke("delete", 1.5f);
    }
    
    private void delete()
    {
        popUp.SetActive(false);
        EventSystem.current.SetSelectedGameObject(buttonPlay);
        PlayerSave.deleteSave();
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void setVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void setQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void setFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void setResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void showLevels()
    {
        levels.SetActive(true);
        buttonPlay.SetActive(false);
        levelSelection.SetActive(false);
        quitButton.SetActive(false);
        optionsButton.SetActive(false);
        deleteButton.SetActive(false);

    EventSystem.current.SetSelectedGameObject(buttonLevel1);
    }

    public void loadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void backLevels()
    {
        levels.SetActive(false);
        buttonPlay.SetActive(true);
        levelSelection.SetActive(true);
        quitButton.SetActive(true);
        optionsButton.SetActive(true);
        deleteButton.SetActive(true);
        EventSystem.current.SetSelectedGameObject(buttonPlay);
    }
}
