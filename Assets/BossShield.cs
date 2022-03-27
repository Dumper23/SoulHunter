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
        meteorsParticles;

    private Transform player;

    private Rigidbody2D rb;

    public int pointsToGive = 100;

    private State currentState;
    private State[] statesToRandomize;

    [SerializeField]
    private int quantityMeteors;

    private int previousValue;

    [SerializeField]
    private float speed,
        maxHealth = 200f,
        waitingDuration = 2f,
        walkingDuration = 3f,
        flippingDuration = 2.5f,
        meteorsDuration = 5f,
        venomPreAnimationDuration = 1f,
        venomAnimationDuration = 5f,
        venomDuration = 10f,
        meteorsAnimationDuration = 2.5f,
        switchFaseDuration = 2f;

    private float
        currentHealth,
        waitingStartTime,
        walkingStartTime,
        flippingStartTime,
        meteorsStartTime,
        venomStartTime,
        venomAnimationStartTime,
        meteorsAnimationStartTime,
        switchFaseStartTime;

    private bool isRight,
        isActivated = false,
        meteoring = false,
        goFlip = false,
        resetWalking = true,
        venoming = false,
        switchingFase = false;

    private int actualFase = 0;

    private BossRangeOfActivation rangeOfActivation;

    private VenomArea venomArea;

    private BoxCollider2D area;

    private GameObject sprite,
        meteorsParticlesGO;

    public HealthBarBoss healthBar;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<playerController>().gameObject.transform;

        rangeOfActivation = transform.Find("Range").gameObject.GetComponent<BoxCollider2D>().GetComponent<BossRangeOfActivation>();
        venomArea = transform.Find("VenomArea").gameObject.GetComponent<BoxCollider2D>().GetComponent<VenomArea>();

        area = transform.Find("Area").gameObject.GetComponent<BoxCollider2D>();

        meteorsParticlesGO = Instantiate(meteorsParticles);
        meteorsParticlesGO.transform.position = new Vector3(area.transform.position.x + area.offset.x, area.transform.position.y - 10, area.transform.position.z);
        meteorsParticlesGO.SetActive(false);

        sprite = transform.Find("Sprite").gameObject;
        rb = sprite.GetComponent<Rigidbody2D>();

        statesToRandomize = new State[2];

        statesToRandomize[0] = State.Meteors;
        statesToRandomize[1] = State.Venom;

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

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
    void Update()
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
                healthBar.gameObject.SetActive(true);
                isActivated = true;
                actualFase = 1;
            }

        }
        else
        {
            if (Time.time >= waitingStartTime + waitingDuration)
            {
                SwitchState(State.Walking);
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

    }

    private void UpdateFrontAttackState()
    {

    }

    private void ExitFrontAttackState()
    {

    }
    #endregion

    //-----------METEORS---------
    #region METEORS
    private void EnterMeteorsState()
    {
        meteorsParticlesGO.SetActive(false);
        meteorsAnimationStartTime = Time.time;
        previousValue = 0;
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

    }
    #endregion

    //---------VENOM----------
    #region VENOM
    private void EnterVenomState()
    {
        venomAnimationStartTime = Time.time;
        //play animation
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

    }
    #endregion

    //----------JUMPATTACK-----
    #region JUMPATTACK
    private void EnterJumpAttackState()
    {

    }

    private void UpdateJumpAttackState()
    {

    }

    private void ExitJumpAttackState()
    {

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
                //spriteAnimator.Play("boss1SwitchFaseAnimation2");
                break;
            case 3:
                //spriteAnimator.Play("boss1SwitchFaseAnimation3");
                break;
        }

        switchingFase = true;
        switchFaseStartTime = Time.time;
    }

    private void UpdateSwitchFaseState()
    {
        if (Time.time >= switchFaseStartTime + switchFaseDuration)
        {
            SwitchState(State.Waiting);
        }
    }

    private void ExitSwitchFaseState()
    {
        //spriteAnimator.Play("noBoss1Animation");
        switchingFase = false;
    }
    #endregion

    //-----------DEAD-----------
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
        //TMP
        if (meteoring && venoming)
        {
            return State.Walking;
        }

        bool isOk = false;
        while (!isOk)
        {
            int pos = Random.Range(0, (statesToRandomize.Length));
            if (!(statesToRandomize[pos] == State.Meteors && meteoring) && !(statesToRandomize[pos] == State.Venom && venoming))
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
