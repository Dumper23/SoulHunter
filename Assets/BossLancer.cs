using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLancer : FatherEnemy
{
    private enum State
    {
        Waiting,
        FrontAttack,
        Summoning,
        DownLancers,
        UpperAttack,
        Dead
    }

    private Transform player;

    public int pointsToGive = 100;

    private State currentState;
    private State[] statesToRandomize;

    private Animator 
        lancerAnimation,
        upperRangeCircleAnimation;

    private BoxCollider2D area,
        upperRange;

    [SerializeField]
    private GameObject
        downLancer,
        summoner,
        healer;

    [SerializeField]
    private ParticleSystem lancerParticles;

    private float quantity,
        lancersParticlesStartTime,
        downLancersAttackStartTime,
        waitingStartTime,
        frontAttackStartTime,
        summoningStartTime,
        nextSummonStartTime,
        upperAttackStartTime,
        inRangeStartTime,
        currentHealth;

    private int[] notSpawn,
        summon;

    private bool isAttackDone,
        particlesEnded,
        particlesCreated,
        isActivated = false,
        inRange = false,
        goUpper = false,
        upperDone = false;

    [SerializeField]
    private float lineOfActivation,
        waitLancersParticlesDuration = 3f,
        downLancersAttackDuration = 1f,
        waitingForAttackDuration = 2f,
        frontAttackDuration = 1.5f,
        summoningDuration = 3f,
        waitUntilNextSummon = 0.5f,
        upperChargeDuration = 1f,
        upperAttackDuration = 1f,
        timeInRange = 2f,
        percentageHealers = 0.15f,
        maxHealth = 500;

    public Transform spawnSummoner;

    private int 
        quantitySummoning,
        quantitySummoned;

    public HealthBarBoss healthBar;

    private UpperRangePlayerDetection upperAttackRange;

    private GameObject upperRangeCircle;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<playerController>().gameObject.transform;
        GameObject lancer = transform.Find("Lancer").gameObject;
        lancerAnimation = lancer.GetComponent<Animator>();
        area = transform.Find("Area").gameObject.GetComponent<BoxCollider2D>();
        upperRange = transform.Find("UpperRange").gameObject.GetComponent<BoxCollider2D>();
        upperAttackRange = upperRange.GetComponent<UpperRangePlayerDetection>();
        upperRangeCircle = transform.Find("UpperRangeCircle").gameObject;
        upperRangeCircleAnimation = upperRangeCircle.GetComponent<Animator>();

        statesToRandomize = new State[4];
        statesToRandomize[0] = State.Waiting;
        statesToRandomize[1] = State.DownLancers;
        statesToRandomize[2] = State.FrontAttack;
        statesToRandomize[3] = State.Summoning;

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        SwitchState(State.Waiting);

    }

    // Update is called once per frame
    void Update()
    {
        if (!inRange)
        {
            if (upperAttackRange.IsInRange())
            {
                inRangeStartTime = Time.time;
                inRange = true;
            }
        }
        else
        {

            if (!upperAttackRange.IsInRange())
            {
                inRange = false;
            }
            else
            {
                if (!goUpper) {
                    if (Time.time > inRangeStartTime + timeInRange)
                    {
                        goUpper = true;
                    }
                }
            }
        }
        switch (currentState)
        {
            case State.Waiting:
                UpdateWaitingState();
                break;
            case State.FrontAttack:
                UpdateFrontAttackState();
                break;
            case State.Summoning:
                UpdateSummoningState();
                break;
            case State.DownLancers:
                UpdateDownLancersState();
                break;
            case State.UpperAttack:
                UpdateUpperAttackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }

    }

    //---------WAITING-------------
    #region WAITING
    private void EnterWaitingState()
    {
        if (isActivated) {
            waitingStartTime = Time.time;
        }
    }

    private void UpdateWaitingState()
    {
        if (!isActivated)
        {
            float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
            if (distanceFromPlayer < lineOfActivation)
            {
                isActivated = true;
                waitingStartTime = Time.time;
                healthBar.gameObject.SetActive(true);
            }
        }
        else
        {
            if(Time.time >= waitingStartTime + waitingForAttackDuration)
            {
                if (goUpper)
                {
                    SwitchState(State.UpperAttack);
                }
                else
                {
                    SwitchState(randomBehaviour());
                }
            }
        }
    }

    private void ExitWaitingState()
    {

    }
    #endregion

    //--------FRONTATTACK----------
    #region FRONTATTACK
    private void EnterFrontAttackState()
    {
        frontAttackStartTime = Time.time;
        lancerAnimation.Play("LancerAnimation");
    }

    private void UpdateFrontAttackState()
    {
        if (Time.time >= frontAttackStartTime + frontAttackDuration)
        {
            SwitchState(State.Waiting);
        }

    }

    private void ExitFrontAttackState()
    {
        lancerAnimation.Play("noLancerAnimation");
    }
    #endregion

    //----------SUMMONING--------------
    #region SUMMONING
    private void EnterSummoningState()
    {
        summoningStartTime = Time.time;
        nextSummonStartTime = Time.time;
        quantitySummoned = 0;

        quantitySummoning = (int)Mathf.Round(summoningDuration / waitUntilNextSummon);

        int quantityHealers = (int)Mathf.Round(percentageHealers * quantitySummoning);

        summon = new int[quantityHealers];

        bool isOK;
        //Healers
        for (int i = 0; i < quantityHealers; i++)
        {
            int attempt = 0;
            isOK = false;
            while (!isOK)
            {
                bool found = false;
                attempt = (int)Mathf.Round(Random.Range(0, quantitySummoning));

                for (int j = 0; j < i; j++)
                {
                    if (summon[j] == attempt)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    isOK = true;
                }
            }

            summon[i] = attempt;
        }

    }

    private void UpdateSummoningState()
    {


        if(Time.time >= summoningStartTime + summoningDuration)
        {
            if (goUpper)
            {
                SwitchState(State.UpperAttack);
            }
            else
            {
                SwitchState(State.Waiting);
            }
        }
        else
        {
            if (Time.time >= nextSummonStartTime + waitUntilNextSummon)
            {

                //summon enemies
                if (quantitySummoned < quantitySummoning)
                {

                    bool found = false;
                    for (int j = 0; j < summon.Length; j++)
                    {
                        if (summon[j] == quantitySummoned)
                        {
                            found = true;
                        }
                    }

                    if (found)
                    {
                        Instantiate(healer, spawnSummoner);
                    }
                    else
                    {
                        Instantiate(summoner, spawnSummoner);
                    }
                    quantitySummoned++;
                    nextSummonStartTime = Time.time;
                }
            }
        }
    }

    private void ExitSummoningState()
    {

    }
    #endregion

    //----------DOWNLANCERS-------------
    #region DOWNLANCERS
    private void EnterDownLancersState()
    {
        lancersParticlesStartTime = Time.time;
        isAttackDone = false;
        particlesEnded = false;
        particlesCreated = false;
        Vector3 areaSize = area.size;
        
        quantity = Mathf.Round(areaSize.x / downLancer.transform.localScale.x);
        //Debug.Log(areaSize);
        //Debug.Log(positionStart);
        //Debug.Log(quantity);
        float quantityEmpty = Mathf.Round(quantity * 0.2f);
        //Debug.Log(quantityEmpty);

        notSpawn = new int[((int)(quantityEmpty))];

        bool isOK;

        //Lancers where not to spawn
        for (int i = 0; i < quantityEmpty; i++)
        {
            int attempt = 0;
            isOK = false;
            while (!isOK)
            {
                bool found = false;
                attempt = Random.Range(0, (int)quantity);
                
                for (int j = 0; j < i; j++)
                {
                    if (notSpawn[j] == attempt)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    isOK = true;
                }
            }

            notSpawn[i] = attempt;
        }
        //Debug.Log(a[0] + " " + a[1]);
    }

    private void UpdateDownLancersState()
    {
        if(Time.time >= lancersParticlesStartTime + waitLancersParticlesDuration)
        {
            particlesEnded = true;
        }
        if (!isAttackDone)
        {
            Vector2 positionStart = area.transform.position;
            positionStart.x = positionStart.x + (downLancer.transform.localScale.x / 2);
            //Create Lancers in correct positions
            for (int i = 0; i < quantity; i++)
            {
                bool found = false;
                for (int j = 0; j < notSpawn.Length; j++)
                {
                    if (notSpawn[j] == i)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    Vector3 lancerPosition = new Vector3(positionStart.x + i * (downLancer.transform.localScale.x), positionStart.y, 0);

                    if (particlesEnded) {
                        GameObject lancer = downLancersPool.downLancersPoolInstance.GetLancer();
                        lancer.transform.position = lancerPosition;
                        lancer.SetActive(true);
                    }
                    if (!particlesCreated) {
                        GameObject particle = downLancersPool.downLancersPoolInstance.GetParticle();
                        particle.transform.position = lancerPosition;
                        particle.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
                        particle.SetActive(true);
                        //Instantiate(lancerParticles, lancerPosition, Quaternion.Euler(-90f,0f,0f));
                    }

                }
            }
            if (particlesEnded)
            {
                isAttackDone = true;
                downLancersAttackStartTime = Time.time;
            }
        }
        if (!particlesCreated)
        {
            particlesCreated = true;
        }

        if (isAttackDone && Time.time >= downLancersAttackStartTime + downLancersAttackDuration)
        {
            if (goUpper)
            {
                SwitchState(State.UpperAttack);
            }
            else
            {
                SwitchState(State.Waiting);
            }
        }
    }

    private void ExitDownLancersState()
    {
        downLancersPool.downLancersPoolInstance.DisableAll();
    }
    #endregion

    //---------UPPERATTACK--------------
    #region UPPERATTACK
    private void EnterUpperAttackState()
    {
        upperAttackStartTime = Time.time;
        upperDone = false;
        upperRangeCircle.SetActive(true);
        upperRangeCircleAnimation.Play("UpperRangeAnimation");
    }

    private void UpdateUpperAttackState()
    {
        if (!upperDone && Time.time >= upperAttackStartTime + upperChargeDuration)
        {
            upperDone = true;
            lancerAnimation.Play("UpperLancerAnimation");
            upperRangeCircle.SetActive(false);
        }

        if (Time.time >= upperAttackStartTime + upperAttackDuration + upperChargeDuration)
        {
            SwitchState(State.Waiting);
        }
    }

    private void ExitUpperAttackState()
    {
        lancerAnimation.Play("noLancerAnimation");
        goUpper = false;
        inRange = false;
        
    }
    #endregion

    //------------DEAD-----------------
    #region DEAD
    private void EnterDeadState()
    {
        healthBar.gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }
    #endregion

    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Waiting:
                ExitWaitingState();
                break;
            case State.FrontAttack:
                ExitFrontAttackState();
                break;
            case State.Summoning:
                ExitSummoningState();
                break;
            case State.DownLancers:
                ExitDownLancersState();
                break;
            case State.UpperAttack:
                ExitUpperAttackState();
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
            case State.FrontAttack:
                EnterFrontAttackState();
                break;
            case State.Summoning:
                EnterSummoningState();
                break;
            case State.DownLancers:
                EnterDownLancersState();
                break;
            case State.UpperAttack:
                EnterUpperAttackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }
    private State randomBehaviour()
    {
        int pos = Random.Range(0, (statesToRandomize.Length));
        return statesToRandomize[pos];
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfActivation);
    }

    public override void applyKnockback(float[] position)
    {
        //Nothing
    }
    public override void Damage(float[] attackDetails, bool wantKnockback)
    {
        currentHealth -= attackDetails[0];
        healthBar.SetHealth(currentHealth);

        //Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        //particleDamage.Play();
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

        if (currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
            GameManager.Instance.addPoints(pointsToGive);
        }
    }

    public override void mostraMissatge()
    {
        Debug.Log("Im a BOSS");
    }
}
