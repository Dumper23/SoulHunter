using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class playerController : MonoBehaviour
{
    [Header("Basic Movement settings")]
    public float playerVelocity = 30f;
    public float jumpVelocity = 20f;
    public float gravityScale = 10f;
    public int maxJumps = 2;
    public ParticleSystem jumpParticle;

    private int availableJumps;


    [Header("Dash settings")]
    public float dashForce = 40f;
    public float startDashTime = 0.25f;
    public float dashRecoveryTime = 1.5f;
    public ParticleSystem dashParticle;

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
    private float nextAttackTime = 0;

    //Other settings
    private Transform t;
    private Rigidbody2D r2d;
    private SpriteRenderer sprite;
    private Animator animator;
    private Transform groundCollider;

    


    void Start()
    {
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

        r2d.velocity = new Vector2(horizontalIn * playerVelocity, r2d.velocity.y);


        bool isGrounded = Physics2D.OverlapCircle(groundCollider.position, 0.15f, LayerMask.GetMask("Ground"));

        if (isGrounded && !Input.GetButton("Jump"))
        {
            availableJumps = maxJumps;
        }

        if (wantsToJump && availableJumps > 0)
        {
            float hVelocity = r2d.velocity.y;

            r2d.velocity = new Vector2(hVelocity, jumpVelocity);
            
            availableJumps--;
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
            currentDashRecoveryTime = dashRecoveryTime;
            isDashing = true;
            dashParticle.Play();
            currentDashTime = startDashTime;
            r2d.velocity = Vector3.zero;
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
            r2d.velocity = transform.right * dashDirection * dashForce;
            currentDashTime -= Time.deltaTime;
            
            tmp.a = 0.2f;
            sprite.color = tmp;
            dashed = true;
            if (currentDashTime <= 0)
            {
                isDashing = false;
                tmp.a = 1;
                sprite.color = tmp;
            }
        }

        if (dashed)
        {
            if(currentDashRecoveryTime <= 0)
            {
                dashed = false;
                
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
        }
        else
        {
            wallSliding = false;
        }

        if (wallSliding)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, Mathf.Clamp(r2d.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }

        #endregion

        #region Combat
        if(Time.time >= nextAttackTime )
        if (Input.GetKeyDown(KeyCode.K))
        {
            //Play attack animation
            
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

            foreach(Collider2D enemy in hitEnemies)
            {
                //Creidar la funcio de lenemic per restarli vida
                //enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
                nextAttackTime = Time.time + 1 / attackRate;
            }

        }
        #endregion

        animator.SetFloat("hVel", Mathf.Abs(r2d.velocity.x));


    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
