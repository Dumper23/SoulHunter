using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rain : MonoBehaviour
{
    void Start()
    {
        GetComponent<ParticleSystem>().Simulate(50);
        GetComponent<ParticleSystem>().Play();

    }

    
}
