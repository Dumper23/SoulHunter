using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIgnore : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "BossIgnore")
        {
            foreach (Collider2D c in transform.GetComponents<Collider2D>())
            {
                Physics2D.IgnoreCollision(c, collision.transform.GetComponent<Collider2D>());
            }
        }
    }
}
