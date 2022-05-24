using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : FatherEnemy
{
    public GameObject arrow;
    public Transform firePoint;
    public float attackRate = 1f;
    public float range = 6f;
    public float maxHealth = 25f;
    public int soulsToGive = 10;
    public GameObject soul;
    public float soulForce = 35;
    
    public List<AudioClip> audios;
    public GameObject deadSoundObject;

    private const int DAMAGE_SOUND = 0;
    private const int DEAD_SOUND = 1;

    private AudioSource audioSource;
    private Animator anim;
    private float time;
    private Transform target;
    private float currentHealth;

    [SerializeField]
    private GameObject
        hitParticle,
        deathChunkParticle,
        deathBloodParticle;

    [SerializeField]
    private ParticleSystem particleDamage;

    private int
        facingDirection,
        damageDirection;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        target = FindObjectOfType<playerController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (target.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }

        if((target.position - transform.position).magnitude >= range)
        {
            anim.Play("idle");
        }

        if (time > attackRate)
        {
            if ((target.position - transform.position).magnitude < range)
            {
                anim.Play("attack");
                Invoke("shoot", 0.6f);
                time = 0;
            }
        }
        
    }

    private void shoot()
    {
        GameObject projectile = Instantiate(arrow, firePoint.position, firePoint.rotation);
        
        float angle = Mathf.Atan2((target.position - transform.position).normalized.y, (target.position - transform.position).normalized.x) * Mathf.Rad2Deg;
        projectile.GetComponentInChildren<SpriteRenderer>().gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        projectile.GetComponent<Arrow>().speed = 15;
        projectile.GetComponent<Arrow>().lifeTime = 5;
        projectile.GetComponent<Arrow>().direction = (target.position - transform.position).normalized;


    }

    public override void applyKnockback(float[] position)
    {
    }

    public override void Damage(float[] attackDetails, bool wantKnockback)
    {
        if (isDemon)
        {
            currentHealth -= attackDetails[0] / 3;
        }
        else if (hasShield)
        {
            currentHealth -= attackDetails[0] / 2;
        }
        else
        {
            currentHealth -= attackDetails[0];
        }
        audioSource.clip = audios[DAMAGE_SOUND];
        audioSource.Play();
        //Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        particleDamage.Play();
        if (attackDetails[1] > transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        //Hit particle

        if (currentHealth <= 0.0f)
        {
            die();
        }
    }

    public override void mostraMissatge()
    {
    }


    private void die()
    {
        //Spawn chunks and blood
        deadSoundObject.GetComponent<AudioSource>().clip = audios[DEAD_SOUND];
        Instantiate(deadSoundObject, transform.position, transform.rotation);

        for (int i = 0; i <= soulsToGive; i++)
        {
            GameObject g = Instantiate(soul, transform.position, Quaternion.identity);
            g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * soulForce, ForceMode2D.Impulse);
        }

        Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle, transform.position, deathBloodParticle.transform.rotation);
        Destroy(gameObject);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
