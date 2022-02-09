using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_shooter : FatherEnemy
{

    public int damageToPlayer = 1;
    public int pointsToGive = 10;

    public float
        speed,
        speedKnockback,
        lineOfSite,
        shootingRange,
        maxHealth,
        knockbackDuration;        
    public float fireRate = 1;

    public GameObject bullet;
    public GameObject bulletParent;
    private Transform player;
    private float 
        currentHealth,
        nextFireTime,
        knockbackStartTime;
    private float[] posPlayerForKnockback;


    [SerializeField]
    private GameObject
        hitParticle,
        deathChunkParticle,
        deathBloodParticle;

    [SerializeField]
    private ParticleSystem particleDamage;

    private int 
        damageDirectionX,
        damageDirectionY;
    private Rigidbody2D rb;

    private bool isKnockback = false;
    private bool inRange = false;

    // Start is called before the first frame update
    void Start()
    {
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if(distanceFromPlayer < lineOfSite && distanceFromPlayer > shootingRange)
        {
            inRange = true;
            transform.position = Vector2.MoveTowards(this.transform.position, player.position, speed * Time.deltaTime);
        }
        else if ((distanceFromPlayer < shootingRange && nextFireTime < Time.time) && player.gameObject.activeInHierarchy)
        {
            inRange = true;
            Instantiate(bullet, bulletParent.transform.position, Quaternion.identity);
            nextFireTime = Time.time + fireRate;
        }
        if (distanceFromPlayer > lineOfSite)
        {
            rb.velocity = Vector2.zero;
            inRange = false;
        }
        if (isKnockback)
        {
            Knockback();
        }
    }
    public override void Damage(float[] attackDetails)
    {

        currentHealth -= attackDetails[0];

        //Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        particleDamage.Play();

        if (attackDetails[1] > transform.position.x)
        {
            damageDirectionX = -1;
        }
        else
        {
            damageDirectionX = 1;
        }

        if (attackDetails[2] > transform.position.y)
        {
            damageDirectionY = -1;
        }
        else
        {
            damageDirectionY = 1;
        }


        //Hit particle

        if (currentHealth > 0.0f)
        {
            Knockback();
        }
        else if (currentHealth <= 0.0f)
        {
            Dead();
            GameManager.Instance.addPoints(pointsToGive);
        }
    }
    public override void applyKnockback(float[] position)
    {
        posPlayerForKnockback = new float[3];
        posPlayerForKnockback[0] = position[0];
        posPlayerForKnockback[1] = position[1];
        posPlayerForKnockback[2] = position[2];
        Knockback();
    }
    private void Knockback()
    {
        knockbackStartTime = Time.time;
        isKnockback = true;
        if (null != posPlayerForKnockback)
        {
            if (posPlayerForKnockback[0] == 1.0f)
            {

                if (posPlayerForKnockback[1] > transform.position.x)
                {
                    damageDirectionX = -1;
                }
                else
                {
                    damageDirectionX = 1;
                }

                if (posPlayerForKnockback[2] > transform.position.y)
                {
                    damageDirectionY = -1;
                }
                else
                {
                    damageDirectionY = 1;
                }
                posPlayerForKnockback[0] = 0.0f;
            }
        }
        if (inRange)
        {
            transform.position = transform.position + new Vector3(damageDirectionX, damageDirectionY, 0.0f) * speedKnockback;
        }
        else
        {
            isKnockback = false;
        }
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            isKnockback = false;
        }
    }
    private void Dead()
    {
        //Spawn chunks and blood
        Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle, transform.position, deathBloodParticle.transform.rotation);
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSite);
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }

    public override void mostraMissatge()
    {
        Debug.Log("EEEEEEIIII2");
    }
}