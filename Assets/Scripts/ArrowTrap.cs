using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    public GameObject arrow;
    public float fireRate = 1f;
    public float reloadTime = 1f;
    public float arrowSpeed = 2f;
    public float arrowRotationSpeed = 500f;
    public bool isAlwaysShooting = false;
    public Transform activationPoint;
    public float activationRange = 1f;
    public SpriteRenderer pressurePlate;

    private float time = 0f;
    private bool activated = false;
    private AudioSource a;
    
    void Start()
    {
        a = GetComponent<AudioSource>();
        if (isAlwaysShooting)
        {
            pressurePlate.enabled = false;
        }
    }

    void Update()
    {
        time += Time.deltaTime;
        if (isAlwaysShooting)
        {
            if(time > fireRate)
            {
                time = 0;
                shoot();
            }
        }
        else
        {
            Collider2D[] collision = Physics2D.OverlapCircleAll(activationPoint.position, activationRange);
            foreach (Collider2D c in collision)
            {
                if (c != null && c.tag == "Player" && !activated)
                {
                    shoot();
                    time = 0;
                    activated = true;
                }
            }
            if (activated)
            {
                pressurePlate.color = Color.red;
                if (time > reloadTime)
                {
                    time = 0;
                    activated = false;
                    pressurePlate.color = Color.white;
                }
            }
        }

       
    }

    private void shoot()
    {
        a.Play();
        GameObject go = Instantiate(arrow, transform.position, Quaternion.identity);
        go.GetComponent<Arrow>().direction = activationPoint.position - transform.position;
        go.GetComponent<Arrow>().speed = arrowSpeed;
        go.GetComponent<Arrow>().rotationSpeed = arrowRotationSpeed;
        
    }

    private void OnDrawGizmos()
    {
        if (activationPoint != null)
        {
            Gizmos.DrawWireSphere(activationPoint.position, activationRange);
            Gizmos.DrawLine(activationPoint.position, transform.position);
        }
    }
}
