#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

public class Node
{
    private readonly Board position;
    private readonly List<Node> children = new List<Node>();

    private double winValue;
    private int visitCount;
    private bool isSkip;

    public Node(Board position)
    {
        this.position = position;
        winValue = 0;
        visitCount = 0;
        isSkip = false;
    }

    public double getUCB1(Node node)
    {
        if(node.visitCount == 0)
        {
            return double.PositiveInfinity;
        }

        return node.winValue / node.visitCount + 2 * Math.Sqrt(Math.Log(visitCount / node.visitCount));
    }

    public Node? getMaxUCB1()
    {
        if(children.Count == 0)
        {
            return null;
        }

        int maxIndex = 0;
        double maxUCB1 = double.NegativeInfinity;

        for(int i = 0; i < children.Count; ++i)
        {
            double ucb = getUCB1(children.ElementAt(i));

            if(ucb > maxUCB1)
            {
                maxUCB1 = ucb;
                maxIndex = i;
            }
        }

        return children.ElementAt(maxIndex);
    }

    public void addVisit()
    {
        ++visitCount;
    }

    public void addValue(double add)
    {
        winValue += add;
    }

    public void addChild(Node child)
    {
        children.Add(child);
    }

    public List<Node> getChildren()
    {
        return children;
    }

    public Node getChildAt(int index)
    {
        return children.ElementAt(index);
    }

    public Node? getBestChild()
    {
        if(children.Count == 0)
        {
            return null;
        }

        int maxIndex = 0;
        double maxWinRate = 0;

        for(int i = 0; i < children.Count; ++i)
        {
            double winRate = children.ElementAt(i).winValue / children.ElementAt(i).visitCount;

            if(winRate > maxWinRate)
            {
                maxWinRate = winRate;
                maxIndex = i;
            }
        }

        return children.ElementAt(maxIndex);
    }

    public List<int[]> getLegalMoves()
    {
        return position.getLegalMoves();
    }

    public Board getPosition()
    {
        return position;
    }

    public bool getIsSkip()
    {
        return isSkip;
    }
    
    public void setSkip()
    {
        isSkip = true;
    }

    public int getVisitCount()
    {
        return visitCount;
    }

    public double getValue()
    {
        return winValue;
    }
}
