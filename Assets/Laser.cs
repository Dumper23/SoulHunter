using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float defDistanceRay = 100;
    public Transform laserFirePoint;
    public LineRenderer LR;
    Transform m_transform;

    [SerializeField]
    private LayerMask whatIsGround,
        playerMask;

    [SerializeField]
    private ParticleSystem particlesHit;

    private void Awake()
    {
        m_transform = GetComponent<Transform>();
    }

    void ShootLaser()
    {
        if(Physics2D.Raycast(m_transform.position, transform.right,whatIsGround))
        {

            RaycastHit2D hit = Physics2D.Raycast(laserFirePoint.position, transform.right, defDistanceRay, playerMask);
            if (hit)
            {
                if (hit.transform.tag == "Player")
                {
                    hit.transform.GetComponent<playerController>().takeDamage();
                }
            }
            RaycastHit2D _hit = Physics2D.Raycast(laserFirePoint.position, transform.right, 100, whatIsGround);
            if (_hit)
            {
                if (_hit.transform.tag != "BossIgnore")
                {
                    if (Vector3.Distance(laserFirePoint.position, _hit.point) > defDistanceRay)
                    {
                        
                    
                    //Draw2DRay(laserFirePoint.position, gameObject.GetComponentInParent<Transform>().transform.right * defDistanceRay);

                        Draw2DRay(laserFirePoint.position, (_hit.point - new Vector2(laserFirePoint.position.x, laserFirePoint.position.y)).normalized * defDistanceRay + (Vector2)laserFirePoint.position);
                    }
                    else
                    {
                        Draw2DRay(laserFirePoint.position, _hit.point);
                    }
                }
                else
                {
                    //Draw2DRay(laserFirePoint.position, laserFirePoint.transform.right * defDistanceRay);
                }
            }
            else
            {

                Draw2DRay(laserFirePoint.position, (_hit.point - new Vector2(laserFirePoint.position.x, laserFirePoint.position.y)).normalized * defDistanceRay + (Vector2)laserFirePoint.position);
            }

        }
        else
        {
            Draw2DRay(laserFirePoint.position, laserFirePoint.transform.right * defDistanceRay);

        }
    }

    public void StopParticles()
    {
        particlesHit.Stop();
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        LR.SetPosition(0, startPos);
        LR.SetPosition(1, endPos);
        particlesHit.transform.position = endPos - (endPos-startPos).normalized * 0.5f;
        particlesHit.Play();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShootLaser();
    }
}
