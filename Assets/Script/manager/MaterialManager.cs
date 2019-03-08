using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    [System.Serializable]
    public struct MatCollection {
        public string team_name;
        public Material unselected;
        public Material selected;
    }

    public MatCollection[] all_materials;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Material getMaterial(string team, bool selected)
    {
        for (int i = 0; i < all_materials.Length; i++)
        {
            if (all_materials[i].team_name == team)
            {
                if (selected)
                    return all_materials[i].selected;
                else
                    return all_materials[i].unselected;
            }
        }
        return null;
    }
}
