using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulV2 : MonoBehaviour
{
    [SerializeField]
    private GameObject soul;

    [SerializeField]
    private float soulForce;

    void Start()
    {
        Invoke("goToPlayer", 0.2f);
    }

    void goToPlayer()
    {
        this.transform.tag = "SoulV2";
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "ground" || collision.transform.tag == "difWall")
        {
            gameObject.GetComponentInChildren<Rigidbody2D>().velocity *= 0.5f;
        }
        if (collision.transform.tag == "Enemy")
        {
            GameObject g = Instantiate(soul, collision.transform.position, Quaternion.identity);
            g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * soulForce, ForceMode2D.Impulse);
            Destroy(this.gameObject);
        }
    }
}
