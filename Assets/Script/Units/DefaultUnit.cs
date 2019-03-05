using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultUnit : Mobile
{
    private Rigidbody rb;
    private Animator anim;

    public string team;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.freezeRotation = true;
        action = "running";
    }

    // Update is called once per frame
    void Update()
    {
        if (action == "running")
        {
            movement = new Vector3(0, 0, speed);
            rb.position += movement * Time.deltaTime;
            checkAnimate();
        }
    }

    private void checkAnimate()
    {
        if (movement.magnitude == 0)
        {
            if (anim.GetInteger("AnimIndex") != 0) anim.SetTrigger("Next");
            anim.SetInteger("AnimIndex", 0);
        }
        else
        {
            if (anim.GetInteger("AnimIndex") != 1) anim.SetTrigger("Next");
            anim.SetInteger("AnimIndex", 1);
        }
    }

    void OnMouseUp()
    {
        if (team == "rabbit")  //change it later according to the game manage
        {
            selected = true;
        }
    }
    //BY CLICKING THE UNIT, MEANING SELECT
    //BUT FOR TESTING, MAKE THEM SWITCH TO CRAFTING
    //SET UP THE RESOURCES HOUSE, SO THAT AFTER CRAFTING, IT WILL GO TO THE BANK
    //WELL WELL WELL, GAME MANAGE SHOULD HAVE BOTH TEAM STATUS
    //GAME MANAGE SHOULD HAVE REFERENCE FOR WHERE IS THE BUILDING AND FUNCTION TO FIND THE CLOSEST BUILDING

}
