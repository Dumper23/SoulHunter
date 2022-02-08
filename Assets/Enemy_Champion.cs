using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Champion : MonoBehaviour
{
    private enum State
    {
        Waiting,
        Walking,
        AttackRoll,
        Defense,
        Knockback,
        Dead
    }

    private Transform player;
    private Rigidbody2D rb;

    public GameObject viewA;
    public GameObject viewB;

    public float speed = 2f;
    public float lineOfSite;

    private float
        currentHealth,
        knockbackStartTime,
        walkingStartTime;

    private State currentState;

    public bool swicher = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer < lineOfSite)
        {
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime);
            rb.MovePosition(newPos);
        }
        if (swicher)
        {
            viewA.SetActive(false);
            viewB.SetActive(true);
        }
        else
        {
            viewA.SetActive(true);
            viewB.SetActive(false);
        }
        /*
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
        }*/

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

    //---------WALKING---------------
    #region WALKING
    private void EnterWalkingState()
    {

    }

    private void UpdateWalkingState()
    {

    }

    private void ExitWalkingState()
    {

    }
    #endregion

    //---------ATTACKROLL---------------
    #region ATTACKROLL
    private void EnterAttackRollState()
    {

    }

    private void UpdateAttackRollState()
    {

    }

    private void ExitAttackRollState()
    {

    }
    #endregion

    //---------DEFENSE---------------
    #region DEFENSE
    private void EnterDefenseState()
    {

    }

    private void UpdateDefenseState()
    {

    }

    private void ExitDefenseState()
    {

    }
    #endregion

    //---------KNOCKBACK---------------
    #region KNOCKBACK
    private void EnterKnockbackState()
    {
       
    }

    private void UpdateKnockbackState()
    {
       
    }

    private void ExitKnockbackState()
    {

    }
    #endregion

    //---------DEAD---------------
    #region DEAD
    private void EnterDeadState()
    {

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
            case State.AttackRoll:
                ExitAttackRollState();
                break;
            case State.Defense:
                ExitDefenseState();
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
    }
}
