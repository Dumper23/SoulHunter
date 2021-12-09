using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_shooter : MonoBehaviour
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

    private bool isKnockback = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if(distanceFromPlayer < lineOfSite && distanceFromPlayer > shootingRange)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.position, speed * Time.deltaTime);
        }
        else if (distanceFromPlayer < shootingRange && nextFireTime < Time.time )
        {
            Instantiate(bullet, bulletParent.transform.position, Quaternion.identity);
            nextFireTime = Time.time + fireRate;
        }
        if (isKnockback)
        {
            Knockback();
        }
    }
    public void Damage(float[] attackDetails)
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
    public void applyKnockback()
    {
        Knockback();
    }
    private void Knockback()
    {
        knockbackStartTime = Time.time;
        isKnockback = true;
        transform.position = transform.position + new Vector3(damageDirectionX, damageDirectionY, 0.0f) * speedKnockback;
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
}