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
    public float attackAnimationDelay = 0.25f;

    private float nextAttackTime = 0f;
    private float attackUptime = 1f;
    private bool isAttacking = false;
    

    [Header("Boost settings")]
    public int soulBarSpeed = 3;
    public float shieldRecoveryTime = 6f;

    private float originalPlayerSpeed;
    private int originalPlayerAttackDamage;
    private float startKillTime;
    private bool shielded = false;
    private float nextShield = 3f;

    [Header("UI settings")]
    public RawImage dashIndicator;
    public RawImage jumpIndicator1;
    public RawImage jumpIndicator2;
    public RawImage live1;
    public RawImage live2;
    public RawImage live3;
    public RawImage shield;
    public Text attackIndicator;
    public Text speedIndicator;
    public Text shieldTimer;

    [Header("Sound Settings")]
    public List<AudioSource> playerSounds = new List<AudioSource>();

    [Header("LostSouls Settings")]
    public Dictionary<string, LostSouls> lostSouls = new Dictionary<string, LostSouls>();
    public Transform ThornsPoint;
    public float ThornsRange = 0.5f;


    //Other settings
    private Transform t;
    private Rigidbody2D r2d;
    private SpriteRenderer sprite;
    private Animator animator;
    private string currentState;
    private Transform groundCollider;
    private bool immune = false;
    private Collision2D[] enemy = new Collision2D[10];
    private int enemyCounter = 0;

    //Animation States
    const string PLAYER_IDLE = "idle";
    const string PLAYER_ATTACK = "attack";
    const string PLAYER_DASH = "dash";
    const string PLAYER_RUN = "run";
    const string PLAYER_FALL = "fall";
    const string PLAYER_JUMP = "jump";
    const string PLAYER_WALLSLIDE = "wallSlide";
    const string PLAYER_ATTACKUP = "attackUp";
    

    void Start()
    {
        startKillTime = Time.time;
        originalPlayerSpeed = playerVelocity;
        originalPlayerAttackDamage = attackDamage;

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

    //-----------TODO: FixedUpdate (All the physics calculation and Update All the imput recievement)
    
    void Update()
    {
        attackIndicator.text = "" + attackDamage;
        speedIndicator.text = "" + Mathf.RoundToInt(playerVelocity);
        
        // Movement controlls
        #region Movement
        float horizontalIn = Input.GetAxis("Horizontal");
        bool wantsToJump = Input.GetButtonDown("Jump");
        bool isGrounded = Physics2D.OverlapCircle(groundCollider.position, 0.15f, LayerMask.GetMask("Ground"));

        r2d.velocity = new Vector2(horizontalIn * playerVelocity, r2d.velocity.y);

        if (!isDashing && !wallSliding && !isAttacking)
        {
            if (isGrounded)
            {
                if (horizontalIn != 0)
                {
                    changeAnimationState(PLAYER_RUN);
                }
                else
                {
                    changeAnimationState(PLAYER_IDLE);
                }
            }
            else
            {
                if (r2d.velocity.y > 0)
                {
                    changeAnimationState(PLAYER_JUMP);
                }
                else if (r2d.velocity.y < 0)
                {
                    changeAnimationState(PLAYER_FALL);
                }

            }
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
            //----------------------------------------------------------NO ENTENC EL HVELOCITY

            float hVelocity = r2d.velocity.x;
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
            Invoke("damageImmunity", 0.25f);
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
            changeAnimationState(PLAYER_DASH);
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
            changeAnimationState(PLAYER_WALLSLIDE);
        }
        else
        {
            wallSliding = false;
        }

        if (wallSliding)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, Mathf.Clamp(r2d.velocity.y, -wallSlidingSpeed, Mathf.Infinity));

            if (wantsToJump)
            {
                if (frontCheck.position.x > transform.position.x)
                {
                    r2d.velocity = new Vector2(-100, r2d.velocity.y);
                }
                else
                {
                    r2d.velocity = new Vector2(100, r2d.velocity.y);
                }
                wallSliding = false;
            }
            //Aqui si funciones bé el velocity del rigidbody podriem fer salt de paret :(
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
                isAttacking = true;
                //Play attack animation
                if (isGrounded)
                {
                    changeAnimationState(PLAYER_ATTACK);
                }

                if (!isGrounded && !isDashing && !wallSliding)
                {   
                    if (r2d.velocity.y > 0)
                    {
                        changeAnimationState(PLAYER_ATTACKUP);
                    }
                    else if (r2d.velocity.y < 0)
                    {

                    }
                }
                Invoke("stopAttack", attackAnimationDelay);

                Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, new Vector2(attackRange, attackRange), enemyLayer);

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

        t += Mathf.RoundToInt(Time.time);

        //Functionality that substracts souls each second
        if(points <= 0)
        {
            points = 0;
            GameManager.Instance.loadPoints(points);
        }

        if (Time.time - startKillTime > 1) 
        {
            startKillTime = Time.time;
            if (points >= 0 && points - soulBarSpeed >= 0)
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
        if (GameManager.Instance.getPoints() >= GameManager.Instance.getMaxPoints() * 0.25)
        {
            playerVelocity = originalPlayerSpeed + (originalPlayerSpeed/4);
        }
        else
        {
            playerVelocity = originalPlayerSpeed;
        }

        if (GameManager.Instance.getPoints() >= GameManager.Instance.getMaxPoints() * 0.5)
        {
            attackDamage = Mathf.RoundToInt(originalPlayerAttackDamage + (originalPlayerAttackDamage / 2));
        }
        else
        {
            attackDamage = originalPlayerAttackDamage;
        }

        if (GameManager.Instance.getPoints() >= GameManager.Instance.getMaxPoints() * 0.75)
        {
            if (!shielded)
            {
                if (Time.time - nextShield > shieldRecoveryTime)
                {
                    nextShield = Time.time;
                    shield.enabled = true;
                    shield.color = new Color(255, 255, 255, 255);
                    shielded = true;
                }
                else
                {
                    shieldTimer.enabled = true;
                }
                
            }
            else
            {
                shieldTimer.enabled = false;
                nextShield = Time.time;
            }
        }
        else
        {
            shield.enabled = false;
            shielded = false;
            shieldTimer.enabled = false;
        }

        #endregion

        shieldTimer.text = 10 - (Time.time - nextShield) + "s";
    }

    void stopAttack() 
    {
        isAttacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        #region LostSoul colision

        if(collision.transform.tag == "LostSoul")
        {
            lostSouls.Add(collision.transform.GetComponent<LostSouls>().lostSoulName, collision.transform.GetComponent<LostSouls>());
            Destroy(collision.gameObject);
        }

        #endregion

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
            //Reaction to damage
            if (collision.transform.tag == "EnemyFlyMele" || collision.transform.tag == "Enemy")
            {
                if (!shielded)
                {
                    playerSounds[4].Play();
                    playerLives--;
                    //Activate Thorns here
                }
                else
                {
                    shield.color = new Color(0, 0, 0, 0);
                    shielded = false;
                    shieldTimer.enabled = true;
                    playerSounds[4].Play();

                }
                if (collision.transform.tag == "EnemyFlyMele")
                {
                    collision.transform.GetComponentInParent<Enemy_fly_melee>().applyKnockback();
                }
                immune = true;
                Invoke("damageImmunity", 1f);
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
        Gizmos.DrawWireCube(attackPoint.position, new Vector3(attackRange, attackRange, 0));
        Gizmos.DrawWireSphere(ThornsPoint.position, ThornsRange);
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

    //Function that allows us to play any animation of the animator (Avoiding horrible web structures)
    private void changeAnimationState(string newState)
    {
        //We avoid playing the same animation multiple times
        if (currentState == newState) return;

        //We play a determinated animation
        animator.Play(newState);

        currentState = newState;
    }

    //function to Invoke that makes the immunity false
    private void damageImmunity()
    {
        immune = false;
    }

    private void lostSoulsFunctionality()
    {
        if (lostSouls.ContainsKey("Thorns"))
        {
            lostSouls.TryGetValue("Thorns", out LostSouls ls);
            if (ls.isEquiped && ls.isActive)
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(ThornsPoint.position, ThornsRange, enemyLayer);

                foreach (Collider2D enemy in hitEnemies)
                {
                    Debug.Log(enemy.name);
                }
            }
        }
    }
}
