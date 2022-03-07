using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FatherEnemy : MonoBehaviour
{
    public bool hasShield = false;
    public bool isDemon = false;
    public bool outBursted = false;

    public abstract void mostraMissatge();
    public abstract void applyKnockback(float[] position);
    public abstract void Damage(float[] damageMessage, bool wantKnockback);

}
