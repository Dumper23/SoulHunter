using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerBehaviour : FatherEnemy
{
    private float percentageScale;

    // Start is called before the first frame update
    void Start()
    {
        percentageScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (percentageScale > 0)
        {
            percentageScale = percentageScale - (float)(5 * Time.deltaTime);
        }

    }

    public float GetPercentage()
    {
        return percentageScale;
    }

    public override void applyKnockback(float[] position)
    {
        //nothing
    }

    public override void Damage(float[] damageMessage, bool wantKnockback)
    {
        if (percentageScale < 100) {
            percentageScale = percentageScale + 10;
        }
        if (percentageScale > 100)
        {
            percentageScale = 100;
        }
    }

    public override void mostraMissatge()
    {
        Debug.Log("Charging");
    }
}
