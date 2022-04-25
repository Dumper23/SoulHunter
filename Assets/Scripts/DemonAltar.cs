using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DemonAltar : MonoBehaviour
{
    public Animator demonLight;
    public ParticleSystem particle;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            GameManager.Instance.playerInDemonicAltar = true;
            demonLight.Play("turnOn");
            particle.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameManager.Instance.playerInDemonicAltar = false;
            demonLight.Play("turnOff");
            particle.Stop();
        }
    }
}
