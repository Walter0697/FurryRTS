using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamStatus : MonoBehaviour
{
    public string team_name;
    public Immobile[] building;
    public Resources[] storage;
    public Mobile[] armies;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addResources(Resources res)
    {
        for (int i = 0; i < storage.Length; i++)
        {
            if (storage[i].resource_name == res.resource_name)
            {
                storage[i].num += res.num;
                Destroy(res.gameObject);
                break;
            }
        }
    }

    //set it to searching the closest one later
    public Immobile closestStorage(Vector3 unit)
    {
        return building[0];
    }
}
