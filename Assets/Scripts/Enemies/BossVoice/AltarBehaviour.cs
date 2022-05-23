using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarBehaviour : FatherEnemy
{
    private enum State
    {
        Waiting,
        Dead
    }

    [SerializeField]
    private float maxHealth = 40;

    private State currentState;

    private float
    currentHealth;

    [SerializeField]
    private SpriteRenderer energy;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        Color color = energy.color;
        color.a = 0.8f;
        energy.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        Color tmp;

        if (currentHealth <= 0.25 * maxHealth)
        {
            tmp = energy.color;
            tmp.a = 0.8f * 0.25f;
            energy.color = tmp;
        }
        else
        {
            if (currentHealth <= 0.5 * maxHealth)
            {
                tmp = energy.color;
                tmp.a = 0.8f * 0.5f;
                energy.color = tmp;
            }
            else
            {
                if (currentHealth <= 0.75 * maxHealth)
                {
                    tmp = energy.color;
                    tmp.a = 0.8f * 0.75f;
                    energy.color = tmp;
                }
            }
        }


        switch (currentState)
        {
            case State.Waiting:
                UpdateWaitingState();
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

    }

    private void UpdateWaitingState()
    {

    }

    private void ExitWaitingState()
    {

    }
    #endregion

    //---------DEAD---------------
    #region DEAD
    private void EnterDeadState()
    {
        //Deactivation animation
        gameObject.SetActive(false);
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
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (state)
        {
            case State.Waiting:
                EnterWaitingState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        currentHealth = maxHealth;
        Color color = energy.color;
        color.a = 0.8f;
        energy.color = color;
    }

    public override void applyKnockback(float[] position)
    {
        
    }

    public override void Damage(float[] attackDetails, bool wantKnockback)
    {
        currentHealth -= attackDetails[0];

        //particleDamage.Play();

        if (currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
        }
    }

    public override void mostraMissatge()
    {
        Debug.Log("Altaaaaar");
    }
}
