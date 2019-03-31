using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storagable : Immobile
{
    //public string team;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addResource(Resources res)
    {
        TeamStatus[] status = GameObject.FindGameObjectWithTag("GameManage").GetComponents<TeamStatus>();
        if (status[0].team_name == team)
        {
            status[0].addResources(res);
        }
        else
        {
            //target_pos = status[1].closestStorage(transform.position).transform.position;
        }
    }
}
