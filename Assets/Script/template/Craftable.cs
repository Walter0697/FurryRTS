using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craftable : Immobile
{
    public Resources generate;
    public int remainingResouces;
    public int amount;

    // Start is called before the first frame update
    void Start()
    {
        generate.num = amount;
    }

    // Update is called once per frame
    void Update()
    {
        if (remainingResouces <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public Resources getResources()
    {
        Resources res = Instantiate(generate) as Resources;
        res.num = amount;
        remainingResouces -= amount;
        return res;
    }
}
