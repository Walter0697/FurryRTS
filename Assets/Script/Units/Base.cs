using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : Immobile
{
    public float max_health;
    public Mobile giantUnit;
    public Transform spawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        health = max_health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Mobile generateGiantUnit()
    {
        Mobile giant = Instantiate(giantUnit) as Mobile;
        giant.transform.position = spawnPosition.position;
        return giant;
    }
}
