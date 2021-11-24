using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public int playerPoints;
    public Text pointText;
    public Slider soulBar;
    public int maxPoints = 125;

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
            soulBar.maxValue = maxPoints;
            soulBar.value = playerPoints;
        }
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
        soulBar.value = playerPoints;
    }

    public int getMaxPoints()
    {
        return maxPoints;
    }
}
