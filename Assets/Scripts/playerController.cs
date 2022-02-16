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
    public bool loadPlayerData = true;

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
    public ParticleSystem deathParticles;

    private int maxLives;
    private float nextAttackTime = 0f;
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
    public RawImage live4;
    public RawImage shield;
    public Text attackIndicator;
    public Text speedIndicator;
    public Text shieldTimer;
    public List<GameObject> lostSoulToggles = new List<GameObject>();
    public GameObject inventoryUI;

    [Header("Sound Settings")]
    public List<AudioSource> playerSounds = new List<AudioSource>();
    public GameObject deadSoundObject;
    public AudioSource generalAudios;
    public AudioClip deadSound;
    public AudioClip bladeSound;

    [Header("LostSouls Settings")]
    public Dictionary<string, LostSouls> lostSouls = new Dictionary<string, LostSouls>();
    public Transform ThornsPoint;
    public float ThornsRange = 0.5f;
    public GameObject light;
    private bool inventory = false;
    public int maxLostSoulsEquipped = 3;
    private int lostSoulsEquipped = 0;

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
    public string currentLevel = "Tutorial";
    private bool paused = false;

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
        if (loadPlayerData)
        {
            loadPlayer();
            GameObject startPos = GameObject.FindGameObjectWithTag("Start");
            if (startPos != null) {
                transform.position = startPos.transform.position;
            }
        }

        currentLevel = GameManager.Instance.getCurrentLevelName();

        maxLives = playerLives;
        light.SetActive(false);
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

    public void setCurrentLevelName(string levelName)
    {
        currentLevel = levelName;
    }

    private void loadPlayer()
    {
        PlayerData temp = PlayerSave.LoadPlayer();
        if (temp != null)
        {

            attackDamage = temp.attackDamage;
            attackRate = temp.attackRate;
            playerVelocity = temp.speed;
            transform.position = new Vector3(temp.position[0], temp.position[1]);
            currentLevel = temp.currentLevel;
            for (int i = 0; i < temp.lostSouls.Length; i++)
            {
                if (temp.lostSouls[i] != null)
                {
                    LostSouls ls = new LostSouls();
                    ls.lostSoulName = temp.lostSouls[i];
                    ls.isActive = true;
                    ls.isEquiped = false;
                    lostSouls.Add(temp.lostSouls[i], ls);
                }
            }
        }
    }

    //-----------TODO: FixedUpdate (All the physics calculation and Update All the imput recievement)

    [System.Obsolete]
    void Update()
    {
        attackIndicator.text = "" + attackDamage;
        speedIndicator.text = "" + Mathf.RoundToInt(playerVelocity);


        //Lost Souls functionality
        #region LostSouls
        //Lost souls inventory

        if (Input.GetButtonDown("Inventory") && !GameManager.Instance.isPaused()){
            inventory = !inventory;
            updateInventory();
            toggleInventory();
        }

        if (lostSouls.Count > 0)
        {
            lostSoulsFunctionality();
        }

        if (lostSouls.TryGetValue("Thorns", out LostSouls th))
        {
            if (th.isActive && th.isEquiped)
            {
                th.isActive = false;
            }
        }
        #endregion

        // Movement controlls
        if (!inventory && !paused)
        {
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
                    hittedGround = false;
                    if (r2d.velocity.y <= 0)
                    {
                        Instantiate(groundImpactParticles, groundCollider.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                        Camera.main.GetComponent<Animator>().SetTrigger("shake");
                        playerSounds[0].Play();
                    }
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
            if ((Input.GetButtonDown("Dash") || Input.GetAxisRaw("Dash") != 0) && !dashed)
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
                    dashDirection = (sprite.flipX ? -1 : 1);
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
                if (currentDashRecoveryTime <= 0)
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

            if (isTouchingFront && !isGrounded && horizontalIn != 0)
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

                /*if (wantsToJump)
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
                }*/
                //Aqui si funciones b� el velocity del rigidbody podriem fer salt de paret :(
            }

            #endregion

            //Combat functionality
            #region Combat
            if (horizontalIn < 0)
            {
                attackPoint.position = new Vector3(-relativeAttackPointPos + transform.position.x, attackPoint.position.y, attackPoint.position.z);
            }
            else if (horizontalIn > 0)
            {
                attackPoint.position = new Vector3(relativeAttackPointPos + transform.position.x, attackPoint.position.y, attackPoint.position.z);
            }

            if (Time.time >= nextAttackTime)
            {
                if (Input.GetButtonDown("Attack"))
                {
                    isAttacking = true;
                    generalAudios.clip = bladeSound;
                    generalAudios.Play();
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
                        if (enemy.tag == "Enemy")
                        {
                            float[] damageMessage = new float[3];
                            damageMessage[0] = attackDamage;
                            damageMessage[1] = transform.position.x;
                            damageMessage[2] = transform.position.y;
                            if (enemy.GetComponentInParent<FatherEnemy>() != null)
                            {
                                enemy.GetComponentInParent<FatherEnemy>().Damage(damageMessage);
                            }
                        }
                        if (enemy.tag == "Healer")
                        {
                            if(enemy.GetComponent<healer>() != null)
                            {
                                enemy.GetComponent<healer>().Damage(attackDamage, this);
                            }
                        }
                    }
                    nextAttackTime = Time.time + 1 / attackRate;
                }
            }

            #endregion
        }
        //Player Death functionality
        #region Player death
        if (playerLives <= 0)
        {
            updateLiveUI();
            deadSoundObject.GetComponent<AudioSource>().clip = deadSound;
            Instantiate(deadSoundObject, transform.position, transform.rotation);
            Invoke("die", 2f);
            (Instantiate(deathParticles, transform.position, Quaternion.identity) as ParticleSystem).Play();
            this.gameObject.GetComponent<playerController>().enabled = false;
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            this.gameObject.SetActive(false);
        }
        #endregion

        //Boost bar functionality
        #region Boost bar
        int points = GameManager.Instance.getPoints();
        //Augmento de da�o, Dash recovery, Velocidad
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
        paused = GameManager.Instance.isPaused();
    }

    public void takeDamage()
    {
        if (!shielded)
        {
            playerSounds[4].Play();
            playerLives--;
            //Activate Thorns here
            if (lostSouls.TryGetValue("Thorns", out LostSouls thorns))
            {
                if (thorns.isEquiped)
                {
                    thorns.isActive = true;
                }
            }
        }
        else
        {
            shield.color = new Color(0, 0, 0, 0);
            shielded = false;
            shieldTimer.enabled = true;
            playerSounds[4].Play();

        }

        immune = true;
        Invoke("damageImmunity", 1f);
        r2d.velocity = (new Vector2((sprite.flipX ? 1 : -1) * 2 * playerVelocity, jumpVelocity));
        updateLiveUI();
    }

    private void die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void addLive()
    {
        if (playerLives + 1 <= maxLives)
        {
            playerLives++;
            updateLiveUI();
        }
    }

    void stopAttack() 
    {
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        #region LostSoul Trigger
        
        if (collision.transform.tag == "LostSoul")
        {
            lostSouls.Add(collision.transform.GetComponent<LostSouls>().lostSoulName, collision.transform.GetComponent<LostSouls>());
            Destroy(collision.gameObject);
        }

        #endregion

        if (collision.transform.tag == "Bullet")
        {
            if (!isDashing)
            {
                takeDamage();
                collision.gameObject.SetActive(false);
            }
            else
            {
                Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        #region Dash immunity


        //Optimitzar no haver de comprovar cada cop si son enemic o enemic volador
        if ((collision.transform.tag == "Enemy") && isDashing)
        {
            enemy[enemyCounter] = collision;
            enemyCounter++;

            collision.transform.GetComponentInChildren<Rigidbody2D>().isKinematic = true;

            if (null != collision.transform.GetComponentInChildren<BoxCollider2D>())
            {
                collision.transform.GetComponentInChildren<BoxCollider2D>().isTrigger = true;
            }
            if (null != collision.transform.GetComponentInChildren<CapsuleCollider2D>())
            {
                collision.transform.GetComponentInChildren<CapsuleCollider2D>().isTrigger = true;
            }
            if (null != collision.transform.GetComponentInChildren<CircleCollider2D>())
            {
                collision.transform.GetComponentInChildren<CircleCollider2D>().isTrigger = true;
            }
            

            Invoke("returnEnemyToCollision", 0.25f);
        }

        #endregion

        #region Damage to player

        if (collision.transform.tag == "Trap")
        {
            playerLives = 0;
        }

        if ((collision.transform.tag == "Enemy" || collision.transform.tag == "Bullet") && !immune)
        {

            //Reaction to damage
            takeDamage();

            if (collision.transform.tag == "Enemy")
            {
                float[] knockbackInfo = new float[3];
                knockbackInfo[0] = 1.0f;
                knockbackInfo[1] = transform.position.x;
                knockbackInfo[2] = transform.position.y;
                collision.transform.GetComponentInParent<FatherEnemy>().applyKnockback(knockbackInfo);
            }
            if (collision.transform.tag == "Bullet")
            {
                Destroy(collision.gameObject);
            }
        }
        #endregion
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((collision.transform.tag == "Enemy" || collision.transform.tag == "Bullet") && !immune)
        {
            takeDamage();

            if (collision.transform.tag == "Enemy")
            {
                float[] knockbackInfo = new float[3];
                knockbackInfo[0] = 1.0f;
                knockbackInfo[1] = transform.position.x;
                knockbackInfo[2] = transform.position.y;
                collision.transform.GetComponentInParent<FatherEnemy>().applyKnockback(knockbackInfo);
            }
            if (collision.transform.tag == "Bullet")
            {
                Destroy(collision.gameObject);
            }
        }
    }

    //Update the ui to display the correct number of lives
    private void updateLiveUI()
    {
        Color color = new Color();
        color.r = 255;
        color.g = 255;
        color.b = 255;
        color.a = 1f;
        if(playerLives == 4)
        {
            live1.color = color;
            live2.color = color;
            live3.color = color;
            live4.color = color;
        }
        else if (playerLives == 3)
        {
            color.a = 1f;
            live1.color = color;
            live2.color = color;
            live3.color = color;
            color.a = 0.2f;
            live4.color = color;
        }
        else if (playerLives == 2)
        {
            color.a = 1f;
            live1.color = color;
            live2.color = color;
            color.a = 0.2f;
            live3.color = color;
            live4.color = color;
        }
        else if(playerLives == 1)
        {
            color.a = 1f;
            live1.color = color;
            color.a = 0.2f;
            live2.color = color;
            live3.color = color;
            live4.color = color;
        }
        else
        {
            color.a = 1f;
            color.a = 0.2f;
            live1.color = color;
            live2.color = color;
            live3.color = color;
            live4.color = color;
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
                    if (null != enemy[i].transform.GetComponentInChildren<BoxCollider2D>())
                    {
                        enemy[i].transform.GetComponentInChildren<BoxCollider2D>().isTrigger = false;
                    }
                    if (null != enemy[i].transform.GetComponentInChildren<CapsuleCollider2D>())
                    {
                        enemy[i].transform.GetComponentInChildren<CapsuleCollider2D>().isTrigger = false;
                    }
                    if (null != enemy[i].transform.GetComponentInChildren<CircleCollider2D>())
                    {
                        enemy[i].transform.GetComponentInChildren<CircleCollider2D>().isTrigger = false;
                    }
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
        if (lostSouls.ContainsKey("Light"))
        {
            lostSouls.TryGetValue("Light", out LostSouls ls);
            if (ls.isEquiped && ls.isActive)
            {
                light.SetActive(true);
            }
            else
            {
                light.SetActive(false);
            }
        }

        if (lostSouls.ContainsKey("Fireflies"))
        {
            lostSouls.TryGetValue("Fireflies", out LostSouls ls);
            if (ls.isEquiped && ls.isActive)
            {
                GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");
                foreach (GameObject trap in traps)
                {
                    trap.GetComponent<Traps>().setFireflies(true);
                }
            }
            else
            {
                GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");
                foreach (GameObject trap in traps)
                {
                    trap.GetComponent<Traps>().setFireflies(false);
                }
            }
        }

        if (lostSouls.ContainsKey("Thorns"))
        {
            lostSouls.TryGetValue("Thorns", out LostSouls ls);
            if (ls.isEquiped && ls.isActive)
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(ThornsPoint.position, ThornsRange, enemyLayer);
                foreach (Collider2D enemy in hitEnemies)
                {
                    //Fer mal als enemics un cop hi hagi herencies
                    float[] damageMessage = new float[3];
                    damageMessage[0] = attackDamage;
                    damageMessage[1] = transform.position.x;
                    damageMessage[2] = transform.position.y;
                    enemy.GetComponentInParent<FatherEnemy>().Damage(damageMessage);
                }
            }
        }
    }

    private void updateInventory()
    {
        if (lostSoulToggles.Count >= 0) {
            lostSoulToggles[0].GetComponent<Toggle>().Select();
        }
        for (int i = 0; i < lostSoulToggles.Count; i++)
        {
            if (lostSouls.TryGetValue(lostSoulToggles[i].name, out LostSouls ls))
            {
                Color c = lostSoulToggles[i].GetComponent<Image>().color;
                c.a = 1f;
                lostSoulToggles[i].GetComponent<Image>().color = c;
                lostSoulToggles[i].GetComponent<Toggle>().interactable = true;
            }
            else
            {
                Color c = lostSoulToggles[i].GetComponent<Image>().color;
                c.a = 0.1f;
                lostSoulToggles[i].GetComponent<Image>().color = c;
                lostSoulToggles[i].GetComponent<Toggle>().interactable = false;
            }
        }
    }

    private void toggleInventory()
    {
        if (inventory)
        {
            Time.timeScale = 0f;
            GameManager.Instance.changeInventory(true);
            inventoryUI.SetActive(true);
        }
        else{
            GameManager.Instance.changeInventory(false);
            Time.timeScale = 1f;
            inventoryUI.SetActive(false);
        }
    }

    public void toggleLostSoul(string value)
    {
        if(lostSouls.TryGetValue(value, out LostSouls ls))
        {
            if (lostSoulsEquipped <= maxLostSoulsEquipped)
            {
                ls.isEquiped = !ls.isEquiped;
                if (ls.isEquiped)
                {
                    lostSoulsEquipped++;
                }
                else
                {
                    lostSoulsEquipped--;
                }
            }
            else
            {
                //No hi ha mes espai per a equipar lost souls maybe informar jugador
            }
        }
    }
}
