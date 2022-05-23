using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulV3 : MonoBehaviour
{
    void Start()
    {
        Invoke("goToPlayer", 0.2f);
    }

    void goToPlayer()
    {
        this.transform.tag = "SoulV3";
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "ground" || collision.transform.tag == "difWall")
        {
            gameObject.GetComponentInChildren<Rigidbody2D>().velocity *= 0.5f;
        }
        if (collision.transform.tag == "Spirit")
        {
            Destroy(this.gameObject);
        }
    }
}
