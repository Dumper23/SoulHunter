using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spikeBall : MonoBehaviour
{
    public GameObject deadAudio;

    private AudioSource audio;

    private void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));
        audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject go = Instantiate(deadAudio, transform.position, Quaternion.identity);
        go.GetComponent<AudioSource>().clip = audio.clip;
        go.GetComponent<AudioSource>().volume = 0.05f;
        go.GetComponent<AudioSource>().Play();

    }
}
