using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultUnit : Mobile
{
    private Rigidbody rb;
    //public float closeEnoughDist = 1.0f;
    public float movementCloseEnough = 0.5f;
    public float attackDistance = 2.0f;

    public float craftTime = 2f;

    private Vector3 lookDir = new Vector3(1,0,0);

    private float maxforce = 1;
    public static Vector3 mouseDownPoint;
    public float autoAttackDist = 10.0f;

    public Resources carrying = null;
    public Transform carryingPosition;
    public int maxWeight = 10;
    private bool arrived;

    private int pathfindingTime = 0;
    private float pathfindingDetectionDist = 5.0f;

    private float countDown;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        unit = GetComponent<Unit>();
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        //rb.freezeRotation = true;
        action = "idle";
        selected = false;
        arrived = false;
        countDown = 0;

        unit.movement = new Vector3(0, 0, 0);
    }


    //Flocking Behavior

    // Separation
    // Method checks for nearby boids and steers away
    public Vector2 separate(ArrayList boids)
    {
        float desiredseparation = 1.0f;
        Vector2 steer = new Vector2(0, 0);
        int count = 0;
        // For every boid in the system, check if it's too close
        foreach (Mobile other in boids)
        {
            float d = Vector2.Distance(this.transform.position, other.transform.position);
            // If the distance is greater than 0 and less than an arbitrary amount (0 when you are yourself)
            if ((d > 0) && (d < desiredseparation))
            {
                // Calculate vector pointing away from neighbor
                Vector2 diff = this.transform.position - other.transform.position;
                diff.Normalize();
                diff /= d;        // Weight by distance
                steer += diff;
                count++;            // Keep track of how many
            }
        }
        // Average -- divide by how many
        if (count > 0)
        {
            steer /= (float)count;
        }

        // As long as the vector is greater than 0
        if (steer.magnitude > 0)
        {
            // First two lines of code below could be condensed with new PVector setMag() method
            // Not using this method until Processing.js catches up
            // steer.setMag(maxspeed);

            // Implement Reynolds: Steering = Desired - Velocity

            steer.Normalize();
            steer *= speed * 0.2f;
            steer -= velocity;
            if (steer.magnitude > maxforce)
            {
                steer.Normalize();
                steer *= maxforce;
            }
        }
        return steer;
    }


    // Alignment
    // For every nearby boid in the system, calculate the average velocity
    public Vector2 align(ArrayList boids)
    {
        Vector2 sum = new Vector2(0, 0);
        int count = 0;
        foreach (Mobile other in boids)
        {
            float d = Vector2.Distance(this.transform.position, other.transform.position);
            //if ((d > 0) && (d < neighbordist))
            if(other == target)
            {
                sum += other.velocity;
                count++;
            }
        }
        if (count > 0)
        {
            sum /= (float)count;
            // First two lines of code below could be condensed with new PVector setMag() method
            // Not using this method until Processing.js catches up
            // sum.setMag(maxspeed);

            // Implement Reynolds: Steering = Desired - Velocity
            sum.Normalize();
            sum *= speed * 0.2f;
            Vector2 steer = sum - velocity;
            if (steer.magnitude > maxforce)
            {
                steer.Normalize();
                steer *= maxforce * 0.01f;
            }
            return steer;
        }
        else
        {
            return new Vector2(0, 0);
        }
    }

    Vector2 seek(Vector2 target)
    {
        Vector2 desired = target - new Vector2(transform.position.x, transform.position.z);  // A vector pointing from the position to the target
                                                          // Scale to maximum speed
        desired.Normalize();
        desired *= speed * 0.2f;

        // Above two lines of code below could be condensed with new PVector setMag() method
        // Not using this method until Processing.js catches up
        // desired.setMag(maxspeed);

        // Steering = Desired minus Velocity
        Vector2 steer = desired - velocity;
        if (steer.magnitude > maxforce)
        {
            steer.Normalize();
            steer *= maxforce;
        }
        return steer;
    }

    // Cohesion
    // For the average position (i.e. center) of all nearby boids, calculate steering vector towards that position
    public Vector2 cohesion(ArrayList boids)
    {
        float neighbordist = 1000;
        Vector2 sum = new Vector2(0, 0);   // Start with empty vector to accumulate all positions
        int count = 0;
        foreach (Mobile other in boids)
        {
            float d = Vector2.Distance(this.transform.position, other.transform.position);
            if (((d > 0) && (d < neighbordist)) || other == target)
            {
                sum += new Vector2(other.transform.position.x, other.transform.position.z); // Add position
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count;
            return seek(sum);  // Steer towards the position
        }
        else
        {
            return new Vector2(0, 0);
        }
    }





    // Update is called once per frame
    void Update()
    {
        unit.movement *= 0;
        unit.craft = false;
        if (Input.GetMouseButtonDown(0))//Left mouse btn
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                changeSelected(false);
            }

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                mouseDownPoint = hit.point;
            }

            //Draw box
            
        }
        else if (Input.GetMouseButtonUp(0)) // LMB let go
        {
            //Select items inside box
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                float largerX = (hit.point.x > mouseDownPoint.x ? hit.point.x : mouseDownPoint.x);
                float largerZ = (hit.point.z > mouseDownPoint.z ? hit.point.z : mouseDownPoint.z);
                float smallerX = (hit.point.x < mouseDownPoint.x ? hit.point.x : mouseDownPoint.x);
                float smallerZ = (hit.point.z < mouseDownPoint.z ? hit.point.z : mouseDownPoint.z);

                if(largerX > transform.position.x && transform.position.x > smallerX &&
                   largerZ > transform.position.z && transform.position.z > smallerZ)
                {
                    this.select();
                }
            }
            GameObject box = GameObject.Find("SelectionBox");
            Vector3 pos = new Vector3(0, 0, 0);
            Vector3 scale = new Vector3(0, 0, 0);
            box.transform.position = pos;
            box.transform.localScale = scale;
        }

        if (Input.GetMouseButton(0)) // LMB Held down
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                GameObject box = GameObject.Find("SelectionBox");
                Vector3 pos = new Vector3((hit.point.x + mouseDownPoint.x) / 2, 10, (hit.point.z + mouseDownPoint.z) / 2);
                Vector3 scale = new Vector3(Mathf.Abs(hit.point.x - mouseDownPoint.x), 1, Mathf.Abs(hit.point.z - mouseDownPoint.z));
                box.transform.position = pos;
                box.transform.localScale = scale;
            }
        }
        
        if (Input.GetMouseButtonDown(1))//Right mouse btn
        {
            if (this.selected)
            {
                acceleration = new Vector2(0, 0);
                velocity = new Vector2(0, 0);
                arrived = false;
                this.target = null;
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    
                    target_pos = hit.point;
                    lookDir = new Vector3(target_pos.x - transform.position.x, 0, target_pos.z - transform.position.z);
                    //rb.freezeRotation = false;

                    //rb.freezeRotation = true;
                    action = "running";
                    OnBoardObject o = hit.transform.gameObject.GetComponent<OnBoardObject>();
                    
                    
                    if (o) {
                        this.target = o;
                        Craftable c = o as Craftable;
                        Mobile m = o as Mobile;
                        Immobile i = o as Immobile;
                        if (c)
                        {
                            unit.craft = true;
                            countDown = 0;
                            action = "crafting";
                            TeamStatus[] status = GameObject.FindGameObjectWithTag("GameManage").GetComponents<TeamStatus>();
                            target_pos = status[0].storage_loc.transform.position;
                            target_pos = new Vector3(target_pos.x, 0, target_pos.z);
                        }
                        else if (m && m.team != team)
                        {
                            action = "attacking";
                            target_pos = new Vector3(this.target.transform.position.x, 0, this.target.transform.position.z);
                        }
                        else if (m && m.team == team)
                        {
                            action = "flocking";
                            target_pos = new Vector3(this.target.transform.position.x, 0, this.target.transform.position.z);
                        }
                        else if (i && i.team != team)
                        {
                            action = "crafting";
                            target_pos = new Vector3(this.target.transform.position.x, 0, this.target.transform.position.z);
                        }
                    }
                }
            }
            
        }
        //Auto-attacking

        Mobile[] mobile = FindObjectsOfType<Mobile>();
        foreach (Mobile m in mobile)
        {
            if ((action == "idle" || action == "flocking" || action == "crafting") && m.team != team && Vector3.Distance(transform.position, m.transform.position) <= autoAttackDist)
            {
                action = "attacking";
                target = m;
                target_pos = new Vector3(this.target.transform.position.x, 0, this.target.transform.position.z);
            }
        }


        if (action == "attacking" && target != null)
        {
            if (Vector3.Distance(rb.position, this.target_pos) <= attackDistance)
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
                unit.movement = movementDir * speed;
                lookDir = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
                rb.position += unit.movement * Time.deltaTime;

                
            }
        }
        else if (action == "running")
        {
            Vector3 movementDir = this.target_pos - rb.position;

            RaycastHit hit;
            //Debug.DrawRay(transform.position + new Vector3(0,1,0), transform.forward * pathfindingDetectionDist, Color.white, 100.0f, false);
            if (Physics.Raycast(transform.position + new Vector3(0,1,0), transform.forward, out hit, pathfindingDetectionDist)){
                Immobile i = hit.transform.gameObject.GetComponent<Immobile>();
                Mobile m = hit.transform.gameObject.GetComponent<Mobile>();

                if (i || m)
                {
                    Vector3 c = (this.target_pos - rb.position);
                    Vector3 t = Vector3.Cross(new Vector3(c.x,0,c.z), new Vector3(0, 1, 0));
                    //t.Normalize();
                    movementDir = t;
                }
            }
            //should follow the target instead of running meaninglessly
            movementDir.y = 0;
            movementDir.Normalize();
            unit.movement = movementDir * speed;
            rb.transform.LookAt(new Vector3(this.target_pos.x, rb.position.y, this.target_pos.x));
            rb.position += unit.movement * Time.deltaTime;


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
                    //target_pos = target.transform.position;
                    lookDir = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
                    diff.y = 0;
                    unit.movement = diff.normalized * speed;
                    rb.position += unit.movement * Time.deltaTime;
                }
                else
                {
                    countDown += Time.deltaTime;
                    if (countDown >= craftTime)
                    {
                        Craftable craft = target as Craftable;
                        countDown -= craftTime;
                        if (carrying == null)
                        {
                            carrying = craft.getResources();
                            carrying.transform.position = carryingPosition.transform.position;
                            carrying.transform.parent = transform;
                        }
                        else
                        {
                            Resources crafted = craft.getResources();
                            //may change it later for different resources
                            carrying.num += crafted.num;
                            //Debug.Log(carrying.num);
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
                unit.movement = diff.normalized * speed;
                rb.position += unit.movement * Time.deltaTime;
                lookDir = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
            }
        }
        else if (action == "flocking")
        {
            //velocity = new Vector2(0,0);
            target_pos = target.transform.position;
            //Vector3 temp = rb.position - target_pos;
            //temp.y = 0;
            //if (temp.magnitude > closeEnoughDist)
            //{
            //    temp.Normalize();
                //velocity = new Vector2(temp.x, temp.z) * speed;
            //}
            

            Mobile[] mobs = FindObjectsOfType<Mobile>();
            ArrayList flocking = new ArrayList();
            foreach(Mobile m in mobs)
            {
                if((m.action == "flocking" && m.team == team && m.target == target) || m == target)
                {
                    flocking.Add(m);
                }
            }
            Vector2 sep = separate(flocking);
            Vector2 ali = align(flocking);
            Vector2 coh = cohesion(flocking);

            acceleration = new Vector2(0, 0);
            acceleration += sep * 0.01f;
            acceleration += ali * 0.01f;
            acceleration += coh * 0.01f;

            velocity += acceleration;
            if (velocity.magnitude > speed)
            {
                velocity.Normalize();
                velocity *= speed * 0.2f;
            }
            transform.Translate(new Vector3(velocity.x, 0, velocity.y));
            acceleration *= 0;

        }
        else if (action == "idle")
        {
            unit.movement *= 0;
        }
        /*if (transform.position.y < 0)
        {
            Vector3 temp = transform.position;
            temp.y = 10;
            transform.position = temp;
        }*/
        Quaternion q = Quaternion.LookRotation(lookDir, new Vector3(0, 1, 0));
        transform.rotation = q;
    }

    void OnMouseUp()
    {
        if (team == "rabbit" || team == "tiger")  //change it later according to the game manage
        {
            select();
        }
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
