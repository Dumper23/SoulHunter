using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDemonPool : MonoBehaviour
{
    public static BossDemonPool BossDemonPoolInstance;

    [SerializeField]
    private GameObject
        poolLancers,
        poolParticles,
        poolMeteors,
        poolSideLava,
        poolSideLavaAm,
        poolBullet;


    private bool notEnoughLancersInPool = true;
    private bool notEnoughParticlesInPool = true;
    private bool notEnoughMeteors = true;
    private bool notEnoughSideLava = true;
    private bool notEnoughSideLavaAm = true;
    private bool notEnoughBullets = true;


    private List<GameObject> lancers;
    private List<GameObject> lancersParticles;
    private List<GameObject> meteors;
    private List<GameObject> sideLava;
    private List<GameObject> sideLavaAm;
    private List<GameObject> bullets;


    private void Awake()
    {
        BossDemonPoolInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        lancers = new List<GameObject>();
        lancersParticles = new List<GameObject>();
        meteors = new List<GameObject>();
        sideLava = new List<GameObject>();
        sideLavaAm = new List<GameObject>();
        bullets = new List<GameObject>();
    }

    public GameObject GetBullet()
    {
        if (bullets.Count > 0)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (!bullets[i].activeInHierarchy)
                {
                    return bullets[i];
                }
            }
        }

        if (notEnoughBullets)
        {
            GameObject bul = Instantiate(poolBullet);
            bul.SetActive(false);
            bullets.Add(bul);
            return bul;
        }

        return null;
    }
    public void DisableBullets()
    {
        foreach (GameObject bullet in bullets)
        {
            bullet.SetActive(false);
        }
    }

    public GameObject GetLancer()
    {
        if (lancers.Count > 0)
        {
            for (int i = 0; i < lancers.Count; i++)
            {
                if (!lancers[i].activeInHierarchy)
                {
                    return lancers[i];
                }
            }
        }

        if (notEnoughLancersInPool)
        {
            GameObject bul = Instantiate(poolLancers);
            bul.SetActive(false);
            lancers.Add(bul);
            return bul;
        }

        return null;
    }

    public GameObject GetParticle()
    {
        if (lancersParticles.Count > 0)
        {
            for (int i = 0; i < lancersParticles.Count; i++)
            {
                if (!lancersParticles[i].activeInHierarchy)
                {
                    return lancersParticles[i];
                }
            }
        }

        if (notEnoughParticlesInPool)
        {
            GameObject bul = Instantiate(poolParticles);
            bul.SetActive(false);
            lancersParticles.Add(bul);
            return bul;
        }

        return null;
    }

    public GameObject GetSideLava()
    {
        if (sideLava.Count > 0)
        {
            for (int i = 0; i < sideLava.Count; i++)
            {
                if (!sideLava[i].activeInHierarchy)
                {
                    return sideLava[i];
                }
            }
        }

        if (notEnoughSideLava)
        {
            GameObject bul = Instantiate(poolSideLava);
            bul.SetActive(false);
            sideLava.Add(bul);
            return bul;
        }

        return null;
    }

    public GameObject GetSideLavaAm()
    {
        if (sideLavaAm.Count > 0)
        {
            for (int i = 0; i < sideLavaAm.Count; i++)
            {
                if (!sideLavaAm[i].activeInHierarchy)
                {
                    return sideLavaAm[i];
                }
            }
        }

        if (notEnoughSideLavaAm)
        {
            GameObject bul = Instantiate(poolSideLavaAm);
            bul.SetActive(false);
            sideLavaAm.Add(bul);
            return bul;
        }

        return null;
    }

    public GameObject GetMeteor()
    {
        if (meteors.Count > 0)
        {
            for (int i = 0; i < meteors.Count; i++)
            {
                if (!meteors[i].activeInHierarchy)
                {
                    return meteors[i];
                }
            }
        }

        if (notEnoughMeteors)
        {
            GameObject bul = Instantiate(poolMeteors);
            bul.SetActive(false);
            meteors.Add(bul);
            return bul;
        }

        return null;
    }


    public void DisableAll()
    {
        foreach (GameObject lancer in lancers)
        {
            lancer.SetActive(false);
        }
        foreach (GameObject particle in lancersParticles)
        {
            particle.SetActive(false);
        }
    }

    public void DisableParticles()
    {
        foreach (GameObject particle in lancersParticles)
        {
            particle.SetActive(false);
        }
    }

    public void DisableAllSL()
    {
        foreach (GameObject sl in sideLava)
        {
            sl.SetActive(false);
        }
        foreach (GameObject slAm in sideLavaAm)
        {
            slAm.SetActive(false);
        }
    }

    public void DisableParticlesSL()
    {
        foreach (GameObject slAm in sideLavaAm)
        {
            slAm.SetActive(false);
        }
    }
}
