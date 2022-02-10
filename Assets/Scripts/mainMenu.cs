using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class mainMenu : MonoBehaviour
{

    public AudioMixer audioMixer;
    public Dropdown resolutionDropDown;
    public GameObject popUp;

    private Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        popUp.SetActive(false);
        resolutionDropDown.ClearOptions();

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

    public void playGame()
    {
        PlayerData data = PlayerSave.LoadPlayer();
        if (data != null)
        {
            SceneManager.LoadScene(data.currentLevel);
        }
        else
        {
            SceneManager.LoadScene("Tutorial");
        }
    }

    public void deleteSavedGame()
    {
        popUp.SetActive(true);
        Invoke("delete", 1.5f);
    }

    private void delete()
    {
        popUp.SetActive(false);
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
}
