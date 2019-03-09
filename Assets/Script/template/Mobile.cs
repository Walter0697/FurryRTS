using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mobile : OnBoardObject {

    public string unit_type;
    public string action;           //might be private, dont know yet
    //running / crafting / building / defending (idie?)
    public bool selected;

    //movement related
    public OnBoardObject target;
    public Vector3 target_pos;

    public float speed = 1;
    public Vector3 movement;
    public string team;

    //status
    
    public bool dead;

    protected SkinnedMeshRenderer mesh;

    // Use this for initialization
    void Start () {
        selected = false;
        dead = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    protected void changeSelected(bool option)
    {
        selected = option;
        MaterialManager mm = GameObject.FindGameObjectWithTag("MaterialManage").GetComponent<MaterialManager>();
        mesh.material = mm.getMaterial(team, selected);
    }
}
