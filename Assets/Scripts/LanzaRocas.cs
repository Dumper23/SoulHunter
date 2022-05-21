using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanzaRocas : FatherEnemy
{
    public GameObject ball;
    public float attackRate = 2f;
    public float range = 25f;
    public float force = 5f;
    public bool forceDependingOnDistance = true;
    public float health = 25f;
    public int soulsToGive;
    public GameObject soul;
    public float soulForce = 35f;

    private float time = 0;
    private Transform target;
    private float ballForce = 35f;
    private SpriteRenderer sp;
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        target = FindObjectOfType<playerController>().transform;
    }

    void Update()
    {
        if (target.position.x > transform.position.x)
        {
            sp.flipX = false;
        }
        else
        {
            sp.flipX = true;
        }

            time += Time.deltaTime;
        if(time >= attackRate)
        {
            if ((target.position - transform.position).magnitude <= range)
            {
                time = 0;
                shoot();
            }
        }
    }

    private void dead()
    {
        for (int i = 0; i < soulsToGive; i++)
        {
            GameObject g = Instantiate(soul, transform.position, Quaternion.identity);
            g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * soulForce, ForceMode2D.Impulse);
        }

        Destroy(gameObject);
    }

    private void shoot()
    {
        GameObject b = Instantiate(ball, transform.position, Quaternion.identity);
        if (forceDependingOnDistance)
        {
            ballForce = (target.position - transform.position).magnitude / 1.5f;
            if(ballForce >= 12)
            {
                ballForce = 12;
            }
        }
        else
        {
            ballForce = force;
        }

        if (target.position.x > transform.position.x)
        {
            b.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 0.9f) * ballForce, ForceMode2D.Impulse);
        }
        else
        {
            b.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 0.9f) * ballForce, ForceMode2D.Impulse);
        }
    }


    public override void applyKnockback(float[] position)
    {
    }

    public override void Damage(float[] damageMessage, bool wantKnockback)
    {
        health -= damageMessage[0];
        if(health <= 0)
        {
            dead();
        }
    }

    public override void mostraMissatge()
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
