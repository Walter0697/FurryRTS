using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamStatus : MonoBehaviour
{
    public string team_name;
    public Base main_build;
    public List<Immobile> building;
    public Resources[] storage;
    public List<Mobile> armies;
    public Immobile storage_loc;

    //UI related
    public Image hpShow;
    public Text hpText;
    public Text woodNum;
    public Text fishNum;

    public Immobile defaultHouse;
    public Immobile longrangeHouse;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //updating UI
        hpShow.fillAmount = main_build.health / main_build.max_health;
        hpText.text = ((int)main_build.health).ToString() + "/" + ((int)main_build.max_health).ToString();
        for (int i = 0; i < storage.Length; i++)
        {
            if (storage[i].resource_name == "wood")
                woodNum.text = storage[i].num.ToString();
            else if (storage[i].resource_name == "fish")
                fishNum.text = storage[i].num.ToString();
        }

        if (Input.GetKeyDown("1"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                Vector3 mouseDownPoint = hit.point;
                buildBuilding(0, 0, mouseDownPoint);
            }
        }
        else if (Input.GetKeyDown("2"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000))
            {
                Vector3 mouseDownPoint = hit.point;
                buildBuilding(0, 1, mouseDownPoint);
            }
        }
    }

    public void buildBuilding(int index, int build, Vector3 pos)
    {
        if (index == 0 && team_name == "tiger") return;
        if (index == 1 && team_name == "rabbit") return;
        if (build == 0) //defaultunit
        {
            if (canBuild(50, pos))
            {
                Immobile b = Instantiate(defaultHouse) as Immobile;
                b.transform.position = new Vector3(pos.x, pos.y + 2, pos.z);
                building.Add(b);
            }
        }
        else if (build == 1)
        {
            if (canBuild(80, pos))
            {
                Immobile b = Instantiate(longrangeHouse) as Immobile;
                b.transform.position = new Vector3(pos.x, pos.y + 2, pos.z);
                building.Add(b);
            }
        }
    }

    public bool canBuild(int cost, Vector3 position)
    {
        for (int i = 0; i < storage.Length; i++)
        {
            if (storage[i].resource_name == "wood")
            {
                if (storage[i].num >= cost)
                {
                    storage[i].num -= cost;
                    return true;
                }
            }
        }
        return false;
    }

    public void addResources(Resources res)
    {
        for (int i = 0; i < storage.Length; i++)
        {
            if (storage[i].resource_name == res.resource_name)
            {
                storage[i].num += res.num;
                Destroy(res.gameObject);
                break;
            }
        }
    }

    public int getWood()
    {
        for (int i = 0; i < storage.Length; i++)
        {
            if (storage[i].resource_name == "wood")
                return storage[i].num;
        }
        return 0;
    }

    public int getFish()
    {
        for (int i = 0; i < storage.Length; i++)
        {
            if (storage[i].resource_name == "fish")
                return storage[i].num;
        }
        return 0;
    }

    public void addUnits(Mobile mob)
    {
        armies.Add(mob);
    }

    //set it to searching the closest one later
    public Immobile closestStorage(Vector3 unit)
    {
        return building[0];
    }

    public void healBase(int index)
    {
        if (index == 0 && team_name == "tiger") return;
        if (index == 1 && team_name == "rabbit") return;
        for (int i = 0; i < storage.Length; i++)
        {
            if (storage[i].resource_name == "fish")
            {
                if (storage[i].num >= 10)
                {
                    storage[i].num -= 10;
                    main_build.health = (main_build.health + 500 > main_build.max_health) ? main_build.max_health : main_build.health + 500;
                }
            }
        }
    }

    public void generateGiant(int index)
    {
        if (index == 0 && team_name == "tiger") return;
        if (index == 1 && team_name == "rabbit") return;
        for (int i = 0; i < storage.Length; i++)
        {
            if (storage[i].resource_name == "fish")
            {
                if (storage[i].num >= 50)
                {
                    storage[i].num -= 50;
                    Mobile giant = main_build.generateGiantUnit();
                    armies.Add(giant);
                }
            }
        }
    }
}
