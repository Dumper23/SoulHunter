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

    private Rigidbody2D rb;
    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }


    void Update()
    {
        transform.Translate(((direction + transform.position) - transform.position).normalized * Time.deltaTime * speed);
        spriteToRotate.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponent<playerController>().takeDamage();
        }

        if (collision.tag == "Player" || collision.tag == "ground" || collision.tag == "Mushroom")
        {
            Destroy(gameObject);
        }
    }
}
