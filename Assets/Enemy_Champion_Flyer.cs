using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Enemy_Champion_Flyer : FatherEnemy
{
    private enum State
    {
        Waiting,
        Walking,
        AttackRush,
        Lightning,
        Summon,
        Knockback,
        Dead
    }

    public int pointsToGive = 30;

    private Transform player;
    private Rigidbody2D rb;

    [SerializeField]
    private BossRangeOfActivation rangeOfActivation;

    [SerializeField]
    private Transform[] Spots;

    public GameObject sprite;

    [SerializeField]
    private Transform portalSpawn,
        spotA,
        spotB;

    [SerializeField]
    private GameObject
        portal,
        lightningGO,
        lightningAttackGO,
        summon,
        floor,
        areaLightnings,
        rushHight;

    public float speed = 2f;

    [SerializeField]
    private float 
        maxHealth = 80,
        knockbackDuration = 0.5f,
        knockbackAnimation = 1f,
        maxWalkingSwitchStateDuration = 6f,
        waitingDuration = 2f,
        walkingDuration = 2f,
        timeBetweenLightnings = 3f,
        shadowingDuration = 1.5f,
        singleLightningDuration = 2,
        lightningingDuration = 3f,
        lightningAnimationDuration = 1f,
        summonPreDuration = 1f,
        summonDuration = 2f,
        rushMovingDuration = 1f,
        rushDownDuration = 1f,
        rushVulnerableDuration = 2f;

    private bool
        isKnockingBack = false,
        isActivated = false,
        vulnerable = false,
        lightininging = false,
        lnFirstLoop = true,
        isAttackLightninging = false,
        firstLoop = false,
        rushing = false;

    private int facingDirection,
        ansFacingDirection,
        pos;

    private float
        currentHealth,
        knockbackStartTime,
        walkingStartTime,
        waitingStartTime,
        startIntensity,
        endIntensity,
        lightningsStartTime,
        lightingingStartTime,
        startAngle,
        endAngle,
        summonStartTime,
        rushStartTime;

    private State currentState;
    private State[] statesToRandomize;

    [SerializeField]
    private BoxCollider2D lightningArea;

    [SerializeField]
    private Light2D globalLight;

    [SerializeField]
    private ParticleSystem particleDamage,
        particlesArea;

    private ParticleSystem lightningParticles;

    private Vector3 currentPos,
        startPos,
        endPos;

    [SerializeField]
    private AudioClip clip;

    [SerializeField]
    private AudioSource audio;

    [SerializeField]
    private Animator animatorLightning;
    private Animator animatorSprite;


    // Start is called before the first frame update
    void Start()
    {

        currentHealth = maxHealth;
        rb = GetComponentInChildren<Rigidbody2D>();
        player = GameObject.FindObjectOfType<playerController>().transform;
        lightningParticles = lightningGO.GetComponentInChildren<ParticleSystem>();
        animatorSprite = gameObject.GetComponent<Animator>();
        SwitchState(State.Waiting);
        statesToRandomize = new State[8];
        statesToRandomize[0] = State.Summon;
        statesToRandomize[1] = State.Summon;
        statesToRandomize[2] = State.Walking;
        statesToRandomize[3] = State.Walking;
        statesToRandomize[4] = State.AttackRush;
        statesToRandomize[5] = State.Summon;
        statesToRandomize[6] = State.Summon;
        statesToRandomize[7] = State.Walking;

        startIntensity = globalLight.intensity;
        endIntensity = 0.1f;
        lightningsStartTime = Time.time;
        /*
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
        }*/
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time >= lightningsStartTime + singleLightningDuration + shadowingDuration*2 + timeBetweenLightnings && !lightininging)
        {
            lightningsStartTime = Time.time;
            timeBetweenLightnings = Random.Range(1f,10f);
            lightininging = true;
            lnFirstLoop = true;

        }
        else
        {

            if (lightininging)
            {
                if (Time.time >= lightningsStartTime + singleLightningDuration + shadowingDuration * 2)
                {
                    lightininging = false;
                    //end fase
                    animatorLightning.Play("noAnimation");
                }
                else
                {
                    if (Time.time >= lightningsStartTime + singleLightningDuration + shadowingDuration)
                    {
                        //recovering shadow
                        globalLight.intensity = Lerp(endIntensity, startIntensity, lightningsStartTime + singleLightningDuration + shadowingDuration, shadowingDuration);

                    }
                    else
                    {
                        if (Time.time >= lightningsStartTime + shadowingDuration)
                        {
                            //lightining
                            if (lnFirstLoop)
                            {
                                float x = Random.Range(lightningArea.transform.position.x, lightningArea.transform.position.x + lightningArea.size.x);
                                //Animation collider play
                                lightningParticles.transform.parent.transform.position = new Vector3(x, lightningParticles.transform.parent.transform.position.y, lightningParticles.transform.parent.transform.position.z);
                                lightningParticles.Play();
                                lnFirstLoop = false;
                                audio.clip = clip;
                                audio.Play();
                                animatorLightning.Play("Lightning");
                            }
                        }
                        else
                        {
                            //shadowing
                            globalLight.intensity = Lerp(startIntensity, endIntensity, lightningsStartTime, shadowingDuration);


                        }
                    }
                }
            }
        }
        switch (currentState)
        {
            case State.Waiting:
                UpdateWaitingState();
                break;
            case State.Walking:
                UpdateWalkingState();
                break;
            case State.AttackRush:
                UpdateAttackRushState();
                break;
            case State.Lightning:
                UpdateLightningState();
                break;
            case State.Summon:
                UpdateSummonState();
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
        if (isActivated)
        {
            waitingStartTime = Time.time;
        }
    }

    private void UpdateWaitingState()
    {
        if (!isActivated)
        {
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
                SwitchState(randomBehaviour());
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
        currentPos = sprite.transform.position;
        pos = Random.Range(0,Spots.Length);
    }

    private void UpdateWalkingState()
    {
        if (Time.time >= walkingStartTime + walkingDuration || sprite.transform.position == Spots[pos].position)
        {
            SwitchState(randomBehaviour());
        }
        else
        {
            Vector2 target = new Vector2(Spots[pos].position.x, Spots[pos].position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime);
            rb.MovePosition(newPos);
        }
    }

    private void ExitWalkingState()
    {
    }
    #endregion

    //---------ATTACKRUSH---------------
    #region ATTACKRUSH
    private void EnterAttackRushState()
    {
        rushStartTime = Time.time;
        startPos = sprite.transform.position;
        endPos = player.transform.position;
    }

    private void UpdateAttackRushState()
    {
        if (Time.time >= rushStartTime + rushMovingDuration + rushDownDuration + rushVulnerableDuration)
        {
            SwitchState(State.Knockback);
        }
        else
        {
            if (Time.time >= rushStartTime + rushMovingDuration + rushDownDuration)
            {
                vulnerable = true;
            }
            else
            {
                if (Time.time >= rushStartTime + rushMovingDuration)
                {
                    rushing = true;
                    float pos = Lerp(rushHight.transform.position.y, floor.transform.position.y, (rushStartTime + rushMovingDuration), rushDownDuration);
                    sprite.transform.position = new Vector3(sprite.transform.position.x, pos,sprite.transform.position.z);
                }
                else
                {
                    float pos = Lerp(startPos.x, player.position.x, rushStartTime, rushMovingDuration);
                    float pos2 = Lerp(startPos.y, rushHight.transform.position.y, rushStartTime, rushMovingDuration);
                    sprite.transform.position = new Vector3(pos, pos2, sprite.transform.position.z);
                }
            }
        }
    }

    private void ExitAttackRushState()
    {
        vulnerable = false;
        rushing = false;
    }
    #endregion

    //---------LIGHTNING---------------
    #region LIGHTNING
    private void EnterLightningState()
    {
        lightningAttackGO.transform.position = sprite.transform.position;

        int type = Random.Range(0, 2);
        //SPOT A
        float b = Vector2.Distance(lightningAttackGO.transform.position, new Vector2(lightningAttackGO.transform.position.x, spotA.transform.position.y));
        float c = Vector2.Distance(lightningAttackGO.transform.position, spotA.transform.position);
        float a = Vector2.Distance(new Vector2(lightningAttackGO.transform.position.x, spotA.transform.position.y),spotA.transform.position); ;
        //startAngle = Mathf.Acos(((b * b) + (c * c) - (a * a)) / (2 * b * c)) * Mathf.Rad2Deg;
        //lightningAttackGO.transform.Rotate(new Vector3(0, 0, -startAngle));
        //Debug.Log(startAngle);
        //Debug.Log(a);
        //Debug.Log(b);
        //Debug.Log(c);


        //SPOT B
        float b2 = Vector2.Distance(lightningAttackGO.transform.position, new Vector2(lightningAttackGO.transform.position.x, spotB.transform.position.y));
        float c2 = Vector2.Distance(lightningAttackGO.transform.position, spotB.transform.position);
        float a2 = Vector2.Distance(new Vector2(lightningAttackGO.transform.position.x, spotB.transform.position.y), spotB.transform.position); ;
        //endAngle = Mathf.Acos(((b2 * b2) + (c2 * c2) - (a2 * a2)) / (2 * b2 * c2)) * Mathf.Rad2Deg;
        //lightningAttackGO.transform.Rotate(new Vector3(0, 0, endAngle));
        //Debug.Log(endAngle);
        //Debug.Log(a2);
        //Debug.Log(b2);
        //Debug.Log(c2);

        if (type == 0)
        {
            startAngle = -Mathf.Acos(((b * b) + (c * c) - (a * a)) / (2 * b * c)) * Mathf.Rad2Deg;
            endAngle = Mathf.Acos(((b2 * b2) + (c2 * c2) - (a2 * a2)) / (2 * b2 * c2)) * Mathf.Rad2Deg;
            lightningAttackGO.transform.Rotate(new Vector3(0, 0, startAngle));
        }
        else
        {
            endAngle = -Mathf.Acos(((b * b) + (c * c) - (a * a)) / (2 * b * c)) * Mathf.Rad2Deg;
            startAngle = Mathf.Acos(((b2 * b2) + (c2 * c2) - (a2 * a2)) / (2 * b2 * c2)) * Mathf.Rad2Deg;
            lightningAttackGO.transform.Rotate(new Vector3(0, 0, startAngle));
        }

        lightingingStartTime = Time.time;
        isAttackLightninging = true;
    }

    private void UpdateLightningState()
    {
        if (Time.time >= lightingingStartTime + lightningAnimationDuration + lightningingDuration)
        {
            SwitchState(State.Waiting);
        }
        else
        {
            if (Time.time >= lightingingStartTime + lightningAnimationDuration)
            {
                float angle = Lerp(startAngle, endAngle, (lightingingStartTime + lightningAnimationDuration), lightningingDuration);
                lightningAttackGO.transform.rotation = Quaternion.Euler(0, 0, angle);
                lightningAttackGO.SetActive(true);
            }
            else
            {
                //animation
            }
        }
    }

    private void ExitLightningState()
    {
        isAttackLightninging = false;

        lightningAttackGO.SetActive(false);
    }
    #endregion

    //---------SUMMON----------------
    #region SUMMON
    private void EnterSummonState()
    {
        summonStartTime = Time.time;
        firstLoop = true;
    }

    private void UpdateSummonState()
    {
        if (Time.time >= summonStartTime + summonDuration)
        {
            SwitchState(randomBehaviour());
        }
        else
        {
            if (Time.time >= summonStartTime + summonPreDuration && firstLoop)
            {
                firstLoop = false;
                Instantiate(summon, new Vector3(sprite.transform.position.x, sprite.transform.position.y - 1f, sprite.transform.position.z), sprite.transform.rotation);
            }
        }
    }

    private void ExitSummonState()
    {

    }

    #endregion

    //---------KNOCKBACK---------------
    #region KNOCKBACK
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        //sprite.transform.localScale -= new Vector3(0.0f, 0.5f, 0.0f);
        isKnockingBack = true;
        areaLightnings.transform.gameObject.SetActive(true);
        animatorSprite.Play("AreaLightning");
        firstLoop = true;
    }

    private void UpdateKnockbackState()
    {
        if (Time.time >= knockbackStartTime + knockbackAnimation + knockbackDuration)
        {
            particlesArea.transform.parent.GetComponent<Animator>().Play("noAnimationArea");
            SwitchState(State.Walking);
        }
        else
        {
            if (Time.time >= knockbackStartTime + knockbackAnimation && firstLoop)
            {
                firstLoop = false;
                particlesArea.transform.position = sprite.transform.position;
                particlesArea.transform.parent.GetComponent<Animator>().Play("AreaLightningCollider");
                animatorSprite.Play("noAnimationSprite");
                areaLightnings.transform.gameObject.SetActive(false);
                particlesArea.Play();
            }
        }
    }

    private void ExitKnockbackState()
    {
        //sprite.transform.localScale += new Vector3(0.0f, 0.5f, 0.0f);

        isKnockingBack = false;
        
    }
    #endregion

    //---------DEAD---------------
    #region DEAD
    private void EnterDeadState()
    {
        //Spawn chunks and blood
        //Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
        //Instantiate(deathBloodParticle, transform.position, deathBloodParticle.transform.rotation);
        //Instantiate(thornsSoul, transform.position, thornsSoul.transform.rotation);
        globalLight.intensity = startIntensity;
        GameObject p = Instantiate(portal, portalSpawn.position, portal.transform.rotation);
        p.GetComponent<EndLevel>().nextLevelName = "Bossw1";
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
    }

    public override void Damage(float[] attackDetails, bool wantKnockback)
    {
        if (attackDetails[0] == 0 && attackDetails[1] == 0 && attackDetails[2] == 0)
        {
            if (!isAttackLightninging && isActivated && !rushing && !vulnerable && !(Mathf.Abs(sprite.transform.position.y - floor.transform.position.y) <= 3))
            {
                SwitchState(State.Lightning);

            }
            else
            {
                if(!isAttackLightninging && isActivated && !rushing && (vulnerable || (Mathf.Abs(sprite.transform.position.y - floor.transform.position.y) <= 3)))
                {
                    SwitchState(State.Knockback);
                }
            }

        }
        else {
            if (!isKnockingBack && vulnerable)
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
                    vulnerable = false;
                    SwitchState(State.Knockback);
                }
                else if (currentHealth <= 0.0f)
                {
                    SwitchState(State.Dead);
                    GameManager.Instance.addPoints(pointsToGive);
                }
            }
        }
    }

    public override void applyKnockback(float[] position)
    {
        //SwitchState(State.Knockback);
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
            case State.AttackRush:
                ExitAttackRushState();
                break;
            case State.Lightning:
                ExitLightningState();
                break;
            case State.Summon:
                ExitSummonState();
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
            case State.AttackRush:
                EnterAttackRushState();
                break;
            case State.Lightning:
                EnterLightningState();
                break;
            case State.Summon:
                EnterSummonState();
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

    private float Lerp(float start, float end, float timeStartedLerping, float lerpTime = 1)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;

        float percentageComplete = timeSinceStarted / lerpTime;

        float result = Vector3.Lerp(new Vector3(start, 0, 0), new Vector3(end, 0, 0), percentageComplete).x;

        return result;
    }

    public override void mostraMissatge()
    {
        Debug.Log("Soy Zeus");
    }
}
