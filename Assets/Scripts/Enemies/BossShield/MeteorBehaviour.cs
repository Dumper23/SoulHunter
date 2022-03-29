using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorBehaviour : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem meteorsChunks;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,5);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag != "Meteor")
        {
            ContactPoint2D c = collision.contacts[0];
            Instantiate(meteorsChunks, c.point, Quaternion.identity);
            Destroy();
        }
    }
    public void Destroy()
    {
        gameObject.SetActive(false);
    }
}
