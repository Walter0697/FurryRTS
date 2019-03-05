using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craftable : Immobile
{
    public Resources generate;
    public int amount;

    // Start is called before the first frame update
    void Start()
    {
        generate.num = amount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Resources getResources()
    {
        return generate;
    }
}
