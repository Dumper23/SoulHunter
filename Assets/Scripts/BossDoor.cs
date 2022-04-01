using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    public GameObject barriers;
    public GameObject entityToEliminate;
    public GameObject ambientMusic;
    private AudioSource bossMusic;
    public GameObject endBossMusic;

    private void Start()
    {
        if(GetComponent<AudioSource>() != null)
        {
            bossMusic = GetComponent<AudioSource>();
        }
        barriers.SetActive(false);
    }

    private void Update()
    {
        if(entityToEliminate == null && barriers != null)
        {
            if (endBossMusic != null)
            {
                Instantiate(endBossMusic);
            }
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            barriers.SetActive(true);
            if (ambientMusic != null)
            {
                Destroy(ambientMusic);
            }
            if (bossMusic != null)
            {
                bossMusic.Play();
            }
        }
    }

}
