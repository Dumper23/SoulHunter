using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_bush : FatherEnemy
{

    private enum State
    {
        Waiting,
        Shooting,
        Dead
    }

    private Transform player;
    private Rigidbody2D rb;

    [SerializeField]
    private float lineOfSite,
       maxHealth = 20,
       fireRate = 1;

    public int pointsToGive = 10;

    public int soulsToGive = 5;
    public GameObject soul;
    public float soulForce;

    [SerializeField]
    private GameObject
        deathChunkParticle,
        deathBloodParticle;

    [SerializeField]
    private ParticleSystem particleDamage;

    private State currentState;

    private float
        currentHealth,
        nextFireTime;

    [SerializeField]
    private float moveSpeedBullet = 3f,
        rotateSpeedBullet = 100f;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindObjectOfType<playerController>().transform;
    }

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            case State.Waiting:
                UpdateWaitingState();
                break;
            case State.Shooting:
                UpdateShootingState();
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
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer < lineOfSite)
        {
            SwitchState(State.Shooting);
        }
    }

    private void ExitWaitingState()
    {

    }
    #endregion

    //---------SHOOTING----------
    #region SHOOTING
    private void EnterShootingState()
    {

    }

    private void UpdateShootingState()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer > lineOfSite)
        {
            SwitchState(State.Waiting);
        }
        else
        {
            if ((distanceFromPlayer < lineOfSite && nextFireTime < Time.time) && player.gameObject.activeInHierarchy)
            {
                //GetComponent<fireSpikes>().Shoot();
                nextFireTime = Time.time + fireRate;

                Vector3 dist = (player.transform.position - transform.position).normalized;

                GameObject bul = bulletPool.bulletPoolInstance.GetBullet();
                bul.transform.position = transform.position;
                bul.transform.rotation = transform.rotation;
                bul.SetActive(true);
                bul.GetComponent<bulletSeeker>().isSeeking = true;
                bul.GetComponent<bulletSeeker>().SetMoveDirection(dist);
                bul.GetComponent<bulletSeeker>().SetTarget(player);
                bul.GetComponent<bulletSeeker>().SetMoveSpeed(moveSpeedBullet);
                bul.GetComponent<bulletSeeker>().SetRotationSpeed(rotateSpeedBullet);

            }
        }
    }

    private void ExitShootingState()
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
        for (int i = 0; i <= soulsToGive; i++)
        {
            GameObject g = Instantiate(soul, transform.position, Quaternion.identity);
            g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * soulForce, ForceMode2D.Impulse);
        }
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
            case State.Shooting:
                ExitShootingState();
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
            case State.Shooting:
                EnterShootingState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }

    public override void applyKnockback(float[] position)
    {
        
    }

    public override void Damage(float[] attackDetails, bool wantKnockback)
    {
        currentHealth -= attackDetails[0];

        particleDamage.Play();

        if (currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
            GameManager.Instance.addPoints(pointsToGive);
        }
    }

    public override void mostraMissatge()
    {
        Debug.Log("sporer");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSite);
    }
}
