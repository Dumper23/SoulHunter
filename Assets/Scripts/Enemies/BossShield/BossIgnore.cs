using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIgnore : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "BossIgnore")
        {
            Physics2D.IgnoreCollision(transform.GetComponent<Collider2D>(), collision.transform.GetComponent<Collider2D>());
        }
    }
}
