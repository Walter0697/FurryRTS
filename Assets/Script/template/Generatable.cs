using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generatable : Immobile
{
    public Mobile generateUnit;
    public float spawnTime = 5f;
    private float cooldown = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cooldown += Time.deltaTime;
        if (cooldown >= spawnTime)
        {
            cooldown -= spawnTime;
            Debug.Log("spawn!");
            Mobile mob = Instantiate(generateUnit) as Mobile;
            mob.transform.position = transform.position;
        }
    }
}
