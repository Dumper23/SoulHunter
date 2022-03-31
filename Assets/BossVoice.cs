using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVoice : FatherEnemy
{
    private BossRangeOfActivation rangeOfActivation;
    private enum State
    {
        Waiting,
        Projectiles,
        Plataform,
        Altars,
        Shield,
        SwitchFase,
        Dead
    }

    private State currentState;
    private State[] statesToRandomize;

    private bool isActivated = false,
        switchingFase = false,
        hasInmunity = true,
        firstLoop = true,
        hasAltars = false,
        isAltars = false,
        isStarting = true;

    public HealthBarBoss healthBar;

    public int pointsToGive = 100;

    private int actualFase = 0,
        numAltars = 2,
        numAltarsV2 = 4,
        numAltarsV3 = 8;

    private int[] activateAltars;

    [SerializeField]
    private float maxHealth = 500f, 
        waitingDuration = 2f,
        altarsPreDuration = 5f,
        altarsDuration = 2f,
        switchFaseDuration = 2f;

    private float currentHealth, 
        waitingStartTime,
        altarsStartTime,
        switchFaseStartTime;

    public List<AltarBehaviour> altars = new List<AltarBehaviour>();

    [SerializeField]
    private GameObject shield;

    // Start is called before the first frame update
    void Start()
    {
        rangeOfActivation = transform.Find("Range").gameObject.GetComponent<BoxCollider2D>().GetComponent<BossRangeOfActivation>();

        currentHealth = maxHealth;
        SwitchState(State.Waiting);
    }

    // Update is called once per frame
    void Update()
    {

        if (!isStarting)
        {
            bool found = false;

            foreach (AltarBehaviour o in altars)
            {
                if (o.gameObject.activeInHierarchy)
                {
                    found = true;
                }
            }

            if (!found)
            {
                hasAltars = false;
            }
            else
            {
                hasAltars = true;
            }
            if (!hasAltars)
            {
                hasInmunity = false;
            }
            else
            {
                hasInmunity = true;
            }
            if (!isAltars)
            {
                if (!hasInmunity)
                {
                    shield.SetActive(false);
                    SwitchState(State.Altars);
                }
                else
                {
                    shield.SetActive(true);
                }
            }
        }

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
            case State.Projectiles:
                UpdateProjectilesState();
                break;
            case State.Plataform:
                UpdatePlataformState();
                break;
            case State.Altars:
                UpdateAltarsState();
                break;
            case State.Shield:
                UpdateShieldState();
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
                SwitchState(State.Altars);
            }

        }
        else
        {
            if (Time.time >= waitingStartTime + waitingDuration)
            {
                if (!hasAltars) {
                    SwitchState(State.Altars);
                }
            }
        }
    }

    private void ExitWaitingState()
    {

    }
    #endregion

    //------PROJECTILES-----
    #region PROJECTILES
    private void EnterProjectilesState()
    {

    }
    private void UpdateProjectilesState()
    {

    }
    private void ExitProjectilesState()
    {

    }
    #endregion

    //-----PLATAFORM------
    #region PLATAFORM
    private void EnterPlataformState()
    {

    }
    private void UpdatePlataformState()
    {

    }
    private void ExitPlataformState()
    {

    }
    #endregion

    //-------ALTARS------
    #region ALTARS
    private void EnterAltarsState()
    {
        altarsStartTime = Time.time;
        firstLoop = true;
        isAltars = true;
    }
    private void UpdateAltarsState()
    {
        if (Time.time >= altarsStartTime + altarsPreDuration)
        {
            if (firstLoop)
            {
                firstLoop = false;
                activateAltars = new int[numAltars];
                bool isOK;

                //Altars to activate
                for (int i = 0; i < numAltars; i++)
                {
                    int attempt = 0;
                    isOK = false;
                    while (!isOK)
                    {
                        bool found = false;
                        attempt = Random.Range(0, altars.Count);

                        for (int j = 0; j < i; j++)
                        {
                            if (activateAltars[j] == attempt)
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            isOK = true;
                        }
                    }

                    activateAltars[i] = attempt;
                }

                foreach (int i in activateAltars)
                {
                    AltarBehaviour o = altars[i];
                    o.Activate();
                }
                isAltars = false;
            }
            if (Time.time >= altarsStartTime + altarsPreDuration + altarsDuration)
            {
                SwitchState(State.Waiting);
            }
            else
            {
                //animacio invoca altars
            }
        }
        else
        {
            //animacio de pocho sense escut
        }
    }
    private void ExitAltarsState()
    {
        isStarting = false;
    }
    #endregion

    //-------SHIELD------
    #region SHIELD
    private void EnterShieldState()
    {

    }
    private void UpdateShieldState()
    {

    }
    private void ExitShieldState()
    {

    }
    #endregion

    //------SWITCHFASE----
    #region SWITCHFASE
    private void EnterSwitchFaseState()
    {
        switch (actualFase)
        {
            case 1:
                //spriteAnimator.Play("boss1SwitchFaseAnimation1");

                break;
            case 2:
                numAltars = numAltarsV2;
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
            isStarting = true;
            hasInmunity = true;
            shield.SetActive(true);
            SwitchState(State.Altars);
        }
    }
    private void ExitSwitchFaseState()
    {
        switchingFase = false;
    }
    #endregion

    //------DEAD------
    #region DEAD
    private void EnterDeadState()
    {
        healthBar.gameObject.SetActive(false);
        //Instantiate(hardSkinSoul, sprite.transform.position + new Vector3(0, 1, 0), hardSkinSoul.transform.rotation);
        //GameObject p = Instantiate(portal, sprite.transform.position + new Vector3(0, 5, 0), portal.transform.rotation) as GameObject;
        //p.GetComponent<EndLevel>().nextLevelName = "L1W3";
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
            case State.Projectiles:
                ExitProjectilesState();
                break;
            case State.Plataform:
                ExitPlataformState();
                break;
            case State.Altars:
                ExitAltarsState();
                break;
            case State.Shield:
                ExitShieldState();
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
            case State.Projectiles:
                EnterProjectilesState();
                break;
            case State.Plataform:
                EnterPlataformState();
                break;
            case State.Altars:
                EnterAltarsState();
                break;
            case State.Shield:
                EnterShieldState();
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

    public override void applyKnockback(float[] position)
    {
        //Nothing
    }

    public override void Damage(float[] attackDetails, bool wantKnockback)
    {

        if (!switchingFase && !hasInmunity)
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
    }

    public override void mostraMissatge()
    {
        Debug.Log("IM A BOSS");
    }
}
