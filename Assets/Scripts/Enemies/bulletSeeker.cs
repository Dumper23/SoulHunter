using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletSeeker : FatherBullet
{
    private Vector2 moveDirection;
    [SerializeField]
    private float moveSpeed = 3f;
    private float rotateSpeed = 100f;
    public bool isSeeking = true;

    private Transform player;

    private Rigidbody2D rb; 

    private void OnEnable()
    {
        Invoke("Destroy", 10f);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        //transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        //transform.up = (player.transform.position - transform.position);

        if (isSeeking)
        {
            Vector2 direction = (Vector2)player.position - rb.position;

            direction.Normalize();

            float rotateAmount = Vector3.Cross(direction, transform.up).z;

            rb.angularVelocity = -rotateAmount * rotateSpeed;

            rb.velocity = transform.up * moveSpeed;
        }
        else
        {
            rb.velocity = moveDirection;
        }
        
    }

    public void SetMoveDirection(Vector2 dir)
    {
        moveDirection = dir;
    }
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    public void SetRotationSpeed(float speed)
    {
        rotateSpeed = speed;
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "ground" || collision.transform.tag == "difWall")
        {
            gameObject.SetActive(false);
        }
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public override void ChangeDirection()
    {
        isSeeking = false;
        moveDirection = (-player.transform.position + transform.position).normalized * moveSpeed;
    }
}
