using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public string keyType;
    
    private DoorSystemManager doorSystem;

    private void Start()
    {
        doorSystem = FindObjectOfType<DoorSystemManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            doorSystem.addKey(keyType);
            Destroy(gameObject);
        }
    }
}
