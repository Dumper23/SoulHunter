using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreOtherHearts : MonoBehaviour
{
    private Collider2D col,
        playerCol;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        playerCol = FindObjectOfType<playerController>().gameObject.GetComponent<Collider2D>();

    }

    private void Update()
    {
        Physics2D.IgnoreCollision(col, playerCol);
    }
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<HeartDemonBehaviour>())
        {
            Physics2D.IgnoreCollision(col, collision.transform.GetComponentInChildren<IgnoreOtherHearts>().gameObject.GetComponent<Collider2D>());
        }
        if (collision.transform.tag == "Player")
        {
            Physics2D.IgnoreCollision(col, collision.gameObject.GetComponent<Collider2D>());
        }
    }
}
