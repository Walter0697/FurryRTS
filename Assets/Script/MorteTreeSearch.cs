using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorteTreeSearch : TigerAI
{
    public int AGREESIVE_INDEX = 2;
    public float RESOURCE_INDEX = 10;
    public float DEPTH = 3;

    public KeyCode testButton;

    [HideInInspector] public float C = Mathf.Sqrt(2);
    GameManage gameManage;

    // Start is called before the first frame update
    void Start()
    {
        gameManage = GameObject.FindGameObjectWithTag("GameManage").GetComponent<GameManage>();
        TeamStatus[] ts = gameManage.GetComponents<TeamStatus>();
        for (int i = 0; i < ts.Length; i++)
        {
            if (ts[i].team_name == "tiger")
            {
                status = ts[i];
            }
            else
            {
                others = ts[i];
            }
        }
        manager = gameManage.GetComponent<GameManage>();
        countdown = 0; countdown2 = 0; countdown3 = 0;

        spawnedPos = new bool[housePos.Length];
        for (int i = 0; i < spawnedPos.Length; i++)
            spawnedPos[i] = false;
    }

    // Update is called once per frame
    void Update()
    {
        updateAI();
    }

    public override void updateAI()
    {
        countdown += Time.deltaTime;
        countdown2 += Time.deltaTime;
        countdown3 += Time.deltaTime;

        /*if (Input.GetKeyDown(testButton)) {
            countdown += updateTime;
            countdown2 += checkBuildTime;
            countdown3 += checkFoodTime;
        }*/

        if (countdown >= updateTime)
        {
            countdown -= updateTime;
            Node root = makeTree();
            root = calculateAllValue(root);
            root = root.findMax();
            if (root == null) return;

            if (root.action == "crafting")
            {
                Debug.Log("crafting");
                Debug.Log(root.numOfUnits);
                for (int i = 0; i < status.armies.Count; i++)
                {
                    if (status.armies[i].action != "idle") continue;
                    if (root.numOfUnits == 0) break;
                    root.numOfUnits--;
                    status.armies[i].action = "crafting";
                    status.armies[i].unit.craft = true;
                    status.armies[i].target = root.targetC;
                    status.armies[i].target_pos = status.closestStorage(transform.position).transform.position;
                    status.armies[i].target_pos = new Vector3(status.armies[i].target_pos.x, 0, status.armies[i].target_pos.z);
                }
            }
            else if (root.action == "attacking")
            {
                Debug.Log("attacking");
                for (int i = 0; i < status.armies.Count; i++)
                {
                    if (status.armies[i].action != "idle") continue;
                    if (root.numOfUnits == 0) break;
                    root.numOfUnits--;
                    status.armies[i].action = "attacking";
                    status.armies[i].target = root.target;
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
	
	public float calculateValue(float w, float n, float c, float bigN)
	{
		float firstPart = w / n;
		float squareroot = Mathf.Sqrt(Mathf.Log(bigN) / n);
		return firstPart + c * squareroot;
	}
	

    public Node makeTree(Node current = null, int d = 0)
    {
        if (d == DEPTH) return current;

        int counting = 8;
        if (status.armies.Count < 8) counting = status.armies.Count;

        if (current == null) current = new Node();
        for (int i = 0; i < gameManage.craftables.Length; i++)
        {
            
            for (int j = counting; j > 2; j--)
            {
                Node curr = new Node();
                curr.action = "crafting";
                curr.numOfUnits = j;
                curr.targetC = gameManage.craftables[i];
                curr.win = isWinForResource(curr);
                current.addNode(curr);
            }
        }
        for (int i = 0; i < others.building.Count; i++)
        {
            for (int j = counting; j > 2; j--)
            {
                Node curr = new Node();
                curr.action = "attacking";
                curr.numOfUnits = j;
                curr.target = others.building[i];
                curr.win = isWinForAttack(curr);
                current.addNode(curr);
            }
        }
        return current;
    }

    public Node calculateAllValue(Node n)
    {
        if (n.children.Count == 0)
        {
            n.value = getValue(n);
            return n;
        }

        for (int i = 0; i < n.children.Count; i++)
        {
            n.children[i] = calculateAllValue(n.children[i]);
        }
        n.value = getValue(n);
        return n;
    }

    public float distanceBetween(OnBoardObject obj, OnBoardObject obj2)
    {
        return Vector2.Distance(obj.gameObject.transform.position, obj2.gameObject.transform.position);
    }

    public bool isWinForAttack(Node n)
    {
        float totalAttack = n.numOfUnits * status.armies[0].attackPower; //assuming they attack five time
        float buildingHealth = n.target.health;

        float attackIndex = ((buildingHealth - totalAttack * 8) / 30) * AGREESIVE_INDEX;
        float dist = 20 - (distanceBetween(status.main_build, n.target) / 10);

        if (attackIndex + dist > 20) return true;
        return false;
    }

    public bool isWinForResource(Node n)
    {
        float totalNumOfUnit = n.numOfUnits;
        float resourceValue = Random.Range(1, 3);

        float resourceIndex = resourceValue * RESOURCE_INDEX;
        float dist = 20 - (distanceBetween(status.main_build, n.targetC) / 10);

        if (resourceIndex + dist > 20) return true;
        return false;
    }

    //w -> the number of wins for the node 
    //n -> the number of simulations
    //c -> exploration parameter -> theoreitcally equal to sqrt 2
    //N -> total number of simulation after i-move ran by the parent node of the one
    public float getValue(Node n)
	{
		return calculateValue(n.getNumOfWin(), Node.total_simulation, C, n.getNumOfSimulation());
	}
	

    int getSpawnnablePosition()
    {
        int total = 0;
        for (int i = 0; i < spawnedPos.Length; i++)
            if (spawnedPos[i] == false)
                total += 1;
        return total;
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
