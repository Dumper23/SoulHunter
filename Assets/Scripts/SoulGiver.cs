using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulGiver : MonoBehaviour
{
    public GameObject soul;
    public float soulForce;
    public int soulsToGive = 5;
    public int health = 2;

    void Update()
    {
        if(health <= 0)
        {
            for (int i = 0; i <= soulsToGive; i++)
            {
                GameObject g = Instantiate(soul, transform.position, transform.rotation);
                g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * soulForce, ForceMode2D.Impulse);
            }
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            health--;
        }
    }
}
