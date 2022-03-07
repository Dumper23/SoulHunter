using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy_fly_melee : FatherEnemy
{
    private enum State
    {
        Waiting,
        Walking,
        Knockback,
        Dead
    }


    public int damageToPlayer = 1;
    public int pointsToGive = 10;

    private AudioSource audioSource;
    private bool alreadySound = false;
    public List<AudioClip> audios;
    public GameObject deadSoundObject;

    private const int DAMAGE_SOUND = 0;
    private const int DEAD_SOUND = 1;
    private const int INRANGE_SOUND = 2;
    

    private State currentState;

    [SerializeField]
    private float
        maxHealth,
        knockbackDuration,
        lineOfSite;

    [SerializeField]
    private Vector2 knockbackSpeed;

    [SerializeField]
    private GameObject
        hitParticle,
        deathChunkParticle,
        deathBloodParticle;

    private int
        damageDirectionX,
        damageDirectionY;

    private Vector2 
        movement,
        aux;

    private float
        currentHealth,
        knockbackStartTime;
    private float[] posPlayerForKnockback;


    private Rigidbody2D rb;

    private AIPath aiPath;

    private Transform player;

    private bool knocking;

    [SerializeField]
    private ParticleSystem particleDamage;


    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();

        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        currentHealth = maxHealth;
        if (distanceFromPlayer > lineOfSite)
        {
            SwitchState(State.Waiting);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if(distanceFromPlayer < lineOfSite && !knocking)
        {
            SwitchState(State.Walking);
        }
        else if(distanceFromPlayer > lineOfSite)
        {
            SwitchState(State.Waiting);
        }*/

        switch (currentState)
        {
            case State.Waiting:
                UpdateWaitingState();
                break;
            case State.Walking:
                UpdateWalkingState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }

    }
    //---------WAITING--------------
    #region WAITING
    private void EnterWaitingState()
    {
        aiPath.canMove = false;
    }
    private void UpdateWaitingState()
    {
        
        aux.Set(0, 0);
        rb.velocity = aux;
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer < lineOfSite)
        {
            SwitchState(State.Walking);
            if (!alreadySound)
            {
                audioSource.clip = audios[INRANGE_SOUND];
                audioSource.Play();
                alreadySound = true;
            }
        }
        else
        {
            alreadySound = false;
        }
    }
    #endregion
    private void ExitWaitingState()
    {
        aiPath.canMove = true;
    }
    //---------WALKING---------------
    #region WALKING
    private void EnterWalkingState()
    {
        aiPath.canMove = true;
    }

    private void UpdateWalkingState()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer > lineOfSite)
        {
            SwitchState(State.Waiting);
        }
    }

    private void ExitWalkingState()
    {
        aiPath.canMove = false;
    }
    #endregion


    //---------KNOCKBACK---------------
    #region KNOCKBACK
    private void EnterKnockbackState()
    {
        knocking = true;
        knockbackStartTime = Time.time;
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
        movement.Set(knockbackSpeed.x * damageDirectionX, knockbackSpeed.y * damageDirectionY);
        rb.velocity = movement;
    }

    private void UpdateKnockbackState()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
            if(distanceFromPlayer < lineOfSite)
            {
                SwitchState(State.Walking);
            }
            if(distanceFromPlayer > lineOfSite)
            {
                SwitchState(State.Waiting);
            }
        }
    }

    private void ExitKnockbackState()
    {
        knocking = false;
    }
    #endregion

    //---------DEAD---------------
    #region DEAD
    private void EnterDeadState()
    {
        //Spawn chunks and blood
        deadSoundObject.GetComponent<AudioSource>().clip = audios[DEAD_SOUND];
        Instantiate(deadSoundObject, transform.position, transform.rotation);

        Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle, transform.position, deathBloodParticle.transform.rotation);
        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }
    #endregion


    public override void Damage(float[] attackDetails, bool wantKnockback)
    {


        currentHealth -= attackDetails[0];

        audioSource.clip = audios[DAMAGE_SOUND];
        audioSource.Play();
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
            if (wantKnockback)
            {
                SwitchState(State.Knockback);
            }
        }
        else if (currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
            GameManager.Instance.addPoints(pointsToGive);
        }
    }
    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Waiting:
                ExitWaitingState();
                break;
            case State.Walking:
                ExitWalkingState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (state)
        {
            case State.Waiting:
                EnterWaitingState();
                break;
            case State.Walking:
                EnterWalkingState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }

    public override void applyKnockback(float[] position)
    {
        ExitWalkingState();
        posPlayerForKnockback = new float[3];
        posPlayerForKnockback[0] = position[0];
        posPlayerForKnockback[1] = position[1];
        posPlayerForKnockback[2] = position[2];
        SwitchState(State.Knockback);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, lineOfSite);
    }
    public override void mostraMissatge()
    {
        Debug.Log("EEEEEEIIII3");
    }
}
