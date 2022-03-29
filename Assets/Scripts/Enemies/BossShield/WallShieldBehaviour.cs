using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallShieldBehaviour : MonoBehaviour
{

    public void Deactivate()
    {
        //play particles??
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Limit")
        {
            Deactivate();
        }
    }
}
