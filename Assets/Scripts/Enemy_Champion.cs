using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Champion : FatherEnemy
{
    private enum State
    {
        Waiting,
        Walking,
        AttackRoll,
        Defense,
        Spikes,
        Knockback,
        Dead
    }

    public int pointsToGive = 30;

    private Transform player;
    private Rigidbody2D rb;

    public GameObject viewA;
    public GameObject viewB;

    [SerializeField]
    private GameObject
        deathChunkParticle,
        deathBloodParticle;

    public float speed = 2f,
                speedRoll = 8f;

    [SerializeField]
    private float lineOfSite,
        maxHealth = 80,
        knockbackDuration = 0.5f,
        walkingSwitchStateDuration = 2f,
        AttackRollDuration = 2f,
        DefenseDuration = 2f,
        SpikesDuration = 1f,
        wallCheckDistance;

    private bool inRange,
        wallDetected,
        inThorns,
        isKnockingBack = false;

    private int facingDirection,
        ansFacingDirection;

    private float
        currentHealth,
        knockbackStartTime,
        AttackRollStartTime,
        DefenseStartTime,
        SpikesStartTime,
        walkingStartTime;

    private State currentState;
    private State[] statesToRandomize;

    [SerializeField]
    private Transform wallCheck;

    [SerializeField]
    private LayerMask
        whatIsGround;

    [SerializeField]
    private ParticleSystem particleDamage;

    public bool swicher = true;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        statesToRandomize = new State[3];
        statesToRandomize[0] = State.Defense;
        statesToRandomize[1] = State.AttackRoll;
        statesToRandomize[2] = State.Spikes;

        if (player.position.x > this.transform.position.x)
        {
            facingDirection = 1;

        }
        else
        {
            viewA.transform.Rotate(0.0f, 180.0f, 0.0f);
            viewB.transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (swicher)
        {
            viewA.SetActive(true);
            viewB.SetActive(false);
        }
        else
        {
            viewA.SetActive(false);
            viewB.SetActive(true);
        }
        
        switch (currentState)
        {
            case State.Waiting:
                UpdateWaitingState();
                break;
            case State.Walking:
                UpdateWalkingState();
                break;
            case State.AttackRoll:
                UpdateAttackRollState();
                break;
            case State.Defense:
                UpdateDefenseState();
                break;
            case State.Spikes:
                UpdateSpikesState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }

    }

    //---------WAITING---------------
    #region WAITING
    private void EnterWaitingState()
    {
        swicher = true;
    }

    private void UpdateWaitingState()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer < lineOfSite)
        {
            inRange = true;
            SwitchState(State.Walking);
        }
    }

    private void ExitWaitingState()
    {

    }
    #endregion

    //---------WALKING---------------
    #region WALKING
    private void EnterWalkingState()
    {
        walkingStartTime = Time.time;
        swicher = true;
        inThorns = false;
        
    }

    private void UpdateWalkingState()
    {
        ansFacingDirection = facingDirection;
        if (player.position.x > this.transform.position.x)
        {
            facingDirection = 1;
            
        }
        else
        {
            facingDirection = -1;
        
        }
        if(ansFacingDirection != facingDirection)
        {
            viewA.transform.Rotate(0.0f, 180.0f, 0.0f);
            viewB.transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer > lineOfSite)
        {
            inRange = false;
            SwitchState(State.Waiting);
        }
        else
        {
            if (Time.time >= walkingStartTime + walkingSwitchStateDuration) {
                SwitchState(randomBehaviour());
            }
            else
            {
                Vector2 target = new Vector2(player.position.x, rb.position.y);
                Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime);
                rb.MovePosition(newPos);
            }
        }
    }

    private void ExitWalkingState()
    {

    }
    #endregion

    //---------ATTACKROLL---------------
    #region ATTACKROLL
    private void EnterAttackRollState()
    {
        AttackRollStartTime = Time.time;
        swicher = false;
        inThorns = true;
    }

    private void UpdateAttackRollState()
    {
        if (Time.time >= AttackRollStartTime + AttackRollDuration)
        {
            SwitchState(State.Waiting);
        }
        else
        {
            wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance*facingDirection, whatIsGround);
            if (wallDetected)
            {
                Debug.Log("WAAAAALL");
                Flip();
            }
            this.transform.Translate(new Vector2(1, 0) * speedRoll * facingDirection * Time.deltaTime);
        }
    }

    private void ExitAttackRollState()
    {
        inThorns = false;
    }
    #endregion

    //---------DEFENSE---------------
    #region DEFENSE
    private void EnterDefenseState()
    {
        DefenseStartTime = Time.time;
        swicher = false;
        inThorns = true;
    }

    private void UpdateDefenseState()
    {
        if (Time.time >= DefenseStartTime + DefenseDuration)
        {
            SwitchState(State.Waiting);
        }
    }

    private void ExitDefenseState()
    {
        inThorns = false;
    }
    #endregion

    //---------SPIKES----------------
    #region SPIKES
    private void EnterSpikesState()
    {
        SpikesStartTime = Time.time;
        swicher = false;
        inThorns = true;
        viewA.transform.localScale -= new Vector3(0.5f, 0.0f, 0.0f);
        viewB.transform.localScale -= new Vector3(0.5f, 0.0f, 0.0f);
    }

    private void UpdateSpikesState()
    {
        if (Time.time >= SpikesStartTime + SpikesDuration)
        {
            SwitchState(State.Defense);
        }
    }

    private void ExitSpikesState()
    {
        GetComponent<fireSpikes>().Shoot();
        inThorns = false;
        viewA.transform.localScale += new Vector3(0.5f, 0.0f, 0.0f);
        viewB.transform.localScale += new Vector3(0.5f, 0.0f, 0.0f);
    }
    #endregion
    //---------KNOCKBACK---------------
    #region KNOCKBACK
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        viewA.transform.localScale -= new Vector3(0.0f,0.5f,0.0f);
        viewB.transform.localScale -= new Vector3(0.0f, 0.5f, 0.0f);
        isKnockingBack = true;
    }

    private void UpdateKnockbackState()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Defense);
        }
    }

    private void ExitKnockbackState()
    {
        viewA.transform.localScale += new Vector3(0.0f, 0.5f, 0.0f);
        viewB.transform.localScale += new Vector3(0.0f, 0.5f, 0.0f);
        isKnockingBack = false;
    }
    #endregion

    //---------DEAD---------------
    #region DEAD
    private void EnterDeadState()
    {
        //Spawn chunks and blood
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

    private State randomBehaviour()
    {
        int pos = Random.Range(0, (statesToRandomize.Length));
        return statesToRandomize[pos];
    }
    private void Flip()
    {
        facingDirection *= -1;
        viewA.transform.Rotate(0.0f, 180.0f, 0.0f);
        viewB.transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    public override void Damage(float[] attackDetails)
    {
        if (!isKnockingBack) {
            if (!inThorns)
            {
                currentHealth -= attackDetails[0];

                //Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
                particleDamage.Play();
                /*
                if (attackDetails[1] > alive.transform.position.x)
                {
                    damageDirection = -1;
                }
                else
                {
                    damageDirection = 1;
                }*/

                //Hit particle

                if (currentHealth > 0.0f)
                {
                    SwitchState(State.Knockback);
                }
                else if (currentHealth <= 0.0f)
                {
                    SwitchState(State.Dead);
                    GameManager.Instance.addPoints(pointsToGive);
                }
            }
            else {
                player.GetComponent<playerController>().takeDamage();
            }
        }
    }

    public override void applyKnockback(float[] position)
    {
        SwitchState(State.Knockback);
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
            case State.AttackRoll:
                ExitAttackRollState();
                break;
            case State.Defense:
                ExitDefenseState();
                break;
            case State.Spikes:
                ExitSpikesState();
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
            case State.AttackRoll:
                EnterAttackRollState();
                break;
            case State.Defense:
                EnterDefenseState();
                break;
            case State.Spikes:
                EnterSpikesState();
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSite);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance*facingDirection, wallCheck.position.y));

    }

    public override void mostraMissatge()
    {
        Debug.Log("Soy SONIIIICC");
    }
}
