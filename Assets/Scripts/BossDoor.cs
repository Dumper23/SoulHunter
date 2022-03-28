using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    public GameObject barriers;
    public GameObject entityToEliminate;

    private void Start()
    {
        barriers.SetActive(false);
    }

    private void Update()
    {
        if(entityToEliminate == null && barriers != null)
        {
            Destroy(barriers);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            barriers.SetActive(true);
        }
    }

}
