using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class downLancersPool : MonoBehaviour
{
    public static downLancersPool downLancersPoolInstance;

    [SerializeField]
    private GameObject
        poolLancers,
        poolParticles;
    //    poolSummoner,
    //    poolHealer
      
    private bool notEnoughLancersInPool = true;
    private bool notEnoughParticlesInPool = true;
    //private bool notEnoughSummonersPool = true;
    //private bool notEnoughHealersInPool = true;

    private List<GameObject> lancers;
    private List<GameObject> lancersParticles;
    //private List<GameObject> summoners;
    //private List<GameObject> healers;

    private void Awake()
    {
        downLancersPoolInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        lancers = new List<GameObject>();
        lancersParticles = new List<GameObject>();
        //summoners = new List<GameObject>();
        //healers = new List<GameObject>();
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
    //No se si hauria de ser així per optimitzar
    /*public GameObject GetSummoner()
    {
        if (summoners.Count > 0)
        {
            for (int i = 0; i < summoners.Count; i++)
            {
                if (!summoners[i].activeInHierarchy)
                {
                    return summoners[i];
                }
            }
        }

        if (notEnoughSummonersPool)
        {
            GameObject bul = Instantiate(poolSummoner);
            bul.SetActive(false);
            summoners.Add(bul);
            return bul;
        }

        return null;
    }
    public GameObject GetHealer()
    {
        if (healers.Count > 0)
        {
            for (int i = 0; i < healers.Count; i++)
            {
                if (!healers[i].activeInHierarchy)
                {
                    return healers[i];
                }
            }
        }

        if (notEnoughHealersInPool)
        {
            GameObject bul = Instantiate(poolHealer);
            bul.SetActive(false);
            healers.Add(bul);
            return bul;
        }

        return null;
    }*/

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
}
