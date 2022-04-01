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
    public float health = 1f;
    public float jumpForce = 200f;
    public float wallDetectionDistance = 1f;
    public LayerMask whatIsGround;

    private AudioSource audioSource;
    private bool alreadySound = false;
    public List<AudioClip> audios;
    public GameObject deadSoundObject;

    private const int INRANGE_SOUND = 0;
    private const int DEAD_SOUND = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));
    }

    void Update()
    {
        if (!playerDetected)
        {
            Collider2D playerInRange = Physics2D.OverlapCircle(transform.position, detectionRange, LayerMask.GetMask("Player"));
            if (playerInRange != null && playerInRange.tag == "Player")
            {
                playerDetected = true;
                if (!alreadySound)
                {
                    audioSource.clip = audios[INRANGE_SOUND];
                    audioSource.Play();
                    alreadySound = true;
                }
            }
            else
            {
                alreadySound = false;
            }
        }
        else 
        {
            GameObject player = null;
            if (GameObject.FindObjectOfType<playerController>() != null)
            {
                player = GameObject.FindObjectOfType<playerController>().gameObject;
            }

            if (player != null)
            {
                playerPos = player.transform.position;
                Vector2 movement = Vector2.zero;

                wallDetected = Physics2D.Raycast(transform.position, (playerPos.x > transform.position.x ? -transform.right : transform.right), wallDetectionDistance, whatIsGround);

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
    }

    public void Damage(float damage, playerController p)
    {
        if(health - damage > 0)
        {
            health = health - damage;
        }
        else
        {
            die(p);
        }
    }

    private void die(playerController p)
    {
        deadSoundObject.GetComponent<AudioSource>().clip = audios[DEAD_SOUND];
        Instantiate(deadSoundObject, transform.position, transform.rotation);
        p.addLive();
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Trap")
        {
            deadSoundObject.GetComponent<AudioSource>().clip = audios[DEAD_SOUND];
            Instantiate(deadSoundObject, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
