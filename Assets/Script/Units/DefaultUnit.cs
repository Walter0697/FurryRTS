using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultUnit : Mobile
{
    private Rigidbody rb;
    private Animator anim;
    public float closeEnoughDist = 1.0f;
    public float movementCloseEnough = 0.5f;

    public int attackPower = 1;
    public float craftTime = 2f;

    public Resources carrying = null;
    public int maxWeight = 10;
    private bool arrived;

    private float countDown;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        rb.freezeRotation = true;
        action = "idle";
        selected = false;
        arrived = false;
        countDown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector3(0, 0, 0);

        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))//Left mouse btn
        {
            changeSelected(false);
        }
            
        if (Input.GetMouseButtonDown(1))//Right mouse btn
        {
            if (this.selected)
            {
                arrived = false;
                this.target = null;
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
                            countDown = 0;
                            action = "crafting";
                            TeamStatus[] status = GameObject.FindGameObjectWithTag("GameManage").GetComponents<TeamStatus>();
                            target_pos = status[0].closestStorage(transform.position).transform.position;
                            target_pos = new Vector3(target_pos.x, 0, target_pos.z);
                        }
                        Mobile m = o as Mobile;
                        if (m && m.team != team)
                        {
                            action = "attacking";
                            target_pos = new Vector3(target_pos.x, 0, target_pos.z);
                        }
                    }
                }
            }
            
        }
        if (action == "attacking" && target != null)
        {
            if (Vector3.Distance(rb.position, this.target_pos) <= closeEnoughDist)
            {
                target.health -= this.attackPower;
                if (target.health <= 0)
                {
                    Destroy(target.gameObject);
                    target = null;
                    action = "idle";
                }
            }
            else
            {
                target_pos = target.transform.position;
                Vector3 movementDir = this.target_pos - rb.position;
                movementDir.y = 0;
                movementDir.Normalize();
                movement = movementDir * speed;
                rb.transform.LookAt(new Vector3(this.target_pos.x, rb.position.y, this.target_pos.x));
                rb.position += movement * Time.deltaTime;
            }
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

            if (Vector3.Distance(rb.position, this.target_pos) <= movementCloseEnough)
            {
                action = "idle";
            }
        }
        else if (action == "crafting")
        {
            
            if (!target)
            {
                action = "idle";
            }
            else if (carrying == null || carrying.num < maxWeight)
            {
                if (!arrived)
                {
                    //Vector3 targetLoc = new Vector3(target.transform.position.x, 0, target.transform.position.z);
                    Vector3 diff = target.transform.position - transform.position;
                    diff.y = 0;
                    movement = diff.normalized * speed;
                    rb.position += movement * Time.deltaTime;
                }
                else
                {
                    countDown += Time.deltaTime;
                    if (countDown >= craftTime)
                    {
                        Craftable craft = target as Craftable;
                        countDown -= craftTime;
                        if (carrying == null)
                            carrying = craft.getResources();
                        else
                        {
                            Resources crafted = craft.getResources();
                            //may change it later for different resources
                            carrying.num += crafted.num;
                            Debug.Log(carrying.num);
                            Destroy(crafted.gameObject);
                        }

                        if (carrying.num >= maxWeight)
                        {
                            arrived = false;
                        }
                    }
                }
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
            changeSelected(true);
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
                arrived = true;
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
