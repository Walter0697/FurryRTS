using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerAI : MonoBehaviour
{
	public TeamStatus status;
	public TeamStatus others;
	public GameManage manager;

	public float updateTime = 1f;
	private float countdown;

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
		countdown = 0;
    }

    // Update is called once per frame
    void Update()
    {
		countdown += Time.deltaTime;
		if (countdown >= updateTime) {
			int numOfOption = manager.GetNumOfCraftable();
			countdown -= updateTime;
			foreach (Mobile ele in status.armies) {
				if (ele.action == "idle")
					performOption(ele, Random.Range(0, numOfOption));
			}
		}
    }

	void performOption(Mobile m, int option)
	{
		if (option < manager.GetNumOfCraftable ()) {
			m.target = manager.GetCraftable(option);
			m.unit.craft = true;
			m.action = "crafting";
			m.target_pos = status.closestStorage(transform.position).transform.position;
			m.target_pos = new Vector3(m.target_pos.x, 0, m.target_pos.z);
		}
	}
}
