using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private Grid<PathNode> isoGrid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable = true;
    
    public PathNode cameFromNode;

    public CultMember cultMember;

    public PathNode(Grid<PathNode> isoGrid, int x, int y)
    {
        this.isoGrid = isoGrid;
        this.x = x;
        this.y = y;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public bool IsWalkable()
    {
        return isWalkable;
    }

    public bool HasCultMember()
    {
        if (cultMember != null)
        {
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}
