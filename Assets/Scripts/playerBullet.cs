using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBullet : MonoBehaviour
{
    public Vector2 directionToMove;
    public float speed;
    public int damage;
    public GameObject particles;

    private GameObject currentParticles;

    private void OnEnable()
    {
        currentParticles = Instantiate(particles, transform.position, Quaternion.identity);
        currentParticles.GetComponent<ParticlesFollowing>().timeToDestroy = 5f;
        currentParticles.GetComponent<ParticlesFollowing>().target = transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(directionToMove * speed * Time.deltaTime);
    }

    private void OnDisable()
    {
        currentParticles.GetComponent<ParticleSystem>().Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ground" || collision.tag == "difWall")
        {
            this.gameObject.SetActive(false);
        }

            if (collision.tag == "Enemy")
        {
            float[] damageMessage = new float[3];
            damageMessage[0] = damage;
            damageMessage[1] = transform.position.x;
            damageMessage[2] = transform.position.y;
            if (collision.GetComponentInParent<FatherEnemy>() != null)
            {
                if (collision.GetComponentInParent<FatherEnemy>().gameObject.GetComponent<HeartDemonBehaviour>())
                {
                    damageMessage[0] = 10f;
                }
                collision.GetComponentInParent<FatherEnemy>().Damage(damageMessage, true);

            }
            this.gameObject.SetActive(false);
        }
    }

    public void destroyWithTime(float timeToDestroy)
    {
        Invoke("unableObject", timeToDestroy);
    }

    private void unableObject()
    {
        currentParticles.GetComponent<ParticleSystem>().Stop();
        this.gameObject.SetActive(false);
    }
}
