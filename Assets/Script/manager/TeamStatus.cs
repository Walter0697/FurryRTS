using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamStatus : MonoBehaviour
{
    public string team_name;
    public Base main_build;
    public Immobile[] building;
    public Resources[] storage;
    public List<Mobile> armies;

    //UI related
    public Image hpShow;
    public Text hpText;
    public Text woodNum;
    public Text fishNum;

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

    public void addUnits(Mobile mob)
    {
        armies.Add(mob);
    }

    //set it to searching the closest one later
    public Immobile closestStorage(Vector3 unit)
    {
        return building[0];
    }
}
