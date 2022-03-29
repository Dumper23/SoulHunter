using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]


public class playerController : MonoBehaviour
{
    [Serializable]
    public struct LostSoulsIcons
    {
        public string name;
        public Sprite sprite;
    }

    [Serializable]
    public struct LostSoulsPlaceHolderIcons
    {
        public string name;
        public Image image;
    }

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
    private bool facingRight = false;
    

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
    private bool chargedJump = false;

    [Header("Combat settings")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;
    public LayerMask bulletLayer;
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

    private float originalPlayerSpeed;
    private int originalPlayerAttackDamage;
    private float startKillTime;
    private bool shielded = false;
    private float nextShield = 0f;

    [Header("UI settings")]
    public RawImage dashIndicator;
    public RawImage jumpIndicator1;
    public RawImage jumpIndicator2;
    public RawImage live1;
    public RawImage live2;
    public RawImage live3;
    public RawImage live4;
    public RawImage shield;
    public Text shieldTimer;
    public TextMeshProUGUI lostSoulName;
    public TextMeshProUGUI lostSoulDescription;
    public List<GameObject> lostSoulToggles = new List<GameObject>();
    public LostSoulsPlaceHolderIcons[] lostSoulsEquippedIcons;
    public GameObject inventoryUI;

    [Header("Sound Settings")]
    public List<AudioSource> playerSounds = new List<AudioSource>();
    public GameObject deadSoundObject;
    public AudioSource generalAudios;
    public AudioClip deadSound;
    public AudioClip bladeSound;

    [Header("LostSouls Settings")]
    public Dictionary<string, LostSouls> lostSouls = new Dictionary<string, LostSouls>();
    public Sprite nothingEquipped;
    public LostSoulsIcons[] lostSoulsIcons;
    public Transform ThornsPoint;
    public float ThornsRange = 0.5f;
    public GameObject light;
    private bool inventory = false;
    public int maxLostSoulsEquipped = 3;
    public float hardSkinRecoveryTime = 6f;
    public float stoneBreakerDamageMultiplier = 1.5f;
    public float holyWaterDamageMultiplier = 1.5f;
    public float outBurstRange = 1f;
    public float outBurstDamage = 1f;

    private int lostSoulsEquipped = 0;
    private bool stoneBreaker = false;
    private bool holyWater = false;
    private bool soulKeeperActive = false;
    private bool deflectMissiles = false;
    private Dictionary<string, string> lostSoulDescriptionDictionary = new Dictionary<string, string>();

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


    public float wallJumpTime = 0.2f;
    private float wallJumpCounter;
    void Start()
    {
        descriptions();
        if (loadPlayerData)
        {
            loadPlayer();
            updateInventory();
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

    private void descriptions()
    {
        lostSoulDescriptionDictionary.Add("Light", 
            "Provides you with some light, it is said that it guides you to the deepest and darkest thoughts...");

        lostSoulDescriptionDictionary.Add("Thorns",
            "Returns the damage you recieve to all the enemies that are close to you, confidence sucks!");

        lostSoulDescriptionDictionary.Add("Fireflies", 
            "Makes more visible all the traps on the level. \n\nThese little flying creatures are creepier than what you think.");
        //Si et pares a pensar pot ser que estiguin a les punxes perque hi ha sang i s'alimenten d'aquesta xd

        lostSoulDescriptionDictionary.Add("StoneBreaker",
            "You will deal more damage to enemies with shield.\n\nThe name is not self explanatory but thats why this section exists. :)");

        lostSoulDescriptionDictionary.Add("OutBurst",
            "You will damage the enemies if you hit them while dashing.\n\nShadow could have eaten something expired, causing him to leave a terible smell while dashing.");

        lostSoulDescriptionDictionary.Add("HardSkin", 
            "It Grants you with a shield that will protect you from 1 hit, it will be recharged in " + hardSkinRecoveryTime + "s.\n\nShadow told us that the shield is a table from Ikea, we still don't know if that's true or not.");

        lostSoulDescriptionDictionary.Add("SoulKeeper", 
            "When you kill one healer, instead of 1hp you will recieve 2hp. \n\n");

        lostSoulDescriptionDictionary.Add("DeflectMissiles",
            "With this Lost Soul you will be able to desviate projectiles. (It won't hurt enemies)\n\nThis Lost Soul appears to come from a different planet, and in the back it says 'May the force be with you', it's procedence it's a big mistery.");

        lostSoulDescriptionDictionary.Add("HolyWater",
            "You will deal more damage to Demonic enemies.\n\nShadow almost used this water as a perfume, luckily it doesn't smell too good.");

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
                    for (int j = 0; j < temp.equippedLostSouls.Length; j++)
                    {
                        if (!ls.isEquiped)
                        {
                            if (temp.equippedLostSouls[j] == temp.lostSouls[i])
                            {
                                ls.isEquiped = true;
                                break;
                            }
                            else
                            {
                                ls.isEquiped = false;
                            }
                        }
                    }
                    
                    lostSouls.Add(temp.lostSouls[i], ls);
                }
            }
        }
    }

