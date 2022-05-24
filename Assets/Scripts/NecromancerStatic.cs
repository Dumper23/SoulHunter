using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerStatic : FatherEnemy
{
    public GameObject[] skeletonsToProtect;
    public List<LineRenderer> lines = new List<LineRenderer>();
    public Material lineMaterial;

    public GameObject deadSoundObject;
    public AudioClip[] audios;

    public GameObject soul;
    public float soulForce = 35;
    public int soulsToGive = 10;

    private const int DAMAGE_SOUND = 0;
    private const int DEAD_SOUND = 1;

    [SerializeField]
    private GameObject
        deathChunkParticle,
        deathBloodParticle;

    void Start()
    {
        foreach (GameObject skeleton in skeletonsToProtect)
        {
            skeleton.GetComponent<Piquero>().isProtected = true;
            GameObject go = Instantiate(new GameObject(), gameObject.transform);
            LineRenderer line = go.AddComponent<LineRenderer>();
            line.startColor = Color.red;
            line.material = lineMaterial;
            line.startWidth = 0.2f;
            line.endWidth = 0f;
            lines.Add(line);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        foreach(LineRenderer line in lines)
        {
            line.SetPosition(0, transform.position);
            if(skeletonsToProtect[i] != null){
                line.SetPosition(1, skeletonsToProtect[i].GetComponentInChildren<Rigidbody2D>().gameObject.transform.position);
            }
            i++;
        }
    }

    public override void applyKnockback(float[] position)
    {

    }

    public override void Damage(float[] damageMessage, bool wantKnockback)
    {
        deadSoundObject.GetComponent<AudioSource>().clip = audios[DEAD_SOUND];
        Instantiate(deadSoundObject, transform.position, transform.rotation);

        for (int i = 0; i <= soulsToGive; i++)
        {
            GameObject g = Instantiate(soul, transform.position, Quaternion.identity);
            g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * soulForce, ForceMode2D.Impulse);
        }

        Instantiate(deathChunkParticle, transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodParticle, transform.position, deathBloodParticle.transform.rotation);
        foreach(GameObject skeleton in skeletonsToProtect)
        {
            skeleton.GetComponent<Piquero>().isProtected = false;
        }
        Destroy(gameObject);
    }

    public override void mostraMissatge()
    {

    }
}
