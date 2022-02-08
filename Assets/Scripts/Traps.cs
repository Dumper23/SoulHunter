using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
    public GameObject light;
    public ParticleSystem fireflies;

    public void setFireflies(bool b)
    {
        light.SetActive(b);
        fireflies.gameObject.SetActive(b);
    }

}
