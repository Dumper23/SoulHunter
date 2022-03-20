using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string doorType;
    public GameObject infoPanel;

    private DoorSystemManager doorManager;

    private void Start()
    {
        doorManager = FindObjectOfType<DoorSystemManager>(); 
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            infoPanel.SetActive(true);
            if (Input.GetButton("Interact"))
            {
                if (doorManager.openDoor(doorType))
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            infoPanel.SetActive(false);
        }
    }
}
