using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public float value;
	public string action;
    public int numOfUnits;
    public Immobile target;
    public Craftable targetC;

    public bool win;
    public static int total_simulation = 0;
    public List<Node> children = new List<Node>();

    public void addNode(Node n)
    {
        children.Add(n);
        total_simulation += 1;
    }

    public int getNumOfSimulation()
    {
        if (children.Count == 0) return 1;
        int sum = 0;
        for (int i = 0; i < children.Count; i++)
        {
            sum += children[i].getNumOfSimulation();
        }
        return sum;
    }

    public int getNumOfWin()
    {
        if (children.Count == 0)
        {
            if (win) return 1;
            return 0;
        }
        int sum = 0;
        for (int i = 0; i < children.Count; i++)
        {
            sum += children[i].getNumOfWin();
        }
        return sum;
    }

    public Node findMax()
    {
        if (children.Count == 0) return null;

        float maxvalue = children[0].value;
        Node maxNode = children[0];
        for (int i = 1; i < children.Count; i++)
        {
            if (children[i].value > maxvalue)
            {
                maxvalue = children[i].value;
                maxNode = children[i];
            }
        }
        return maxNode;
    }

    public Node findMin()
    {
        if (children.Count == 0) return null;

        float minvalue = children[0].value;
        Node minNode = children[0];
        for (int i = 1; i < children.Count; i++)
        {
            if (children[i].value < minvalue)
            {
                minvalue = children[i].value;
                minNode = children[i];
            }
        }
        return minNode;
    }
}
