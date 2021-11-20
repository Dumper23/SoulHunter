using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpointsystem : MonoBehaviour
{
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player" && !isActivated)
        {
            PlayerSave.SavePlayer(collision.transform.GetComponent<playerController>());
            Debug.Log("Saved!");
            isActivated = true;
        }
    }
}
