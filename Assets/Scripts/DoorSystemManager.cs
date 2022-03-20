using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorSystemManager : MonoBehaviour
{

    [Header("Keys")]
    public bool greenKey;
    public bool blueKey;
    public bool orangeKey;
    public bool redKey;

    [Header("UI")]
    public Image greenKeyImg; 
    public Image blueKeyImg; 
    public Image orangeKeyImg; 
    public Image redKeyImg;

    private Color notFound, found;

    private void Start()
    {
        notFound = new Color();
        notFound.r = 255;
        notFound.g = 255;
        notFound.b = 255;
        notFound.a = 0.25f;

        found = new Color();
        found.r = 255;
        found.g = 255;
        found.b = 255;
        found.a = 1f;
    }

    private void Update()
    {
        if (greenKey)
        {
            greenKeyImg.color = found;
        }
        else
        {
            greenKeyImg.color = notFound;
        }

        if (blueKey)
        {
            blueKeyImg.color = found;
        }
        else
        {
            blueKeyImg.color = notFound;
        }

        if (orangeKey)
        {
            orangeKeyImg.color = found;
        }
        else
        {
            orangeKeyImg.color = notFound;
        }

        if (redKey)
        {
            redKeyImg.color = found;
        }
        else
        {
            redKeyImg.color = notFound;
        }
    }

    public void addKey(string keyType)
    {
        switch (keyType)
        {
            case "greenKey":
                greenKey = true;
                break;
            case "blueKey":
                blueKey = true;
                break;
            case "orangeKey":
                orangeKey = true;
                break;
            case "redKey":
                redKey = true;
                break;
        }
    }

    public bool openDoor(string doorType)
    {
        switch (doorType)
        {
            case "greenDoor":
                return greenKey;
            case "blueDoor":
                return blueKey;
            case "orangeDoor":
                return orangeKey;
            case "redDoor":
                return redKey;
            default:
                return false;
        }
    }
}
