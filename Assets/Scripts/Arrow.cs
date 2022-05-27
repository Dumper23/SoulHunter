using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 1f;
    public Vector3 direction;
    public float lifeTime = 5f;
    public GameObject spriteToRotate;
    public float rotationSpeed;
    public bool isRotating = true;

    private Rigidbody2D rb;
    private Collider2D c;
    private void Start()
    {
        if (isRotating)
        {
            c = GetComponent<Collider2D>();
            c.enabled = false;
            Invoke("enableCollision", 0.07f);
        }
        Destroy(gameObject, lifeTime);
        if (GetComponentInChildren<Animator>())
        {
            GetComponentInChildren<Animator>().enabled = true;
            GetComponentInChildren<Animator>().Play("arrow");
        }
    } 

    void enableCollision()
    {
        c.enabled = true;
    }


    void Update()
    {
        transform.Translate(((direction + transform.position) - transform.position).normalized * Time.deltaTime * speed);
        if (isRotating)
        {
            spriteToRotate.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "ground")
        {
            Destroy(this.gameObject);
        }

        if (collision.tag == "Player")
        {
            collision.GetComponent<playerController>().takeDamage();
            if (!isRotating)
            {
                Destroy(gameObject);
            }
        }

        if (collision.tag == "Player" || collision.tag == "ground" || collision.tag == "Mushroom")
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "ground")
        {
            Destroy(this.gameObject);
        }

    }
}
