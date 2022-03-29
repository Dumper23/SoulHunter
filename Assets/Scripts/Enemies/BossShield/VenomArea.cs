using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenomArea : MonoBehaviour
{
    private bool 
        venoming = false,
        venomed = false;

    [SerializeField]
    private ParticleSystem venomParticles;

    public void StartVenoming(Vector3 pos)
    {
        venoming = true;
        venomParticles.transform.position = pos;
        venomParticles.Play();
    }

    public void StopVenoming()
    {
        venomed = false;
        venomParticles.Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player" && venoming)
        {
            venomed = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "Player" && venoming)
        {
            venomed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Player" && venoming)
        {
            venomed = false;
        }
    }

    public bool IsVenomed()
    {
        return venomed;
    }
}
