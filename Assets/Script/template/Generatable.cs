using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generatable : Immobile
{
    public Mobile generateUnit;
    public float spawnTime = 5f;
    private float cooldown = 0f;

    public Transform spawnPosition;

    private TeamStatus status;

    // Start is called before the first frame update
    void Start()
    {
        TeamStatus[] ts = GameObject.FindGameObjectWithTag("GameManage").GetComponents<TeamStatus>();
        for (int i = 0; i < ts.Length; i++)
        {
            if (ts[i].team_name == generateUnit.team)
            {
                status = ts[i];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        cooldown += Time.deltaTime;
        if (cooldown >= spawnTime)
        {
            cooldown -= spawnTime;
            Mobile mob = Instantiate(generateUnit) as Mobile;
            mob.transform.position = spawnPosition.position;
            status.addUnits(mob);
        }
    }
}
