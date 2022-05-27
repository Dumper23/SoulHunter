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

    //public GameObject viewA;
    public GameObject sprite;
    public GameObject sprite2;
    //public GameObject viewB;

    [SerializeField]
    private GameObject
        deathChunkParticle,
        deathBloodParticle,
        portal;

    public float speed = 2f,
                speedRoll = 8f;

    [SerializeField]
    private float lineOfSite,
        maxHealth = 80,
        knockbackDuration = 0.5f,
        maxWalkingSwitchStateDuration = 6f,
        maxAttackRollDuration = 12f,
        DefenseDuration = 2f,
        SpikesDuration = 1f,
        wallCheckDistance,
        waitingDuration = 2f;

    private bool
        wallDetected,
        inThorns,
        isKnockingBack = false,
        isActivated = false;

    private int facingDirection,
        ansFacingDirection;

    private float
        currentHealth,
        knockbackStartTime,
        AttackRollStartTime,
        DefenseStartTime,
        SpikesStartTime,
        walkingStartTime,
        attackRollDuration,
        walkingDuration,
        waitingStartTime;

    private State currentState;
    private State[] statesToRandomize;

    [SerializeField]
    private BossRangeOfActivation rangeOfActivation;

    [SerializeField]
    private Transform wallCheck;

    [SerializeField]
    private LayerMask
        whatIsGround;

    [SerializeField]
    private ParticleSystem particleDamage;

    public bool swicher = true;

    [SerializeField]
    private GameObject thornsSoul;

    private Animator spriteAnimator;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindObjectOfType<playerController>().transform;
        spriteAnimator = gameObject.GetComponent<Animator>();
        statesToRandomize = new State[5];
        statesToRandomize[0] = State.Defense;
        statesToRandomize[1] = State.AttackRoll;
        statesToRandomize[2] = State.Spikes;
        statesToRandomize[3] = State.Spikes;
        statesToRandomize[4] = State.Spikes;

        if (player.position.x > this.transform.position.x)
        {
            facingDirection = 1;
            sprite.transform.Rotate(0.0f, 180.0f, 0.0f);
            sprite2.transform.Rotate(0.0f, 180.0f, 0.0f);

        }
        else
        {
            sprite.transform.Rotate(0.0f, 180.0f, 0.0f);
            sprite2.transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (swicher)
        {
            sprite.SetActive(true);
            sprite2.SetActive(false);
        }
        else
        {
            sprite.SetActive(false);
            sprite2.SetActive(true);
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
        if (isActivated)
        {
            waitingStartTime = Time.time;
        }
        spriteAnimator.Play("Champion1Idle");
    }

    private void UpdateWaitingState()
    {
        if (!isActivated)
        {/*
            float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
            if (distanceFromPlayer < lineOfSite)
            {
                isActivated = true;
                inRange = true;
                SwitchState(State.Walking);
            }*/
            if (rangeOfActivation.inRange())
            {
                SwitchState(State.Walking);
                isActivated = true;
            }
        }
        else
        {
            if (Time.time >= waitingStartTime + waitingDuration)
            {
                float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
                if (distanceFromPlayer < lineOfSite)
                {
                    SwitchState(State.Walking);
                }
                else
                {
                    SwitchState(State.Waiting);
                }
            }
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
        walkingDuration = Random.Range(maxWalkingSwitchStateDuration / 3, maxWalkingSwitchStateDuration + 1);
        spriteAnimator.Play("Champion1Walk");
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
            sprite.transform.Rotate(0.0f, 180.0f, 0.0f);
            sprite2.transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer > lineOfSite)
        {
            SwitchState(State.Waiting);
        }
        else
        {
            if (Time.time >= walkingStartTime + walkingDuration) {
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
        spriteAnimator.Play("Champion1Idle");
    }
    #endregion

    //---------ATTACKROLL---------------
    #region ATTACKROLL
    private void EnterAttackRollState()
    {
        AttackRollStartTime = Time.time;
        swicher = false;
        inThorns = true;
        attackRollDuration = Random.Range(maxAttackRollDuration/2, maxAttackRollDuration + 1);
        spriteAnimator.Play("Champion1Rush");
    }

    private void UpdateAttackRollState()
    {
        if (Time.time >= AttackRollStartTime + attackRollDuration)
        {
            SwitchState(State.Waiting);
        }
        else
        {
            wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance*facingDirection, whatIsGround);
            if (wallDetected)
            {
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
        spriteAnimator.Play("Champion1Defense");
    }

    private void UpdateDefenseState()
    {
        if (Time.time >= DefenseStartTime + DefenseDuration)
        {
            SwitchState(randomBehaviour());
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
        //sprite.transform.localScale -= new Vector3(0.2f, 0.0f, 0.0f);
        //sprite2.transform.localScale -= new Vector3(0.2f, 0.0f, 0.0f);
        spriteAnimator.Play("Champion1Shoot");
    }

    private void UpdateSpikesState()
    {
        if (Time.time >= SpikesStartTime + SpikesDuration)
        {
            SwitchState(randomBehaviour());
        }
    }

    private void ExitSpikesState()
    {
        GetComponent<fireSpikes>().Shoot();
        inThorns = false;
        //sprite.transform.localScale += new Vector3(0.2f, 0.0f, 0.0f);
        //sprite2.transform.localScale += new Vector3(0.2f, 0.0f, 0.0f);
        spriteAnimator.Play("Champion1Defense");
    }

    #endregion

    //---------KNOCKBACK---------------
    #region KNOCKBACK
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        //sprite.transform.localScale -= new Vector3(0.0f,0.2f,0.0f);
        //sprite2.transform.localScale -= new Vector3(0.0f, 0.2f, 0.0f);
        isKnockingBack = true;
        spriteAnimator.Play("Champion1Knockback");
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
        //sprite.transform.localScale += new Vector3(0.0f, 0.2f, 0.0f);
        //sprite2.transform.localScale += new Vector3(0.0f, 0.2f, 0.0f);
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
        Instantiate(thornsSoul, transform.position, thornsSoul.transform.rotation);
        //GameObject p = Instantiate(portal, new Vector3(sprite.transform.position.x, sprite.transform.position.y + 3, sprite.transform.position.z), portal.transform.rotation);
        //p.GetComponent<EndLevel>().nextLevelName = "DemoEnd";
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
        sprite.transform.Rotate(0.0f, 180.0f, 0.0f);
        sprite2.transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    public override void Damage(float[] attackDetails, bool wantKnockback)
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
        //Gizmos.DrawWireSphere(transform.position, lineOfSite);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance*facingDirection, wallCheck.position.y));

    }

    public override void mostraMissatge()
    {
        Debug.Log("Soy SONIIIICC");
    }
}
