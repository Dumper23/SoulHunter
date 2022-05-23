using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPortal : MonoBehaviour
{

    private bool playerEntered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            playerEntered = true;
        }
    }

    public bool endTP()
    {
        return playerEntered;
    }
}
