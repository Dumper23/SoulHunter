using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class auidoOnContact : MonoBehaviour
{
    public AudioClip audioToPlay;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            GetComponent<AudioSource>().clip = audioToPlay;
            GetComponent<AudioSource>().Play();
        }
    }
}
