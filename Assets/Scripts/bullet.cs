using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    private GameObject target;
    public float speed;
    private Rigidbody2D bulletRB;
    //private Vector3 targetPosition;
    private float timeCreated;
    public float maxTimeLife;

    // Start is called before the first frame update
    void Start()
    {
        timeCreated = Time.time;
        bulletRB = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player");
        Vector2 moveDir = (target.transform.position - transform.position).normalized * speed;
        bulletRB.velocity = new Vector2(moveDir.x, moveDir.y);

        //Destroy(this.gameObject, 2);
        //targetPosition = FindObjectOfType<playerController>().transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - timeCreated > maxTimeLife)
        {
            Destroy(gameObject);
        }
        //transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        /*if(transform.position == targetPosition)
        {
            Destroy(gameObject);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "ground")
        {
            Destroy(gameObject);
        }
    }

}
