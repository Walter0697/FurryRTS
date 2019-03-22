using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManage : MonoBehaviour
{
    public Craftable[] craftables;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //should replace by mouse click instead of this
    public Craftable GetCraftable()
    {
        return craftables[Random.Range(0, craftables.Length)];
    }
}
