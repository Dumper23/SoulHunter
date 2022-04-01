using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireVoiceBullets : MonoBehaviour
{
    [SerializeField]
    private int bulletsAmount = 10;

    [SerializeField]
    private float startAngle = 90f,
        endAngle = 270f;

    private Vector2 bulletMoveDirection;

    [SerializeField]
    private GameObject spawnPoint;

    private float fireRate = 2f;

    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("Fire", 0f, 2f);
    }

    private void Fire()
    {
        float angleStep = (endAngle - startAngle) / bulletsAmount;
        float angle = startAngle;

        for (int i = 0; i < bulletsAmount + 1; i++)
        {
            float bulDirX = spawnPoint.transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float bulDirY = spawnPoint.transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            Vector2 bulDir = (bulMoveVector - spawnPoint.transform.position).normalized;

            GameObject bul = bulletPool.bulletPoolInstance.GetBullet();
            bul.transform.position = spawnPoint.transform.position;
            bul.transform.rotation = spawnPoint.transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<voiceBullet>().SetMoveDirection(bulDir);

            angle += angleStep;
        }
    }

    public void Shoot()
    {
        InvokeRepeating("Fire", 0f, fireRate);
    }

    public void StopShoot()
    {
        CancelInvoke("Fire");
    }

    public void SetAmmount(int ammount)
    {
        bulletsAmount = ammount;
    }

    public void SetFireRate(float rate)
    {
        fireRate = rate;
    }
}
