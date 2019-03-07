using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultUnit : Mobile
{
    private Rigidbody rb;
    private Animator anim;
    public float closeEnoughDist = 0.5f;

    public string team;
    private void OnDrawGizmosSelected()
    {
        
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.freezeRotation = true;
        action = "idle";
        selected = false;
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector3(0, 0, 0);
        if (Input.GetMouseButtonDown(1))
        {
            this.target = null;
            if (this.selected)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    
                    target_pos = hit.point;
                    action = "running";
                    OnBoardObject o = hit.transform.gameObject.GetComponent<OnBoardObject>();
                    if (o) {
                        this.target = o;
                        Craftable c = o as Craftable;
                        if (c)
                        {
                            action = "crafting";
                            TeamStatus[] status = GameObject.FindGameObjectWithTag("GameManage").GetComponents<TeamStatus>();
                            target_pos = status[0].closestStorage(transform.position).transform.position;
                            target_pos = new Vector3(target_pos.x, 0, target_pos.z);
                        }
                    }
                }
            }
            
        }
        if (action == "attack" && target != null)
        {
            target_pos = target.transform.position;
        }
        if (action == "running")
        {
            //should follow the target instead of running meaninglessly
            Vector3 movementDir = this.target_pos - rb.position;
            movementDir.y = 0;
            movementDir.Normalize();
            movement = movementDir * speed;
            rb.transform.LookAt(new Vector3(this.target_pos.x, rb.position.y, this.target_pos.x));
            rb.position += movement * Time.deltaTime;

            if (Vector3.Distance(rb.position, this.target_pos) <= closeEnoughDist)
            {
                action = "idle";
            }
        }
        else if (action == "crafting")
        {
            if (carrying == null)
            {
                //Vector3 targetLoc = new Vector3(target.transform.position.x, 0, target.transform.position.z);
                Vector3 diff = target.transform.position - transform.position;
                diff.y = 0;
                movement = diff.normalized * speed;
                rb.position += movement * Time.deltaTime;
            }
            else
            {
                Vector3 diff = target_pos - transform.position;
                diff.y = 0;
                movement = diff.normalized * speed;
                rb.position += movement * Time.deltaTime;
            }
        }
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

    void OnMouseUp()
    {
        if (team == "rabbit" || team == "tiger")  //change it later according to the game manage
        {
            select();
        }

    }

    private void OnMouseDrag()
    {
        // Implement a way to draw a box around multiple units and select them all
    }

    public void select()
    {
        
        //action = "crafting";
        target = GameObject.FindGameObjectWithTag("GameManage").GetComponent<GameManage>().GetCraftable();
        TeamStatus[] status = GameObject.FindGameObjectWithTag("GameManage").GetComponents<TeamStatus>();
        if (status[0].team_name == team)
        {
            selected = true;
            //target_pos = status[0].closestStorage(transform.position).transform.position;
            //target_pos = new Vector3(target_pos.x, 0, target_pos.z);
            //target_pos = new Vector3(300, 0, 200);
        }
        else
        {
            //target_pos = status[1].closestStorage(transform.position).transform.position;
        }
    }
    //BY CLICKING THE UNIT, MEANING SELECT
    //BUT FOR TESTING, MAKE THEM SWITCH TO CRAFTING
    //SET UP THE RESOURCES HOUSE, SO THAT AFTER CRAFTING, IT WILL GO TO THE BANK
    //WELL WELL WELL, GAME MANAGE SHOULD HAVE BOTH TEAM STATUS
    //GAME MANAGE SHOULD HAVE REFERENCE FOR WHERE IS THE BUILDING AND FUNCTION TO FIND THE CLOSEST BUILDING

    private void OnCollisionEnter(Collision collision)
    {
        if (action == "crafting")
        {
            Craftable craft = collision.gameObject.GetComponent<Craftable>();
            if (craft && craft == target)
            {
                carrying = craft.getResources();
            }
            else if (!craft && carrying != null)
            {
                Storagable storage = collision.gameObject.GetComponent<Storagable>();
                if (storage && storage.team == team)
                {
                    storage.addResource(carrying);
                    carrying = null;
                }
            }
        }
    }
}
