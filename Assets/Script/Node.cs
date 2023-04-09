using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int x = 0;
    public int y = 0;
    public int g = 0;
    public int h = 0;

    public int f
    {
        get
        {
            return g + h;
        }
    }

    public bool isWall = false;
    public Node parentNode;

    public Node(bool isWall, int x, int y)
    {
        this.isWall = isWall;
        this.x = x;
        this.y = y;
    }
}