    //-----------TODO: FixedUpdate (All the physics calculation and Update All the imput recievement)

    [System.Obsolete]
    void Update()
    {
        float verticalIn = 0;
        //Camera Movement
        if (Input.GetAxisRaw("RightJoystick") > 0)
        {
             verticalIn = -3f;
        }
        else if (Input.GetAxisRaw("RightJoystick") < 0)
        {
            verticalIn = 3f;
        }
        
        if (verticalIn != 0)
        {
            Vector3 pos = Vector3.Lerp(Camera.main.transform.position, transform.position + new Vector3(0, verticalIn, 0), 7 * Time.deltaTime);
            Camera.main.GetComponent<cameraMovement>().follow = false;
            Camera.main.transform.position =  new Vector3(pos.x, pos.y, Camera.main.transform.position.z);
        }
        else
        {
            Camera.main.GetComponent<cameraMovement>().follow = true;
        }

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
            float horizontalIn = Input.GetAxis("Horizontal");
            bool wantsToJump = Input.GetButtonDown("Jump");
            bool isGrounded = Physics2D.OverlapCircle(groundCollider.position, 0.15f, LayerMask.GetMask("Ground"));

            //Wall Sliding functionality
            #region Wall Sliding

            isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, LayerMask.GetMask("Ground"));
            /*
            if (isTouchingFront && !isGrounded && horizontalIn != 0)
            {
                wallSliding = true;
                if (!chargedJump)
                {
                    if(availableJumps < 2)
                    {
                        availableJumps = 1;
                        chargedJump = true;
                    }
                }
                changeAnimationState(PLAYER_WALLSLIDE);
            }
            else
            {
                wallSliding = false;
                chargedJump = false;
            }

            if (wallSliding)
            {
                r2d.velocity = new Vector2(r2d.velocity.x, Mathf.Clamp(r2d.velocity.y, -wallSlidingSpeed, Mathf.Infinity));
            }*/

            wallSliding = false;
            if (isTouchingFront && !isGrounded)
            {
                if ((facingRight && horizontalIn > 0) || (!facingRight && horizontalIn < 0))
                {
                    wallSliding = true;
                }
            }

            if (wallSliding)
            {
                changeAnimationState(PLAYER_WALLSLIDE);
                r2d.velocity = new Vector2(r2d.velocity.x, Mathf.Clamp(r2d.velocity.y, -wallSlidingSpeed, Mathf.Infinity));
                if (Input.GetButtonDown("Jump"))
                {
                    wallJumpCounter = wallJumpTime;

                    r2d.velocity = new Vector2(-horizontalIn * originalPlayerSpeed, jumpVelocity);
                    
                }
            }

            #endregion

