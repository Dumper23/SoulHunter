using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FatherEnemy : MonoBehaviour
{

    public abstract void mostraMissatge();
    public abstract void applyKnockback(float[] position);
    public abstract void Damage(float[] damageMessage);

}
