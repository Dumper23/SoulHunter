using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected enum State
    {
        Walking,
        Knockback,
        Dead
    }

    protected int damageToPlayer;
    protected int pointsToGive;

    protected State currentState;

    [SerializeField]
    protected float
        groundCheckDistance,
        wallCheckDistance,
        movementSpeed,
        maxHealth,
        knockbackDuration,
        enemyDetectionRange;

    [SerializeField]
    protected Transform
        groundCheck,
        groundCheckBack,
        wallCheck,
        enemyCollision;

    [SerializeField]
    protected LayerMask
        whatIsGround,
        whatIsEnemy;

    [SerializeField]
    protected Vector2 knockbackSpeed;

    [SerializeField]
    protected GameObject
        hitParticle,
        deathChunkParticle,
        deathBloodParticle;

    protected int
        facingDirection,
        damageDirection;

    protected Vector2 movement;

    protected float
        currentHealth,
        knockbackStartTime;

    protected bool
        groundDetected,
        groundDetectedBack,
        wallDetected,
        enemyDetected;

    protected GameObject alive;

    protected Rigidbody2D aliveRb;

    protected Animator aliveAnim;

    [SerializeField]
    protected ParticleSystem particleDamage;

}
