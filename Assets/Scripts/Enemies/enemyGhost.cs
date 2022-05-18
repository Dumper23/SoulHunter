using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyGhost : FatherEnemy
{
    public GameObject waypointsParent;
    public float moveSpeed = 1.5f;

    private int currentWaypoint = 0;
    private List<GameObject> waypoints = new List<GameObject>();
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentWaypoint = 0;
        for (int i = 0; i < waypointsParent.transform.childCount; i++)
        {
            waypoints.Add(waypointsParent.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPos = waypoints[currentWaypoint].transform.position - transform.position;

        if(waypoints[currentWaypoint].transform.position.x >= transform.position.x)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }

        transform.Translate(newPos.normalized * Time.deltaTime * moveSpeed);
        if (newPos.magnitude <= 1)
        {
            
            if (currentWaypoint + 1 < waypointsParent.transform.childCount)
            {
                currentWaypoint++;
            }
            else
            {
                currentWaypoint = 0;
            }
        }
    }

    #region Interficie
    public override void applyKnockback(float[] position)
    {
    }

    public override void Damage(float[] damageMessage, bool wantKnockback)
    {
    }

    public override void mostraMissatge()
    {
    }
    #endregion


    private void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;
        int i = 0;
        Transform lastWaypoint = null;
        foreach(Transform waypoint in waypointsParent.transform)
        {
            Gizmos.DrawSphere(waypoint.transform.position, 0.25f);
            if (i == 0)
            {
                    Gizmos.DrawLine(transform.position, waypoint.position);
            }
            else if(i + 1 <= waypointsParent.transform.childCount)
            {
                Gizmos.DrawLine(lastWaypoint.position, waypoint.position);
            }
            lastWaypoint = waypoint;
            i++;
        }
    }

}
