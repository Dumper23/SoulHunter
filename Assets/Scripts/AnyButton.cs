using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnyButton : MonoBehaviour
{

    public string levelToLoad = "MainMenu";

    void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene(levelToLoad);
        }
    }
}
