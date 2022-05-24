using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireSpirit : MonoBehaviour
{
    public float jumpForce = 1f;

    private float yPos = 0;
    private Rigidbody2D rb;
    private bool appliedForce = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        yPos = transform.position.y;
    }

    void Update()
    {
        if (transform.position.y < yPos)
        {
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            rb.velocity = Vector2.zero;
        }
        if(transform.position.y == yPos)
        {
            applyForce();
        }
    }

    private void applyForce()
    {
        rb.AddForce(transform.up * jumpForce * Time.deltaTime, ForceMode2D.Impulse);
    }
}
