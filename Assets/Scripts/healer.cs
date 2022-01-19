using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healer : MonoBehaviour
{

    private bool playerDetected = false;
    private Vector2 playerPos;
    private Rigidbody2D rb;
    private float direction = 1;
    private bool wallDetected = false;
    
    public float movementSpeed = 4f;
    public float detectionRange;
    public float timeToDie = 5f;
    public float health = 30f;
    public float jumpForce = 200f;
    public float wallDetectionDistance = 1f;
    public LayerMask whatIsGround;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!playerDetected)
        {
            Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, detectionRange, LayerMask.GetMask("Player"));
            if (playerInRange != null && playerInRange.tag == "Player")
            {
                playerDetected = true;
            }
        }
        else 
        { 
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            Vector2 movement = Vector2.zero;

            wallDetected = Physics2D.Raycast(transform.position, (playerPos.x > transform.position.x ? -transform.right : transform.right), wallDetectionDistance, whatIsGround);
            Debug.Log(wallDetected);

            if (!wallDetected)
            {
                movement.Set(movementSpeed * ((playerPos.x > transform.position.x) ? -direction : direction), rb.velocity.y);
                rb.velocity = movement;
            }

            if (rb.velocity.y == 0 && Mathf.Abs(rb.velocity.x) > 0.6 && !wallDetected)
            {
                rb.AddForce(new Vector2(0, (Random.Range(0, 10) >= 9 ? jumpForce : 1)));
            }
            //Animacio de desapareixer abans de eliminarse 
            Destroy(gameObject, timeToDie);
        }
    }

    public void Damage(float damage)
    {
        if(health - damage > 0)
        {
            health = health - damage;
        }
        else
        {
            die();
        }
    }

    private void die()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>().addLive();
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
