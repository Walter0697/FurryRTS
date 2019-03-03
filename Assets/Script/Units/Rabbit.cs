using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : Mobile
{
    private Rigidbody rb;
    private Animator anim;

    public float speed = 1;

    public Vector3 movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector3(0, 0, speed);
        rb.position += movement * Time.deltaTime;
        checkAnimate();
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
}
