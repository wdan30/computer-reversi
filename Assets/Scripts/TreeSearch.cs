using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class TreeSearch
{
    private static Node root;
    private static readonly List<Node> visited = new List<Node>();
    private static readonly System.Random rnd = new();

    public static void setRoot(Node rootInput)
    {
        root = rootInput ?? throw new ArgumentNullException("Root node cannot be null.");
    }

    private static Node selection()
    {
        visited.Add(root);
        Node position = root;

        while (position.getChildren().Count > 0)
        {
            position = position.getMaxUCB1();
            visited.Add(position);
        }

        return position;
    }

    private static Node expand(Node leaf)
    {
        //Check if leaf is final
        if(leaf.getLegalMoves().Count() == 0 && leaf.getIsSkip())
        {
            return leaf;
        }
        
        foreach(int[] move in leaf.getLegalMoves())
        {
            Board next = new(leaf.getPosition());
            next.makeMove(move);
            leaf.addChild(new Node(next));
        }

        if(leaf.getLegalMoves().Count == 0) 
        {
            Board next = new(leaf.getPosition());
            next.nextTurn();

            Node nextNode = new(next);
            nextNode.setSkip();
            
            leaf.addChild(nextNode);
        }

        Node randomChild = leaf.getChildAt(rnd.Next(leaf.getChildren().Count));
        visited.Add(randomChild);
        return randomChild;
    }

    private static int simulate(Node leaf)
    {
        Board board = new(leaf.getPosition());
        int skips = 0;

        if(leaf.getIsSkip())
        {
            ++skips;
        }

        while(skips < 2)
        {
            if(board.getLegalMoves().Count > 0)
            {
                board.makeMove(board.getLegalMoves().ElementAt(rnd.Next(board.getLegalMoves().Count)));
                skips = 0;
            }
            else
            {
                board.nextTurn();
                ++skips;
            }
        }

        return board.getResult();
    }

    private static void backpropagate(int result)
    {
        for(int i = 0; i < visited.Count; ++i)
        {
            visited[i].addVisit();

            if(result == 0)
            {
                visited[i].addValue(0.5);    
            }
            else if(result == 1 && visited[i].getPosition().getCurrent() == 2)
            {
                visited[i].addValue(1);
            }
            else if(result == 2 && visited[i].getPosition().getCurrent() == 1)
            {
                visited[i].addValue(1);
            }
        }

        visited.Clear();
    }

    public static Node analyze(int time)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        while(sw.Elapsed.TotalSeconds < time)
        {
            Node leaf = selection();
            leaf = expand(leaf);
            backpropagate(simulate(leaf));
        }

        sw.Stop();
        
        if(root.getBestChild() != null)
        {
            return root.getBestChild();
        }
        else
        {
            throw new Exception("No legal moves found.");
        }
    }

    public static void updateRoot(int[] move)
    {
        updateRoot(move[0], move[1]);
    }

    public static void updateRoot(int row, int col)
    {
        if(root.getChildren().Count == 0)
        {
            expand(root);
        }

        foreach(Node child in root.getChildren())
        {
            if(child.getPosition().getPieceAt(row, col) == root.getPosition().getCurrent())
            {
                root = child;
                return;
            }
        }
    }
}
