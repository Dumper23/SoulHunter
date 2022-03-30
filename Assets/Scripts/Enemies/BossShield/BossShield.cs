using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShield : FatherEnemy
{
    private enum State
    {
        Waiting,
        Walking,
        Flipping,
        FrontAttack,
        Meteors,
        Venom,
        JumpAttack,
        SwitchFase,
        Dead
    }

    [SerializeField]
    private GameObject meteor,
        meteorsParticles,
        pointSpawnFront;

    private Transform player;

    private Rigidbody2D rb;

    public int pointsToGive = 100;

    private State currentState;
    private State[] statesToRandomize;

    [SerializeField]
    private int quantityMeteors = 20,
        quantityMeteorsV2 = 30,
        quantityMeteorsV3 = 50;

    private int previousValue;

    [SerializeField]
    private float speed = 2,
        speedV2 = 3,
        speedV3 = 4,
        frontAttackSpeed = 10f,
        frontAttackSpeedV2 = 20f,
        frontAttackSpeedV3 = 30f,
        maxHealth = 500f,
        waitingDuration = 2f,
        walkingDuration = 3f,
        walkingDurationV2 = 3f,
        walkingDurationV3 = 1.5f,
        flippingDuration = 2.5f,
        flippingDurationV2 = 2f,
        flippingDurationV3 = 1.5f,
        meteorsDuration = 5f,
        meteorsDurationV2 = 8f,
        meteorsDurationV3 = 8f,
        venomPreAnimationDuration = 1f,
        venomAnimationDuration = 5f,
        venomAnimationDurationV2 = 4f,
        venomAnimationDurationV3 = 2f,
        venomDuration = 10f,
        frontAttackStep1 = 0.75f,
        frontAttackStep2 = 0.25f,
        frontAttackStep3 = 0.5f,
        frontAttackDuration = 1f,
        frontAttackDurationV2 = 0.2f,
        meteorsAnimationDuration = 2.5f,
        meteorsAnimationDurationV2 = 2f,
        meteorsAnimationDurationV3 = 1.5f,
       // jumpAttackDuration = 5f,
        jumpDuration = 0.5f,
        jumpDurationV3 = 0.25f,
        jumpLevitationDuration = 3f,
        jumpLevitationDurationV3 = 2f,
        jumpDownDuration = 0.1f,
        maxHeight = 8f,
        sismicWaitDuration = 0.25f,
        sismicDuration = 1,
        switchFaseDuration = 2f;

    private float
        currentHealth,
        waitingStartTime,
        walkingStartTime,
        flippingStartTime,
        meteorsStartTime,
        venomStartTime,
        venomAnimationStartTime,
        frontAttackStartTime,
        meteorsAnimationStartTime,
        jumpAttackStartTime,
        sismicStartTime,
        switchFaseStartTime;

    private bool isRight,
        isActivated = false,
        meteoring = false,
        goFlip = false,
        resetWalking = true,
        venoming = false,
        switchingFase = false,
        isRightForFront,
        isFrontAttack = false,
        sismic = false;

    private int actualFase = 0;

    private BossRangeOfActivation rangeOfActivation;

    private VenomArea venomArea;

    private BoxCollider2D area;

    private GameObject sprite,
        meteorsParticlesGO,
        wallGO;

    public HealthBarBoss healthBar;

    [SerializeField]
    private ParticleSystem wallMovement,
        jumpImpact;

    [SerializeField]
    private GameObject
        jumpDamageArea,
        sismicEffect,
        shield,
        hardSkinSoul, 
        portal;

    private Vector3 startPosition,
        newPosition, 
        newPosition2;

    private Animator spriteAnimator;

    private string venomAnimation = "ShieldVenom",
        jumpAnimation = "ShieldJump";

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<playerController>().gameObject.transform;

        rangeOfActivation = transform.Find("Range").gameObject.GetComponent<BoxCollider2D>().GetComponent<BossRangeOfActivation>();
        venomArea = transform.Find("VenomArea").gameObject.GetComponent<BoxCollider2D>().GetComponent<VenomArea>();

        area = transform.Find("Area").gameObject.GetComponent<BoxCollider2D>();


        meteorsParticlesGO = Instantiate(meteorsParticles);
        meteorsParticlesGO.transform.position = new Vector3(area.transform.position.x + area.offset.x, area.transform.position.y - 5, area.transform.position.z);
        meteorsParticlesGO.SetActive(false);

        sprite = transform.Find("Sprite").gameObject;
        spriteAnimator = sprite.GetComponent<Animator>();

        rb = sprite.GetComponent<Rigidbody2D>();
        wallGO = sprite.transform.Find("Pivot").gameObject.transform.Find("Wall").gameObject;

        statesToRandomize = new State[3];

        statesToRandomize[0] = State.Meteors;
        statesToRandomize[1] = State.Venom;
        statesToRandomize[2] = State.FrontAttack;

        currentHealth = maxHealth;


        SwitchState(State.Waiting);

        if(player.position.x >= rb.transform.position.x)
        {
            isRight = true;
        }
        else
        {
            isRight = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        #region Flip
        if (isRight && player.position.x <= rb.transform.position.x)
        {
            isRight = false;
            if (currentState == State.Walking)
            {
                SwitchState(State.Flipping);
            }
            else
            {
                goFlip = true;
            }
  
        }
        else
        {
            if (!isRight && player.position.x >= rb.transform.position.x)
            {
                isRight = true;
                if(currentState == State.Walking)
                {
                    SwitchState(State.Flipping);
                }
                else
                {
                    goFlip = true;
                }
            }
        }
        #endregion

        #region Meteors
        if (meteoring)
        {
            //Maybe canviarlo una mica
            Camera.main.GetComponent<Animator>().SetTrigger("shake");
            float value = Lerp(0, quantityMeteors, meteorsStartTime, meteorsDuration);
            if (previousValue < (int)value)
            {
                //generate Meteor
                GameObject meteor = MeteorsPool.meteorsPoolInstance.GetMeteor();
                
                Vector2 position = new Vector2(Random.Range(area.transform.position.x + meteor.transform.localScale.x, area.transform.position.x + area.size.x - meteor.transform.localScale.x), area.transform.position.y);
                
                meteor.transform.position = position;
                //float scale = Random.Range(0.2f, 1.4f);
                float scale = GetRandomValueForScale();
                meteor.transform.localScale = new Vector3(scale, scale, scale);
                meteor.SetActive(true);
                previousValue = (int)value;
            }
            if (value == quantityMeteors)
            {
                meteoring = false;
            }
        }
        #endregion

        #region Venom
        if (venoming)
        {
            if (Time.time >= venomStartTime + venomDuration)
            {
                venoming = false;
                venomArea.StopVenoming();
            }
            if (venomArea.IsVenomed())
            {
                player.GetComponent<playerController>().takeVenom();
            }
        }
        #endregion

        #region Front atack
        if (!wallGO.activeInHierarchy) {
            isFrontAttack = false;
            wallMovement.Stop();
        }

        if (isFrontAttack)
        {
            if (isRightForFront)
            {
                wallGO.transform.position = new Vector3(wallGO.transform.position.x + frontAttackSpeed * Time.deltaTime, wallGO.transform.position.y, 0);
            }
            else
            {
                wallGO.transform.position = new Vector3(wallGO.transform.position.x - frontAttackSpeed * Time.deltaTime, wallGO.transform.position.y, 0);
            }
            
        }
        #endregion

        #region Sismic
        if (sismic)
        {
            if(Time.time >= sismicStartTime + sismicWaitDuration)
            {
                shield.SetActive(true);
                sismicEffect.transform.position = new Vector3(sprite.transform.position.x, sismicEffect.transform.position.y, sismicEffect.transform.position.z);
                sismicEffect.SetActive(true);
                Camera.main.GetComponent<Animator>().SetTrigger("shake");
                if (Time.time >= sismicStartTime + sismicWaitDuration + sismicDuration)
                {
                    sismicEffect.SetActive(false);
                    sismic = false;
                    spriteAnimator.Play("ShieldIdle");
                }
            }
        }
        #endregion

        #region Switch fase
        if (healthBar.GetPercentageOfHealth() <= 0.66 && actualFase == 1)
        {
            actualFase = 2;
            //previousFase = 1;
            SwitchState(State.SwitchFase);

        }
        if (healthBar.GetPercentageOfHealth() <= 0.33 && actualFase == 2)
        {
            actualFase = 3;
            //previousFase = 2;
            SwitchState(State.SwitchFase);
        }
        #endregion

        switch (currentState)
        {

            case State.Waiting:
                UpdateWaitingState();
                break;
            case State.Walking:
                UpdateWalkingState();
                break;
            case State.Flipping:
                UpdateFlippingState();
                break;
            case State.FrontAttack:
                UpdateFrontAttackState();
                break;
            case State.Meteors:
                UpdateMeteorsState();
                break;
            case State.Venom:
                UpdateVenomState();
                break;
            case State.JumpAttack:
                UpdateJumpAttackState();
                break;
            case State.SwitchFase:
                UpdateSwitchFaseState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    //-------WAITING-----------
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
                waitingStartTime = Time.time;
                healthBar.SetMaxHealth(maxHealth);
                healthBar.gameObject.SetActive(true);
                isActivated = true;
                actualFase = 1;
            }

        }
        else
        {
            if (Time.time >= waitingStartTime + waitingDuration)
            {
                if (!goFlip)
                {
                    SwitchState(State.Walking);
                }
                else
                {
                    SwitchState(State.Flipping);
                }
            }
        }
    }

    private void ExitWaitingState()
    {

    }
    #endregion

    //------WALKING-------------
    #region WALKING
    private void EnterWalkingState()
    {
        if (resetWalking)
        {
            walkingStartTime = Time.time;
        }
    }

    private void UpdateWalkingState()
    {
        //transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime);
        rb.MovePosition(newPos);
        if(Time.time >= walkingStartTime + walkingDuration)
        {
            SwitchState(RandomBehaviour());
        }
    }

    private void ExitWalkingState()
    {
        if (Time.time >= walkingStartTime + walkingDuration)
        {
            resetWalking = true;
        }
        else
        {
            resetWalking = false;
        }
    }
    #endregion

    //---------FLIPPING----------
    #region FLIPPING
    private void EnterFlippingState()
    {
        flippingStartTime = Time.time;
        goFlip = false;
    }

    private void UpdateFlippingState()
    {
        if(Time.time >= flippingStartTime + flippingDuration)
        {
            SwitchState(State.Walking);
        }
    }

    private void ExitFlippingState()
    {
        if (Time.time >= flippingStartTime + flippingDuration && ((!isRight && sprite.transform.localRotation.y >= 0) || (isRight && sprite.transform.localRotation.y < 0)))
        {
            sprite.transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        
    }
    #endregion

    //---------FRONTATTACK--------
    #region FRONTATTACK
    private void EnterFrontAttackState()
    {
        frontAttackStartTime = Time.time;
        isRightForFront = isRight;
        spriteAnimator.Play("ShieldFront");
    }

    private void UpdateFrontAttackState()
    {

        if (Time.time >= frontAttackStartTime + frontAttackStep1)
        {
            if (Time.time >= frontAttackStartTime + frontAttackStep1 + frontAttackStep2)
            {
                if (Time.time >= frontAttackStartTime + frontAttackStep1 + frontAttackStep2 + frontAttackStep3)
                {
                    if (Time.time >= frontAttackStartTime + frontAttackStep1 + frontAttackStep2 + frontAttackStep3 + frontAttackDuration)
                    {
                        if (!goFlip)
                        {
                            SwitchState(State.Walking);
                        }
                        else
                        {
                            SwitchState(State.Flipping);
                        }
                    }
                    else
                    {
                        //"moure"
                        wallMovement.Play();
                        isFrontAttack = true;
                    }
                }
                else
                {
                    //animacio cop a paret
                }
            }
            else
            {
                //puja paret + animacio cop
                wallGO.transform.position = pointSpawnFront.transform.position;
                wallGO.SetActive(true);
            }
        }
        else
        {
            //animacio cop
        }
    }

    private void ExitFrontAttackState()
    {
        spriteAnimator.Play("ShieldIdle");
    }
    #endregion

    //-----------METEORS---------
    #region METEORS
    private void EnterMeteorsState()
    {
        meteorsParticlesGO.SetActive(false);
        meteorsAnimationStartTime = Time.time;
        previousValue = 0;
        spriteAnimator.Play("ShieldMeteors");
    }

    private void UpdateMeteorsState()
    {
        if (Time.time >= meteorsAnimationStartTime + meteorsAnimationDuration)
        {
            meteorsStartTime = Time.time;
            if (!goFlip)
            {
                SwitchState(State.Walking);
            }
            else
            {
                SwitchState(State.Flipping);
            }
            meteorsParticlesGO.SetActive(true);
            meteoring = true;
        }

    }

    private void ExitMeteorsState()
    {
        spriteAnimator.Play("ShieldIdle");
    }
    #endregion

    //---------VENOM----------
    #region VENOM
    private void EnterVenomState()
    {
        venomAnimationStartTime = Time.time;
        //play animation
        spriteAnimator.Play(venomAnimation);
    }

    private void UpdateVenomState()
    {
        if(Time.time >= venomAnimationStartTime + venomPreAnimationDuration)
        {
            if (!venoming)
            {
                venomStartTime = Time.time;
                venomArea.StartVenoming(sprite.transform.position);
            }
            venoming = true;
            if (Time.time >= venomStartTime + venomAnimationDuration)
            {
                if (!goFlip)
                {
                    SwitchState(State.Walking);
                }
                else
                {
                    SwitchState(State.Flipping);
                }
            }
        }

    }

    private void ExitVenomState()
    {
        spriteAnimator.Play("ShieldIdle");
    }
    #endregion

    //----------JUMPATTACK-----
    #region JUMPATTACK
    private void EnterJumpAttackState()
    {
        jumpAttackStartTime = Time.time;
        startPosition = sprite.transform.position;
        rb.gravityScale = 0;
        spriteAnimator.Play(jumpAnimation);
    }

    private void UpdateJumpAttackState()
    {
        
        if (Time.time >= jumpAttackStartTime + jumpDuration)
        {

            if (Time.time >= jumpAttackStartTime + jumpDuration + jumpLevitationDuration)
            {
                //caient
                float height = Lerp(newPosition2.y, startPosition.y, jumpAttackStartTime + jumpDuration + jumpLevitationDuration, jumpDownDuration);
                sprite.transform.position = new Vector3(newPosition2.x, height, newPosition2.z);
                shield.SetActive(false);
                if (Time.time >= jumpAttackStartTime + jumpDuration + jumpLevitationDuration + jumpDownDuration)
                {
                    sismicStartTime = Time.time;
                    sismic = true;
                    Camera.main.GetComponent<Animator>().SetTrigger("shake");
                    jumpImpact.Play();
                    if (!goFlip)
                    {
                        SwitchState(RandomBehaviour());
                    }
                    else
                    {
                        SwitchState(State.Flipping);
                    }
                }
            }
            else
            {
                if ((!isRight && sprite.transform.localRotation.y >= 0) || (isRight && sprite.transform.localRotation.y < 0))
                {
                    sprite.transform.Rotate(0.0f, 180.0f, 0.0f);
                }
                //levitant
                float pos = Lerp(newPosition.x, player.position.x, jumpAttackStartTime + jumpDuration, jumpLevitationDuration);
                sprite.transform.position = new Vector3(pos, newPosition.y, newPosition.z);
                newPosition2 = sprite.transform.position;
                jumpDamageArea.SetActive(true);
            }
        }
        else
        {
            //pujant
            float height = Lerp(startPosition.y, startPosition.y + maxHeight, jumpAttackStartTime, jumpDuration);
            sprite.transform.position = new Vector3(startPosition.x, height, startPosition.z);
            newPosition = sprite.transform.position;
        }
    }

    private void ExitJumpAttackState()
    {
        jumpDamageArea.SetActive(false);
        rb.gravityScale = 10;
    }
    #endregion

    //---------SWITCHFASE--------------
    #region SWITCHFASE
    private void EnterSwitchFaseState()
    {
        switch (actualFase)
        {
            case 1:
                //spriteAnimator.Play("boss1SwitchFaseAnimation1");

                break;
            case 2:

                meteorsAnimationDuration = meteorsAnimationDurationV2;
                meteorsDuration = meteorsDurationV2;
                quantityMeteors = quantityMeteorsV2;
                speed = speedV2;
                venomAnimationDuration = venomAnimationDurationV2;
                walkingDuration = walkingDurationV2;
                frontAttackSpeed = frontAttackSpeedV2;
                flippingDuration = flippingDurationV2;
                frontAttackDuration = frontAttackDurationV2;
                
                statesToRandomize = new State[7];

                statesToRandomize[0] = State.Meteors;
                statesToRandomize[1] = State.Venom;
                statesToRandomize[2] = State.FrontAttack;
                statesToRandomize[3] = State.Meteors;
                statesToRandomize[4] = State.Venom;
                statesToRandomize[5] = State.FrontAttack;
                statesToRandomize[6] = State.JumpAttack;

                venomAnimation = "ShieldVenomV2";
                //spriteAnimator.Play("boss1SwitchFaseAnimation2");
                break;
            case 3:

                meteorsAnimationDuration = meteorsAnimationDurationV3;
                meteorsDuration = meteorsDurationV3;
                quantityMeteors = quantityMeteorsV3;
                speed = speedV3;
                venomAnimationDuration = venomAnimationDurationV3;
                walkingDuration = walkingDurationV3;
                frontAttackSpeed = frontAttackSpeedV3;
                flippingDuration = flippingDurationV3;
                jumpDuration = jumpDurationV3;
                jumpLevitationDuration = jumpLevitationDurationV3;

                venomAnimation = "ShieldVenomV3";
                jumpAnimation = "ShieldVenomV2";
                //spriteAnimator.Play("boss1SwitchFaseAnimation3");
                break;
        }

        if ((!isRight && sprite.transform.localRotation.y >= 0) || (isRight && sprite.transform.localRotation.y < 0))
        {
            sprite.transform.Rotate(0.0f, 180.0f, 0.0f);
        }

        switchingFase = true;
        switchFaseStartTime = Time.time;
    }

    private void UpdateSwitchFaseState()
    {
        if (Time.time >= switchFaseStartTime + switchFaseDuration)
        {
            if (actualFase == 2 || actualFase == 3)
            {
                SwitchState(State.JumpAttack);
            }
            else
            {
                if (!goFlip)
                {
                    SwitchState(State.Walking);
                }
                else
                {
                    SwitchState(State.Flipping);
                }
            }
        }
    }

    private void ExitSwitchFaseState()
    {
        if ((!isRight && sprite.transform.localRotation.y >= 0) || (isRight && sprite.transform.localRotation.y < 0))
        {
            sprite.transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        //spriteAnimator.Play("noBoss1Animation");
        switchingFase = false;
    }
    #endregion

    //-----------DEAD-----------
    #region DEAD
    private void EnterDeadState()
    {
        healthBar.gameObject.SetActive(false);
        Instantiate(hardSkinSoul, sprite.transform.position, hardSkinSoul.transform.rotation);
        Instantiate(portal, sprite.transform.position + new Vector3(3,0,0), portal.transform.rotation);
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
            case State.Walking:
                ExitWalkingState();
                break;
            case State.Flipping:
                ExitFlippingState();
                break;
            case State.FrontAttack:
                ExitFrontAttackState();
                break;
            case State.Meteors:
                ExitMeteorsState();
                break;
            case State.Venom:
                ExitVenomState();
                break;
            case State.JumpAttack:
                ExitJumpAttackState();
                break;
            case State.SwitchFase:
                ExitSwitchFaseState();
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
            case State.Flipping:
                EnterFlippingState();
                break;
            case State.FrontAttack:
                EnterFrontAttackState();
                break;
            case State.Meteors:
                EnterMeteorsState();
                break;
            case State.Venom:
                EnterVenomState();
                break;
            case State.JumpAttack:
                EnterJumpAttackState();
                break;
            case State.SwitchFase:
                EnterSwitchFaseState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }

    public override void Damage(float[] attackDetails, bool wantKnockback)
    {

        if (!switchingFase)
        {
            if (!((isRight && sprite.transform.localRotation.y >= 0) || (!isRight && sprite.transform.localRotation.y < 0))) {
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
        }
    }

    public override void applyKnockback(float[] position)
    {
        //Nothing
    }


    private float Lerp(float start, float end, float timeStartedLerping, float lerpTime = 1)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;

        float percentageComplete = timeSinceStarted / lerpTime;

        float result = Vector3.Lerp(new Vector3(start, 0, 0), new Vector3(end, 0, 0), percentageComplete).x;

        return result;
    }

    private State RandomBehaviour()
    {

        if (meteoring && venoming && isFrontAttack)
        {
            if (actualFase == 3)
            {
                return State.JumpAttack;
            }
            else
            {
                return State.Walking;
            }
        }

        bool isOk = false;
        while (!isOk)
        {
            int pos = Random.Range(0, (statesToRandomize.Length));
            if (!(statesToRandomize[pos] == State.Meteors && meteoring) 
                && !(statesToRandomize[pos] == State.Venom && venoming) 
                && !(statesToRandomize[pos] == State.FrontAttack && isFrontAttack) 
                && !(statesToRandomize[pos] == State.JumpAttack && sismic))
            {
                return statesToRandomize[pos];
            }
        }
        //no hauria
        return State.Walking;
    }

    private float GetRandomValueForScale()
    {
        float rand = Random.value;
        if (rand >= 0.3f) //70%
            return Random.Range(0.3f, 0.8f);
        if (rand >= 0.1f)
            return Random.Range(0.8f, 1.1f);

        return Random.Range(1.1f, 1.4f);
    }
    public override void mostraMissatge()
    {
        Debug.Log("Im a BOSS");
    }

}
