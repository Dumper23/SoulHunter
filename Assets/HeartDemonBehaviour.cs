using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartDemonBehaviour : FatherEnemy
{
    [SerializeField]
    private BossDemon bossDemon;

    [SerializeField]
    private float maxHealth = 20,
        enabledDuration = 5,
        quantityDamageBoss = 10,
        quantityHealBoss = 5;

    private bool active = false,
        canDamage = false,
        canHeal = false,
        triggered = false;

    [SerializeField]
    private Collider2D col;

    private SpriteRenderer sR;

    [SerializeField]
    private Sprite spriteR,
        spriteB,
        spriteP;

    private float
        currentHealth,
        enabledStartTime;

    [SerializeField]
    private GameObject bossSprite;

    private void Start()
    {
        canDamage = false;
        canHeal = false;
        currentHealth = maxHealth;

    }

    private void OnEnable()
    {
        if (sR == null)
        {
            sR = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        }

        canDamage = false;
        canHeal = false;
        currentHealth = maxHealth;
        noGoToPlayer();
        enabledStartTime = Time.time;
        triggered = false;
        sR.sprite = spriteP;
        gameObject.transform.GetChild(0).transform.rotation = Quaternion.Euler(0,0,0);
    }

    private void Update()
    {
        if (canHeal || canDamage) {
            float angle = Mathf.Atan2((bossSprite.transform.position - transform.position).normalized.y, (bossSprite.transform.position - transform.position).normalized.x) * Mathf.Rad2Deg;
            gameObject.transform.GetChild(0).transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
        if (active && !triggered && !canDamage)
        {

            if (Time.time >= enabledStartTime + enabledDuration)
            {
                sR.sprite = spriteR;
                triggered = true;
                goToPlayer();
                canHeal = true;
            }
        }
    }

    private void goToPlayer()
    {
        this.transform.tag = "SoulV2";
        col.isTrigger = true;

    }

    private void noGoToPlayer()
    {
        this.transform.tag = "Untagged";
        col.isTrigger = false;

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {

        /*if (collision.transform.tag == "ground" || collision.transform.tag == "difWall" || collision.transform.tag == "trap")
        {
            
        }*/
        if (collision.transform.tag == "Player")
        {
            Physics2D.IgnoreCollision(col, collision.transform.GetComponent<Collider2D>());
        }
        if (collision.transform.tag == "Enemy" && (canDamage || canHeal))
        {
            if (!collision.transform.parent.GetComponent<HeartDemonBehaviour>()) {
                if (canDamage)
                {
                    float[] attackDetails = new float[3];
                    attackDetails[0] = quantityDamageBoss;
                    attackDetails[1] = -2;
                    attackDetails[2] = -2;
                    bossDemon.Damage(attackDetails, false);
                }
                if (canHeal)
                {
                    float[] attackDetails = new float[3];
                    attackDetails[0] = quantityHealBoss;
                    attackDetails[1] = -1;
                    attackDetails[2] = -1;
                    bossDemon.Damage(attackDetails, false);
                }
                active = false;
                canDamage = false;
                canHeal = false;
            }
            else
            {
                Physics2D.IgnoreCollision(col, collision.transform.GetComponent<Collider2D>());
            }
        }
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
    }

    public bool Activation()
    {
        return active;
    }

    public void SetActivation(bool act)
    {
        active = act;
    }

    public override void applyKnockback(float[] position)
    {
        //nothing
    }

    public override void Damage(float[] attackDetails, bool wantKnockback)
    {

        if (active)
        {
            currentHealth -= 10;


            //Hit particle

            if (currentHealth <= 0.0f)
            {
                goToPlayer();
                canDamage = true;
                sR.sprite = spriteB;
            }
        }
    }

    public override void mostraMissatge()
    {
        Debug.Log("<3");
    }

}
