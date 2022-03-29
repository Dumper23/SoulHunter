using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatChallenge : MonoBehaviour
{
    public GameObject barriers;
    public LayerMask enemyLayer;

    private List<GameObject> entityToEliminate = new List<GameObject>();
    private int entities = 0;

    private void Start()
    {
        barriers.SetActive(false);
        entities = 0;
        
    }

    private void Update()
    {
        entities = (Physics2D.OverlapBoxAll(transform.position, GetComponent<BoxCollider2D>().size, 0f, enemyLayer)).Length;
        if(entities <= 0 && barriers != null)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            barriers.SetActive(true);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider2D>().size);
    }
}
