using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : Immobile
{
    public float max_health;
    // Start is called before the first frame update
    void Start()
    {
        health = max_health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
