using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class playerController : MonoBehaviour
{
    [Header("Basic Movement settings")]
    public float playerVelocity = 10f;
    public float jumpVelocity = 12f;
    public float gravityScale = 4f;
    public int maxJumps = 2;
    public ParticleSystem jumpParticle;
    public ParticleSystem groundImpactParticles;
    public ParticleSystem walkParticles;
    public float startTimeTrail;

    private int availableJumps;
    private bool hittedGround = true;
    private float timeTrail;
    

    [Header("Dash settings")]
    public float dashForce = 40f;
    public float startDashTime = 0.25f;
    public float dashRecoveryTime = 1.5f;
    public ParticleSystem dashParticle;
    public float dashAlpha = 0.75f;
    public Ghost ghost;

    private float currentDashRecoveryTime;
    private bool isDashing;
    private float dashDirection;
    private float currentDashTime;
    private bool dashed = false;


    [Header("Wall Jump settings")]
    public Transform frontCheck;
    public float wallSlidingSpeed;
    public float checkRadius;

    private bool isTouchingFront;
    private bool wallSliding;

    [Header("Combat settings")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;
    public int attackDamage = 10;
    public float attackRate = 2f;
    public int playerLives = 3;
    public float relativeAttackPointPos = 0.58f;

    private float nextAttackTime = 0f;
    private float attackUptime = 1f;

    [Header("Boost settings")]
    public int soulBarSpeed = 3;

    private float originalPlayerSpeed;

    [Header("UI settings")]
    public RawImage dashIndicator;
    public RawImage jumpIndicator1;
    public RawImage jumpIndicator2;
    public RawImage live1;
    public RawImage live2;
    public RawImage live3;
    public Text attackIndicator;

    [Header("Sound Settings")]
    public List<AudioSource> playerSounds = new List<AudioSource>();

    //Other settings
    private Transform t;
    private Rigidbody2D r2d;
    private SpriteRenderer sprite;
    private Animator animator;
    private Transform groundCollider;
    private bool immune = false;
    private Collision2D[] enemy = new Collision2D[10];
    private int enemyCounter = 0;
    

    void Start()
    {
        originalPlayerSpeed = playerVelocity;
        //We get all the components we need
        sprite = GetComponent<SpriteRenderer>();
        r2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        t = gameObject.transform;

        //We set the rigidBody to freeze rotation, continuous colisions and the gravity scale
        r2d.freezeRotation = true;
        r2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        r2d.gravityScale = gravityScale * 0.75f;

        //We find the ground collider game object
        groundCollider = transform.Find("groundCollider");
    }


    void Update()
    {
        // Movement controlls
        #region Movement
        float horizontalIn = Input.GetAxis("Horizontal");
        bool wantsToJump = Input.GetButtonDown("Jump");
        
        animator.SetFloat("vSpeed", r2d.velocity.y);

        r2d.velocity = new Vector2(horizontalIn * playerVelocity, r2d.velocity.y);
        if(horizontalIn != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (availableJumps < 2)
        {
            jumpIndicator1.enabled = false;
        }
        if (availableJumps < 1)
        {
            jumpIndicator2.enabled = false;
        }
        if (availableJumps >= 2)
        {
            jumpIndicator1.enabled = true;
            jumpIndicator2.enabled = true;
        }

        

        bool isGrounded = Physics2D.OverlapCircle(groundCollider.position, 0.15f, LayerMask.GetMask("Ground"));
        animator.SetBool("isGrounded", isGrounded);
        //After the jump we create effects as we hit the ground
        if (isGrounded)
        {
            
            if (hittedGround)
            {
                Collider2D aux = Physics2D.OverlapCircle(groundCollider.position, 0.15f, LayerMask.GetMask("Ground"));
                if (aux.GetComponent<SpriteRenderer>() != null)
                {
                    groundImpactParticles.startColor = aux.GetComponent<SpriteRenderer>().color;
                }
                Instantiate(groundImpactParticles, groundCollider.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                Camera.main.GetComponent<Animator>().SetTrigger("shake");
                hittedGround = false;
                playerSounds[0].Play();
            }
            if (horizontalIn != 0)
            {
                if (timeTrail <= 0)
                {
                    Collider2D aux = Physics2D.OverlapCircle(groundCollider.position, 0.15f, LayerMask.GetMask("Ground"));
                    if (aux.GetComponent<SpriteRenderer>() != null)
                    {
                        walkParticles.startColor = aux.GetComponent<SpriteRenderer>().color;
                    }
                    Instantiate(walkParticles, groundCollider.position, Quaternion.identity);
                    timeTrail = startTimeTrail;
                    playerSounds[1].Play();
                }
                else
                {
                    timeTrail -= Time.deltaTime;
                }
            }
        }
        else
        {
            hittedGround = true;
        }


        if (isGrounded && !Input.GetButton("Jump"))
        {
            availableJumps = maxJumps;
        }

        if (wantsToJump && availableJumps > 0)
        {
            float hVelocity = r2d.velocity.y;

            r2d.velocity = new Vector2(hVelocity, jumpVelocity);
            availableJumps--;
            playerSounds[3].Play();
            
            jumpParticle.Play();
        }

        if (horizontalIn != 0)
        {
            sprite.flipX = (horizontalIn < 0);
        }
        #endregion

        //Dashing functionality
        #region Dash
        if ((Input.GetKeyDown(KeyCode.LeftShift) && !dashed))
        {
            ghost.makeGhost = true;
            currentDashRecoveryTime = dashRecoveryTime;
            isDashing = true;
            //dashParticle.Play();
            playerSounds[2].Play();
            dashIndicator.color = Color.red;
            currentDashTime = startDashTime;
            r2d.velocity = Vector3.zero;
            immune = true;
            if (horizontalIn > 0)
            {
                dashDirection = 1;
            }
            else
            {
                dashDirection = -1;
            }
        }
        Color tmp = sprite.color;
        if (isDashing)
        {
            animator.SetTrigger("isDashing");
            r2d.velocity = transform.right * dashDirection * dashForce;
            currentDashTime -= Time.deltaTime;
            
            tmp.a = dashAlpha;
            sprite.color = tmp;
            dashed = true;
            if (currentDashTime <= 0)
            {
                isDashing = false;
                tmp.a = 1;
                sprite.color = tmp;
            }
        }
        else
        {
            ghost.makeGhost = false;
            immune = false;
        }

        if (dashed)
        {
            if(currentDashRecoveryTime <= 0)
            {
                dashed = false;
                dashIndicator.color = Color.green;

            }
            else
            {
                currentDashRecoveryTime -= Time.deltaTime;
            }
        }

        #endregion

        //Wall Sliding functionality
        #region Wall Sliding

        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, LayerMask.GetMask("Ground"));
        if(isTouchingFront && !isGrounded && horizontalIn != 0)
        {
            wallSliding = true;
            animator.SetBool("isWallSliding", wallSliding);
        }
        else
        {
            wallSliding = false;
            animator.SetBool("isWallSliding", wallSliding);
        }

        if (wallSliding)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, Mathf.Clamp(r2d.velocity.y, -wallSlidingSpeed, float.MaxValue));

        }

        #endregion

        //Combat functionality
        #region Combat
        if (horizontalIn < 0)
        {
            attackPoint.position = new Vector3(-relativeAttackPointPos + transform.position.x, attackPoint.position.y, attackPoint.position.z);
        }
        else if(horizontalIn > 0)
        {
            attackPoint.position = new Vector3(relativeAttackPointPos + transform.position.x, attackPoint.position.y, attackPoint.position.z);
        }

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                //Play attack animation
                if (isGrounded)
                {
                    animator.SetTrigger("isAttacking");
                }
                
                if(!isGrounded)
                {
                    animator.SetBool("isAttackingUp", true);
                }

                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

                foreach (Collider2D enemy in hitEnemies)
                {
                    if(enemy.tag == "EnemyFlyMele")
                    {
                        float[] damageMessage = new float[3];
                        damageMessage[0] = attackDamage;
                        damageMessage[1] = transform.position.x;
                        damageMessage[2] = transform.position.y;
                        enemy.GetComponentInParent<Enemy_fly_melee>().Damage(damageMessage);
                    }

                    if (enemy.tag == "Enemy")
                    {
                        float[] damageMessage = new float[2];
                        damageMessage[0] = attackDamage;
                        damageMessage[1] = transform.position.x;
                        enemy.GetComponentInParent<BasicEnemyController>().Damage(damageMessage);
                    }
                }
                nextAttackTime = Time.time + 1 / attackRate;
            }
        }

        #endregion

        //Player Death functionality
        #region Player death
        if (playerLives <= 0)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            PlayerData data = PlayerSave.LoadPlayer();

            GameManager.Instance.loadPoints(data.points);
            playerLives = data.lives;

            attackDamage = data.attackDamage;
            attackRate = data.attackRate;

            Vector3 pos;
            pos.x = data.position[0];
            pos.y = data.position[1];
            pos.z = data.position[2];
            transform.position = pos;

            playerVelocity = data.speed;
            maxJumps = data.jumpAmount;
            updateLiveUI();

        }
        #endregion

        //Boost bar functionality
        #region Boost bar
        int points = GameManager.Instance.getPoints();
        //Augmento de daño, Dash recovery, Velocidad
        float t = 0f;

        t += Time.time;

        //Functionality that substracts souls each second
        if(points <= 0)
        {
            points = 0;
            GameManager.Instance.loadPoints(points);
        }

        if (t % 2 <= 0)
        {
            if (points >= 0)
            {
                points -= soulBarSpeed;
                GameManager.Instance.loadPoints(points);
            }
            else
            {
                points = 0;
                GameManager.Instance.loadPoints(points);
            }
        }

        //Boosts given at a certain % of souls
        if (GameManager.Instance.getPoints() >= 125 * 0.25)
        {
            playerVelocity = originalPlayerSpeed + (originalPlayerSpeed/4);
        }
        else
        {
            playerVelocity = originalPlayerSpeed;
        }

        #endregion
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        #region Dash immunity


        //Optimitzar no haver de comprovar cada cop si son enemic o enemic volador
        if ((collision.transform.tag == "Enemy" || collision.transform.tag == "EnemyFlyMele") && isDashing)
        {
            enemy[enemyCounter] = collision;
            enemyCounter++;

            collision.transform.GetComponentInChildren<Rigidbody2D>().isKinematic = true;

            if (collision.transform.tag == "Enemy")
            {
                collision.transform.GetComponentInChildren<BoxCollider2D>().isTrigger = true;
            }
            else
            {
                collision.transform.GetComponentInChildren<CircleCollider2D>().isTrigger = true;
            }
            Invoke("returnEnemyToCollision", 0.25f);
        }

        #endregion

        #region Damage to player

        if ((collision.transform.tag == "Enemy" || collision.transform.tag == "EnemyFlyMele") && !immune)
        {
            playerSounds[4].Play();

            //Reaction to damage
            if (collision.transform.tag == "EnemyFlyMele")
            {
                playerLives = playerLives - collision.transform.GetComponentInParent<Enemy_fly_melee>().damageToPlayer;
                collision.transform.GetComponentInParent<Enemy_fly_melee>().applyKnockback();
                r2d.velocity = (new Vector2((sprite.flipX ? 1 : -1) * 2 * playerVelocity, jumpVelocity));
            }

            if (collision.transform.tag == "Enemy")
            {
                playerLives = playerLives - collision.transform.GetComponentInParent<BasicEnemyController>().damageToPlayer;
                r2d.velocity = (new Vector2((sprite.flipX ? 1 : -1) * 2 * playerVelocity, jumpVelocity));
            }

            updateLiveUI();
        }
        #endregion
    }

    //Update the ui to display the correct number of lives
    private void updateLiveUI()
    {
        Color color = new Color();
        color.r = 255;
        color.g = 255;
        color.b = 255;
        color.a = 1f;
        if (playerLives == 3)
        {
            live1.color = color;
            live2.color = color;
            live3.color = color;
        }
        else if (playerLives == 2)
        {
            color.a = 1f;
            live1.color = color;
            live2.color = color;
            color.a = 0.2f;
            live3.color = color;
        }
        else
        {
            color.a = 1f;
            live1.color = color;
            color.a = 0.2f;
            live2.color = color;
            live3.color = color;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    //Makes the enemy colidable after the player passed through it with a dash or immune
    private void returnEnemyToCollision()
    {
        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i] != null)
            {
                enemy[i].transform.GetComponentInChildren<Rigidbody2D>().isKinematic = false;
                if (enemy[i].transform.tag == "Enemy")
                {
                    enemy[i].transform.GetComponentInChildren<BoxCollider2D>().isTrigger = false;
                }
                else
                {
                    enemy[i].transform.GetComponentInChildren<CircleCollider2D>().isTrigger = false;
                }
            }
        }
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i] = null;
        }

        enemyCounter = 0;
    }
}
