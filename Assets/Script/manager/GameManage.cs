using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManage : MonoBehaviour
{
    public Craftable[] craftables;
    public Text winningText;
    private TeamStatus[] status;
    private bool endgame;

    // Start is called before the first frame update
    void Start()
    {
        status = GetComponents<TeamStatus>();
        winningText.enabled = false;
        endgame = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!endgame)
        {
            for (int i = 0; i < status.Length; i++)
            {
                if (status[i].main_build.health <= 0)
                {
                    if (status[i].team_name == "rabbit")
                        winningText.text = "You win!";
                    else
                        winningText.text = "You lose!";
                    winningText.enabled = true;
                    endgame = true;
                    break;
                }
            }
        }
    }

    //should replace by mouse click instead of this
	public Craftable GetCraftable(int index = -1)
    {
		if (index == -1)
        	return craftables[Random.Range(0, craftables.Length)];
		return craftables[index];
    }

	public int GetNumOfCraftable()
	{
		return craftables.Length;
	}
}
