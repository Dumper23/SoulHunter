using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spikeBall : MonoBehaviour
{
    private AudioSource audio;

    private void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));
        audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject go = Instantiate(new GameObject(), transform.position, Quaternion.identity);
        go.AddComponent<AudioSource>();
        go.GetComponent<AudioSource>().clip = audio.clip;
        go.GetComponent<AudioSource>().volume = 0.5f;
        go.GetComponent<AudioSource>().Play();
        go.AddComponent<destroyObject>();
        go.GetComponent<destroyObject>().timeToDestroy = 3f;
    }
}
