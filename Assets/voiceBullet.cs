using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class voiceBullet : MonoBehaviour
{
    private Vector2 moveDirection;
    [SerializeField]
    private float moveSpeed = 3f;

    private void OnEnable()
    {
        Invoke("Destroy", 2f);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    public void SetMoveDirection(Vector2 dir)
    {
        moveDirection = dir;
        //transform.rotation = Quaternion.Euler(new Vector3(0,0,Vector2.Angle(new Vector2(0,1),dir)));
        //transform.rotation = Quaternion.LookRotation(dir);

        //float rot_z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

        //Quaternion toRotate = Quaternion.LookRotation(Vector3.forward, dir);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, 0f*Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "ground")
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

}
