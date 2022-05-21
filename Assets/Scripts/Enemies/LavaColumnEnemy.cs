using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaColumnEnemy : FatherEnemy
{
    public ParticleSystem indicator;
    public float health = 5;
    public float soulForce = 35;
    public GameObject soul;
    public GameObject deathChunkParticle;
    public int soulsToGive = 7;
    public float attackRate = 1f;
    public float range = 10f;
    public GameObject lava;

    [SerializeField]
    private ParticleSystem particleDamage;

    public List<AudioClip> audios;
    public AudioSource audioSource;
    public GameObject deadSoundObject;


    private Transform target;
    private SpriteRenderer sp;
    private const int ATTACK_SOUND = 0;
    private const int DEAD_SOUND = 1;
    private float time = 0;

    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        target = FindObjectOfType<playerController>().transform;
    }

    void Update()
    {
        if(target.position.x > transform.position.x)
        {
            sp.flipX = true;
        }
        else
        {
            sp.flipX = false;
        }

        if(health <= 0)
        {
            Dead();
        }

        time += Time.deltaTime;
        if(time >= attackRate)
        {
            if ((target.position - transform.position).magnitude <= range)
            {
                time = -99999;
                shoot();
            }
        }

    }

    private void shoot()
    {
        indicator.transform.position = target.position - new Vector3(0, 0.5f, 0);
        indicator.gameObject.SetActive(true);
        indicator.Play();
        audioSource.clip = audios[ATTACK_SOUND];
        audioSource.Play();
        Invoke("generateLava", 0.75f);
    }

    private void generateLava()
    {
        Instantiate(lava, indicator.transform.position - new Vector3(0, 5f, 0), Quaternion.identity);
        time = 0;
    }

    public override void applyKnockback(float[] position)
    {
    }

    public override void Damage(float[] damageMessage, bool wantKnockback)
    {
        health -= damageMessage[0];
        audioSource.Play();
        particleDamage.Play();
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
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
