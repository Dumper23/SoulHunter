using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public int playerPoints;
    public Text pointText;

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
        }
    }

    public void addPoints(int points)
    {
        playerPoints += points;
        pointText.text = playerPoints.ToString();
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

}
