using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kamikaze : FatherEnemy
{
    public float detectionRange = 6f;
    public float explotionStartRange = 2f;
    public float explotionDamageRange = 3f;
    public float moveSpeed = 2f;
    public float explotionTime = 5f;
    public List<AudioClip> audios;
    public GameObject deadSoundObject;
    public GameObject circleAdvice;

    public int soulsToGive = 5;
    public GameObject soul;
    public float soulForce = 35;
    public int pointsToGive = 5;

    public float speedKnockback;
    public float knockbackDuration;

    [SerializeField]
    private GameObject
        deathChunkParticle;

    [SerializeField]
    private ParticleSystem particleDamage;

    private const int INRANGE_SOUND = 0;
    private const int DEAD_SOUND = 1;
    

    private AudioSource audioSource;

    private GameObject target;
    private Animator anim;

    private bool explotionReady = false;
    private bool dead = false;
    private bool alreadyDetected = false;

    private float flipX = 0;
    private SpriteRenderer sp;

    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        flipX = -transform.localScale.x;
        anim = GetComponent<Animator>();
        target = FindObjectOfType<playerController>().gameObject;
        audioSource = GetComponent<AudioSource>();
        circleAdvice.SetActive(false);
    }

    void Update()
    {
        if (target.transform.position.x > transform.position.x)
        {
            //transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            sp.flipX = false;
        }
        else
        {
            //transform.localScale = new Vector3(-flipX, transform.localScale.y, transform.localScale.z);
            sp.flipX = true;
        }
        if (explotionReady)
        {
            Invoke("explode", explotionTime);
        }
        if(target != null && !dead)
        {
            if ((target.transform.position - transform.position).magnitude <= detectionRange)
            {
                if (!alreadyDetected)
                {
                    audioSource.clip = audios[INRANGE_SOUND];
                    audioSource.Play();
                }
                
                anim.Play("kamikazeMove");
                transform.Translate((target.transform.position - transform.position).normalized * Time.deltaTime * moveSpeed);
                if((target.transform.position - transform.position).magnitude <= explotionStartRange)
                {
                    anim.Play("kamikazeIdle");
                    circleAdvice.SetActive(true);
                    explotionReady = true;
                    dead = true;
                }
                alreadyDetected = true;
            }
            else
            {
                alreadyDetected = false;
                anim.Play("kamikazeIdle");
            }
        }
        if (dead)
        {
            explotionReady = true;
        }
    }

    private void explode()
    {
        Collider2D[] c = Physics2D.OverlapCircleAll(transform.position, explotionDamageRange);
        foreach (Collider2D collision in c)
        {
            if (collision.transform.tag == "Player")
            {
                collision.GetComponent<playerController>().takeDamage();
            }
        }
        Dead();
        Destroy(gameObject);
    }

    public override void applyKnockback(float[] position)
    {
        
    }

    public override void Damage(float[] attackDetails, bool wantKnockback)
    {
        
    }

    public override void mostraMissatge()
    {

    }

    private void Dead()
    {
        //Spawn chunks and blood
        deadSoundObject.GetComponent<AudioSource>().clip = audios[DEAD_SOUND];
        deadSoundObject.GetComponent<AudioSource>().volume = 0.2f;
        Instantiate(deadSoundObject, transform.position, transform.rotation);

        for (int i = 0; i < soulsToGive; i++)
        {
            GameObject g = Instantiate(soul, transform.position, Quaternion.identity);
            g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * soulForce, ForceMode2D.Impulse);
        }

        Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
        Destroy(gameObject);
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explotionStartRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explotionDamageRange);


    }
}