            if (wallJumpCounter <= 0)
            {
                #region Movement
                
                if (horizontalIn > 0)
                {
                    facingRight = true;
                }
                else
                {
                    facingRight = false;
                }

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



                if (availableJumps == 0)
                {
                    jumpIndicator1.enabled = false;
                    jumpIndicator2.enabled = false;
                }
                if (availableJumps == 1)
                {
                    jumpIndicator1.enabled = true;
                    jumpIndicator2.enabled = false;
                }
                if (availableJumps == 2)
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

                if (wantsToJump && availableJumps > 0 && !wallSliding)
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
                    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
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

                        if (deflectMissiles)
                        {
                            deflectMissilesFunction();
                        }

                        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPoint.position, new Vector2(attackRange, attackRange), enemyLayer);

                        foreach (Collider2D enemy in hitEnemies)
                        {
                            if (enemy.tag == "Enemy")
                            {
                                float[] damageMessage = new float[3];

                                if (enemy.GetComponentInParent<FatherEnemy>().hasShield && enemy.GetComponentInParent<FatherEnemy>().isDemon && stoneBreaker && holyWater)
                                {
                                    damageMessage[0] = attackDamage * stoneBreakerDamageMultiplier * holyWaterDamageMultiplier;
                                }
                                else if (enemy.GetComponentInParent<FatherEnemy>().hasShield && stoneBreaker)
                                {
                                    damageMessage[0] = attackDamage * stoneBreakerDamageMultiplier;
                                }
                                else if (enemy.GetComponentInParent<FatherEnemy>().isDemon && holyWater)
                                {
                                    damageMessage[0] = attackDamage * holyWaterDamageMultiplier;
                                }
                                else
                                {
                                    damageMessage[0] = attackDamage;
                                }

                                damageMessage[1] = transform.position.x;
                                damageMessage[2] = transform.position.y;
                                if (enemy.GetComponentInParent<FatherEnemy>() != null)
                                {
                                    enemy.GetComponentInParent<FatherEnemy>().Damage(damageMessage, true);
                                }
                            }
                            if (enemy.tag == "Healer")
                            {
                                if (enemy.GetComponent<healer>() != null)
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
            else
            {
                wallJumpCounter -= Time.deltaTime;
            }
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
        /*
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

       /* if (GameManager.Instance.getPoints() >= GameManager.Instance.getMaxPoints() * 0.75)
        {
            if (!shielded)
            {
                if (Time.time - nextShield > hardSkinRecoveryTime)
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
        }*/

        #endregion

        paused = GameManager.Instance.isPaused();
    }

    private void deflectMissilesFunction()
    {
        Collider2D[] hitBullets = Physics2D.OverlapBoxAll(attackPoint.position, new Vector2(attackRange, attackRange), bulletLayer);

        foreach(Collider2D bullet in hitBullets)
        {
            if (bullet.GetComponent<bullet>() != null)
            {
                bullet.GetComponent<bullet>().changeDirection();
            }
        }
    }

    public void takeDamage()
    {
        if (!immune)
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
    }

    private void die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void addLive()
    {
        if (playerLives + 1 <= maxLives)
        {
            if (soulKeeperActive)
            {
                playerLives = playerLives + 2;
            }else
            {
                playerLives++;
            }
            
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
            if (!lostSouls.TryGetValue(collision.GetComponent<LostSouls>().lostSoulName, out LostSouls ls))
            {
                lostSouls.Add(collision.transform.GetComponent<LostSouls>().lostSoulName, collision.transform.GetComponent<LostSouls>());
            }
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

        if(collision.transform.tag == "Lancer")
        {
            if (!isDashing)
            {
                takeDamage();
            }
            else
            {
                Physics2D.IgnoreCollision(collision.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ThornsPoint.position, outBurstRange);
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
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
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
                    float[] damageMessage = new float[3];
                    damageMessage[0] = attackDamage;
                    damageMessage[1] = transform.position.x;
                    damageMessage[2] = transform.position.y;
                    enemy.GetComponentInParent<FatherEnemy>().Damage(damageMessage, true);
                }
            }
        }

        if (lostSouls.ContainsKey("HardSkin"))
        {
            lostSouls.TryGetValue("HardSkin", out LostSouls ls);
            if (ls.isEquiped && ls.isActive)
            {
                if (!shielded)
                {
                    if (Time.time - nextShield > hardSkinRecoveryTime)
                    {
                        nextShield = Time.time;
                        shield.gameObject.SetActive(true);
                        shield.color = new Color(255, 255, 255, 255);
                        shielded = true;
                    }
                    else
                    {
                        shieldTimer.gameObject.SetActive(true);
                    }
                }
                else
                {
                    shieldTimer.gameObject.SetActive(false);
                    nextShield = Time.time;
                }
                shieldTimer.text = hardSkinRecoveryTime - (Time.time - nextShield) + "s";
            }
            else
            {
                shieldTimer.gameObject.SetActive(false);
                shield.gameObject.SetActive(false);
                shielded = false;
            }
        }

        if (lostSouls.ContainsKey("StoneBreaker"))
        {
            lostSouls.TryGetValue("StoneBreaker", out LostSouls ls);
            if (ls.isEquiped && ls.isActive)
            {
                stoneBreaker = true;
            }
            else
            {
                stoneBreaker = false;
            }
        }

        if (lostSouls.ContainsKey("HolyWater"))
        {
            lostSouls.TryGetValue("HolyWater", out LostSouls ls);
            if (ls.isEquiped && ls.isActive)
            {
                holyWater = true;
            }
            else
            {
                holyWater = false;
            }
        }

        if (lostSouls.ContainsKey("OutBurst"))
        {
            lostSouls.TryGetValue("OutBurst", out LostSouls ls);
            if (ls.isEquiped && ls.isActive)
            {
                if (isDashing) {
                    Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, outBurstRange);

                    foreach(Collider2D enemy in enemies)
                    {
                        if(enemy.tag == "Enemy" && !enemy.GetComponentInParent<FatherEnemy>().outBursted)
                        {
                            StartCoroutine("endOutburstEnemy", enemy);
                            enemy.GetComponentInParent<FatherEnemy>().outBursted = true;
                            float[] damageMessage = new float[3];
                            damageMessage[0] = outBurstDamage;
                            damageMessage[1] = transform.position.x;
                            damageMessage[2] = transform.position.y;
                            if (enemy.GetComponentInParent<FatherEnemy>() != null)
                            {
                                enemy.GetComponentInParent<FatherEnemy>().Damage(damageMessage, true);
                            }
                        }
                    }
                }
            }
        }

        if (lostSouls.ContainsKey("SoulKeeper"))
        {
            lostSouls.TryGetValue("SoulKeeper", out LostSouls ls);
            if (ls.isEquiped && ls.isActive)
            {
                soulKeeperActive = true;
            }
            else
            {
                soulKeeperActive = false;
            }
        }

        if (lostSouls.ContainsKey("DeflectMissiles"))
        {
            lostSouls.TryGetValue("DeflectMissiles", out LostSouls ls);
            if (ls.isEquiped && ls.isActive)
            {
                deflectMissiles = true;
            }
            else
            {
                deflectMissiles = true;
            }
        }
    }

