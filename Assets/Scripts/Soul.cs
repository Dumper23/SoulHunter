using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    void Start()
    {
        Invoke("goToPlayer", 0.5f);
    }

    void goToPlayer()
    {
        this.transform.tag = "Soul";
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "ground" || collision.transform.tag == "difWall")
        {
            gameObject.GetComponentInChildren<Rigidbody2D>().velocity *= 0.5f;
        } 
    }
}
