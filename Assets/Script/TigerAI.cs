using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerAI : MonoBehaviour
{
	[HideInInspector] public TeamStatus status;
    [HideInInspector] public TeamStatus others;
    [HideInInspector] public GameManage manager;

    public Transform[] housePos;
    [HideInInspector] public bool[] spawnedPos;

	public float updateTime = 1f;
	public float countdown;
    public float checkBuildTime = 3f;
    public float countdown2;
    public float checkFoodTime = 3f;
    public float countdown3;

    // Start is called before the first frame update
    void Start()
    {
		GameObject gameManage = GameObject.FindGameObjectWithTag ("GameManage");
		TeamStatus[] ts = gameManage.GetComponents<TeamStatus>();
		for (int i = 0; i < ts.Length; i++)
		{
			if (ts [i].team_name == "tiger") {
				status = ts [i];
			} else {
				others = ts [i];
			}
		}
		manager = gameManage.GetComponent<GameManage> ();
		countdown = 0;   countdown2 = 0;    countdown3 = 1;

        spawnedPos = new bool[housePos.Length];
        for (int i = 0; i < spawnedPos.Length; i++)
            spawnedPos[i] = false;
    }

    public virtual void updateAI()
    {
        countdown += Time.deltaTime;
        countdown2 += Time.deltaTime;
        countdown3 += Time.deltaTime;

        if (countdown >= updateTime)
        {
            countdown -= updateTime;


            //unit chose to attack
            foreach (Mobile ele in status.armies)
            {
                if (enoughCost())
                {
                    int numOfOption = others.building.Count + 1;
                    int randNum = Random.Range(0, numOfOption);
                    if (randNum == numOfOption - 1)
                    {
                        ele.target = others.main_build;
                        ele.action = "attacking";
                    }
                    else
                    {
                        ele.target = others.building[randNum];
                        ele.action = "attacking";
                    }
                }
                else
                {
                    if (ele.action == "idle")
                    {
                        int numOfOption = getNumOfOption();

                        performOption(ele, Random.Range(0, numOfOption));
                    }
                }
            }
        }

        if (countdown2 >= checkBuildTime)
        {
            countdown2 -= checkBuildTime;
            int numOfOption = getSpawnnablePosition();

            if (status.getWood() > 100)
            {
                pickToBuild(Random.Range(0, numOfOption));
            }
        }

        if (countdown3 >= checkFoodTime)
        {
            countdown3 -= checkFoodTime;

            if (status.getFish() > 20)
            {
                if (status.main_build.max_health / 2 > status.main_build.health)
                {
                    status.healBase(1);
                }
                else if (status.getFish() > 50)
                {
                    status.generateGiant(1);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateAI();
    }

    //if the cost is enough for computer to choose attacking
    bool enoughCost()
    {
        if (status.building.Count * 30 + status.armies.Count > 300)
            return true;
        return false;
    }

    int getNumOfOption()
    {
        return manager.GetNumOfCraftable() + others.building.Count;
    }

    int getSpawnnablePosition()
    {
        int total = 0;
        for (int i = 0; i < spawnedPos.Length; i++)
            if (spawnedPos[i] == false)
                total += 1;
        return total;
    }

	void performOption(Mobile m, int option)
	{
        if (option < manager.GetNumOfCraftable())
        {
            m.target = manager.GetCraftable(option);
            m.unit.craft = true;
            m.action = "crafting";
            m.target_pos = status.closestStorage(transform.position).transform.position;
            m.target_pos = new Vector3(m.target_pos.x, 0, m.target_pos.z);
        }
        else
        {
            m.target = others.building[option - manager.GetNumOfCraftable()];
            //going to attack
        }
	}

    void pickToBuild(int option)
    {
        int j = 0;
        for (int i = 0; i < spawnedPos.Length; i++)
        {
            if (j == option)
            {
                status.buildBuilding(1, Random.Range(0, 1), housePos[i].position);
                spawnedPos[i] = true;
                break;
            }
            else
            {
                if (spawnedPos[i] == false) j++;
            }
        }
    }
}
