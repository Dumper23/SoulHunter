using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorsPool : MonoBehaviour
{
    public static MeteorsPool meteorsPoolInstance;

    [SerializeField]
    private GameObject poolMeteors;

    private bool notEnoughMeteorsInPool = true;

    private List<GameObject> meteors;

    private void Awake()
    {
        meteorsPoolInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        meteors = new List<GameObject>();
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

        if (notEnoughMeteorsInPool)
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
        foreach (GameObject meteor in meteors)
        {
            meteor.SetActive(false);
        }
    }
}
