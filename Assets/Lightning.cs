using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
            {
                if (gameObject.transform.name != "LightningAttack" && gameObject.transform.name != "ColliderArea") {
                    float[] msg = new float[3];
                    msg[0] = 0;
                    msg[1] = 0;
                    msg[2] = 0;
                    collision.GetComponentInParent<FatherEnemy>().Damage(msg, false);
                }
            }
            else
            {
                float[] msg = new float[3];
                msg[0] = 100;
                msg[1] = 0;
                msg[2] = 0;
                collision.GetComponentInParent<FatherEnemy>().Damage(msg, false);
            }
        }
    }
}
