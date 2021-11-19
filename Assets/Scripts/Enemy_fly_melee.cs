using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy_fly_melee : MonoBehaviour
{
    private enum State
    {
        Walking,
        Knockback,
        Dead
    }


    public int damageToPlayer = 1;
    public int pointsToGive = 10;

    private State currentState;

    [SerializeField]
    private float
        maxHealth,
        knockbackDuration;

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

    private Vector2 movement;

    private float
        currentHealth,
        knockbackStartTime;


    private Rigidbody2D rb;

    private AIPath aiPath;


    [SerializeField]
    private ParticleSystem particleDamage;


    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>();

        currentHealth = maxHealth;
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
        }

    }

    //---------WALKING---------------
    #region WALKING
    private void EnterWalkingState()
    {
        aiPath.canMove = true;
    }

    private void UpdateWalkingState()
    {

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
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirectionX, knockbackSpeed.y * damageDirectionY);
        rb.velocity = movement;
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


    public void Damage(float[] attackDetails)
    {


        currentHealth -= attackDetails[0];

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
            SwitchState(State.Knockback);
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

    public void applyKnockback()
    {
        ExitWalkingState();
        SwitchState(State.Knockback);
    }

}