    IEnumerator endOutburstEnemy(Collider2D enemy)
    {
        yield return new WaitForSeconds(0.3f);
        if (enemy != null)
        {
            enemy.GetComponentInParent<FatherEnemy>().outBursted = false;
        }
        StopAllCoroutines();
    }

    private void updateInventory()
    {
        for (int i = 0; i < lostSoulToggles.Count; i++)
        {
            if (lostSouls.TryGetValue(lostSoulToggles[i].name, out LostSouls ls))
            {
                if (ls.isEquiped)
                {
                    lostSoulToggles[i].GetComponent<Toggle>().SetIsOnWithoutNotify(true);
                }

                Color c = lostSoulToggles[i].GetComponent<Image>().color;
                c.a = 1f;
                lostSoulToggles[i].GetComponent<Image>().color = c;
                lostSoulToggles[i].GetComponent<Toggle>().interactable = true;
                if (lostSoulsEquipped >= maxLostSoulsEquipped)
                {
                    if (!ls.isEquiped)
                    {
                        lostSoulToggles[i].GetComponent<Toggle>().interactable = false;
                    }
                }
            }
            else
            {
                Color c = lostSoulToggles[i].GetComponent<Image>().color;
                c.a = 0.1f;
                lostSoulToggles[i].GetComponent<Image>().color = c;
                lostSoulToggles[i].GetComponent<Toggle>().interactable = false;
            }
        }

        for (int i = 0; i < lostSoulToggles.Count; i++)
        {
            if(lostSouls.TryGetValue(lostSoulToggles[i].name, out LostSouls ls))
            {
                if (ls.isEquiped)
                {
                    for (int j = 0; j < lostSoulsIcons.Length; j++)
                    {
                        if(lostSoulsIcons[j].name == lostSoulToggles[i].name)
                        {
                            bool alreadySet = false;
                            for (int k = 0; k < lostSoulsEquippedIcons.Length; k++)
                            { 
                                if(lostSoulsEquippedIcons[k].name == lostSoulToggles[i].name)
                                {
                                    alreadySet = true;
                                }
                            }

                            if (!alreadySet)
                            {
                                alreadySet = false;
                                for (int k = 0; k < lostSoulsEquippedIcons.Length; k++)
                                {
                                    if (lostSoulsEquippedIcons[k].name == "")
                                    {
                                        lostSoulsEquippedIcons[k].image.sprite = lostSoulsIcons[j].sprite;
                                        lostSoulsEquippedIcons[k].name = lostSoulsIcons[j].name;
                                        alreadySet = true;
                                    }
                                    if(alreadySet) break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < lostSoulsEquippedIcons.Length; k++)
                    {
                        if (lostSoulsEquippedIcons[k].name == lostSoulToggles[i].name)
                        {
                            lostSoulsEquippedIcons[k].image.sprite = nothingEquipped;
                            lostSoulsEquippedIcons[k].name = "";
                        }
                    }
                }
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
            if (lostSoulToggles.Count >= 0)
            {
                lostSoulToggles[0].GetComponent<Toggle>().Select();   
            }
        }
        else{
            GameManager.Instance.changeInventory(false);
            Time.timeScale = 1f;
            inventoryUI.SetActive(false);
        }
    }

    public void toggleLostSoul(string value)
    {
        if (lostSouls.TryGetValue(value, out LostSouls ls))
        {
            if (lostSoulsEquipped <= maxLostSoulsEquipped)
            {
                ls.isEquiped = !ls.isEquiped;
                if (ls.isEquiped)
                {
                    if (lostSoulsEquipped + 1 <= maxLostSoulsEquipped)
                    {
                        lostSoulsEquipped++;
                    }
                }
                else
                {
                    if (lostSoulsEquipped - 1 >= 0)
                    {
                        lostSoulsEquipped--;
                    }
                }
            }
            else
            {
                //No hi ha mes espai per a equipar lost souls maybe informar jugador
                Debug.Log("Full!");
            }
        }
        updateInventory();
    }

    public void showLostSoulInfo(string name)
    {
        
        
        if (lostSoulDescriptionDictionary.TryGetValue(name, out string description) && lostSouls.TryGetValue(name, out LostSouls ls)) {
            lostSoulName.SetText(name);
            lostSoulDescription.SetText(description);
        }
        else
        {
            lostSoulName.SetText("???");
            lostSoulDescription.SetText("No information available...");
        }
    }
}
