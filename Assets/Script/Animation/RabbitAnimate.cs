using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitAnimate : AnimateControl
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (unit.dead) anim.SetBool("Dead", true);
        else
            anim.SetFloat("Speed", unit.movement.magnitude);
    }
}
