using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class auidoOnContact : MonoBehaviour
{
    public AudioClip audioToPlay;
    public float bounceForce = 500f;
    public Vector2 directionToBounce = Vector2.up; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            collision.collider.GetComponent<Rigidbody2D>().AddForce(directionToBounce * bounceForce);
            GetComponent<AudioSource>().clip = audioToPlay;
            GetComponent<AudioSource>().Play();
        }
    }
}
