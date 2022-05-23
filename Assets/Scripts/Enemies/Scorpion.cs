using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorpion : FatherEnemy
{
    private enum State
    {
        Walking,
        Knockback,
        Dead,
        Waiting
    }

    public bool isPoisonous = true;

    public GameObject modifierIndicator;
    public float rayOffsetY = 1.5f;
    public float rayOffsetX = 1.5f;

    public int damageToPlayer = 1;
    public int pointsToGive = 10;

    public int soulsToGive = 5;
    public GameObject soul;
    public float soulForce;

    private AudioSource audioSource;
    public List<AudioClip> audios;
    public GameObject deadSoundObject;

    private const int DAMAGE_SOUND = 0;
    private const int DEAD_SOUND = 1;


    private State currentState;

    [SerializeField]
    private float
        groundCheckDistance,
        wallCheckDistance,
        movementSpeed,
        maxHealth,
        knockbackDuration,
        enemyDetectionRange;

    [SerializeField]
    private Transform
        groundCheck,
        groundCheckBack,
        wallCheck,
        enemyCollision;

    [SerializeField]
    private LayerMask 
        whatIsGround, 
        whatIsEnemy,
        whatIsTrap,
        whatIsDiffWall,
        whatIsMushroom,
        whatIsDoor;

    [SerializeField]
    private Vector2 knockbackSpeed;

    [SerializeField]
    private GameObject
        hitParticle,
        deathChunkParticle,
        deathBloodParticle;

    private int 
        facingDirection,
        damageDirection;

    private Vector2 movement;

    private float 
        currentHealth,
        knockbackStartTime;
    private float[] posPlayerForKnockback;

    private bool
        groundDetected,
        groundDetectedBack,
        wallDetected,
        enemyDetected,
        spikesDetected,
        spikesDetected2,
        doorDetected,
        mushroomDetected,
        diffWallDetected;

    public GameObject alive;

    private Rigidbody2D aliveRb;

    private Animator aliveAnim;

    [SerializeField]
    private ParticleSystem particleDamage;

    private bool playerDetected = false;
    private bool isFalling = true;
    private Transform target;

    private void Awake()
    {
        aliveRb = alive.GetComponent<Rigidbody2D>();
        aliveRb.gravityScale = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(modifierIndicator != null)
        {
            if (hasShield || isDemon)
            {
                modifierIndicator.SetActive(true);
            }
        }
        
        aliveAnim = alive.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        target = FindObjectOfType<playerController>().transform;
        facingDirection = 1;
        currentHealth = maxHealth;
        SwitchState(State.Waiting);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Walking:
                UpdateWalkingState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
            case State.Waiting:
                UpdateWaitingState();
                break;
        }

    }

    //---------WALKING---------------
    #region WALKING
    private void EnterWalkingState()
    {
        
    }

    private void UpdateWalkingState()
    {
        aliveRb.gravityScale = 1f;
        
        
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        groundDetectedBack = Physics2D.Raycast(groundCheckBack.position, Vector2.down, groundCheckDistance, whatIsGround);
        spikesDetected = Physics2D.Raycast(groundCheckBack.position, Vector2.down, groundCheckDistance, whatIsTrap);
        spikesDetected2 = Physics2D.Raycast(groundCheckBack.position, Vector2.down, groundCheckDistance, whatIsTrap);
        if ((spikesDetected || spikesDetected2) && !(groundDetected || groundDetectedBack))
        {
            SwitchState(State.Dead);
        }

        wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance * facingDirection, whatIsGround);
        diffWallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsDiffWall);
        doorDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsDoor);
        enemyDetected = Physics2D.OverlapCircle(enemyCollision.position, facingDirection * enemyDetectionRange, whatIsEnemy);
        mushroomDetected = Physics2D.OverlapCircle(enemyCollision.position, facingDirection * enemyDetectionRange, whatIsMushroom);

        if((!groundDetected && groundDetectedBack) || wallDetected || enemyDetected || doorDetected || diffWallDetected || mushroomDetected)
        {
            aliveRb.velocity =new Vector2(0.4f, 2f);
        }
        else
        {
            if (groundDetected && groundDetectedBack)
            {
                isFalling = false;
            }

            if (!isFalling)
            {
                if (target.position.x > alive.transform.position.x)
                {
                    facingDirection = 1;
                    alive.transform.localScale = new Vector3(1, alive.transform.localScale.y, alive.transform.localScale.z);
                }
                else
                {
                    facingDirection = -1;
                    alive.transform.localScale = new Vector3(-1, alive.transform.localScale.y, alive.transform.localScale.z);
                }
                movement.Set(movementSpeed * facingDirection, aliveRb.velocity.y);
                aliveRb.velocity = movement;
            }
            
        }
    }

    private void ExitWalkingState()
    {

    }
    #endregion


    //---------KNOCKBACK---------------
    #region KNOCKBACK
    private void EnterKnockbackState()
    {
        alive.GetComponent<SpriteRenderer>().flipY = false;
        knockbackStartTime = Time.time;
        if(null != posPlayerForKnockback)
        {
            if(posPlayerForKnockback[0] == 1.0f)
            {
                if (posPlayerForKnockback[1] > alive.transform.position.x)
                {
                    damageDirection = -1;
                }
                else
                {
                    damageDirection = 1;
                }
                posPlayerForKnockback[0] = 0.0f;
            }
        }
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        aliveRb.velocity = movement;
        aliveAnim.SetBool("Knockback", true);
    }

    private void UpdateKnockbackState()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Walking);
        }
    }

    private void ExitKnockbackState()
    {
        aliveAnim.SetBool("Knockback", false);
    }
    #endregion

    //---------DEAD---------------
    #region DEAD
    private void EnterDeadState()
    {
        //Spawn chunks and blood
        deadSoundObject.GetComponent<AudioSource>().clip = audios[DEAD_SOUND];
        Instantiate(deadSoundObject, alive.transform.position, transform.rotation);

        for (int i = 0; i <= soulsToGive; i++)
        {
            GameObject g = Instantiate(soul, alive.transform.position, Quaternion.identity);
            g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * soulForce, ForceMode2D.Impulse);
        }

        Instantiate(deathChunkParticle, alive.transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle, alive.transform.position, deathBloodParticle.transform.rotation);
        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }
    #endregion

    //---------Waiting------------
    #region WAITING
    private void UpdateWaitingState()
    {
        if (!playerDetected)
        {
            aliveRb.gravityScale = 0;
            alive.GetComponent<SpriteRenderer>().flipY = true;
        }

        
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));
        RaycastHit2D hit = Physics2D.Raycast(alive.transform.position - new Vector3(rayOffsetX, rayOffsetY, 0), new Vector3(0, -1000) + alive.transform.position - new Vector3(rayOffsetX, rayOffsetY, 0));
        // If it hits something...
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
            if (hit.collider.tag == "Player")
            {
                aliveRb.gravityScale = 9.81f;
                alive.GetComponent<SpriteRenderer>().flipY = false;
                playerDetected = true;
                SwitchState(State.Walking);
            }
        }
    }
    #endregion
    //----ALTRES----

    public override void Damage(float[] attackDetails, bool wantKnockback)
    {
        if (isDemon)
        {
            currentHealth -= attackDetails[0]  / 3;
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
        if (attackDetails[1] > alive.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
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

    private void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void SwitchState(State state)
    {
        switch (currentState)
        {
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundCheckBack.position, new Vector2(groundCheckBack.position.x, groundCheckBack.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawLine(alive.transform.position - new Vector3(rayOffsetX, rayOffsetY, 0), new Vector3(0, -1000) + alive.transform.position - new Vector3(rayOffsetX, rayOffsetY, 0)) ;

    }

    public override void mostraMissatge()
    {
        Debug.Log("EEEEEEIIII");
    }

    public override void applyKnockback(float[] position)
    {
        posPlayerForKnockback = new float[2];
        posPlayerForKnockback[0] = position[0];
        posPlayerForKnockback[1] = position[1];
        SwitchState(State.Knockback);
    }
}
