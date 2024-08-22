using System.Collections.Generic;
using System.Text;
using System;

public class Board
{
    private int SIZE;
    private int[,] position;
    private int current;
    private List<int[]> legalMoves;

    public Board() : this(8) {}
    
    public Board(int size)
    {
        SIZE = size;
        position = new int[SIZE, SIZE];
        current = 1;
        legalMoves = new List<int[]>(60);

        position[SIZE / 2 - 1, SIZE / 2 - 1] = 2;
        position[SIZE / 2, SIZE / 2] = 2;
        position[SIZE / 2 - 1, SIZE / 2] = 1;
        position[SIZE / 2, SIZE / 2 - 1] = 1;

        findLegalMoves();
    }

    public Board(Board copy)
    {
        SIZE = copy.SIZE;
        current = copy.current;
        legalMoves = copy.legalMoves;
        position = new int[SIZE, SIZE];
        Array.Copy(copy.position, position, SIZE * SIZE);
    }

    public override string ToString()
    {
        StringBuilder output = new StringBuilder(256);

        for(int i = 0; i < SIZE; ++i)
        {
            for(int j = 0; j < SIZE; ++j) 
            {
                output.Append(position[i,j] + " ");
            }

            output.Append("\n");
        }
        if(current == 1)
        {
            output.Append("Black to move.");
        }
        else
        {
            output.Append("White to move.");
        }

        return output.ToString();
    }

    //Finding legal moves

    private void findLegalMoves()
    {
        List<int[]> moves = new(64);

        for(int i = 0; i < SIZE; ++i)
        {
            for(int j = 0; j < SIZE; ++j)
            {
                List<int> legals = legalDirections(i, j);

                if(legals.Count > 0)
                {
                    position[i, j] = 3;
                    moves.Add(new int[] {i, j});
                }
                else if(position[i, j] == 3)
                {
                    position[i, j] = 0;
                }
            }
        }

        legalMoves = moves;
    }

    private List<int> legalDirections(int row, int col)
    {
        if(position[row, col] == 1 || position[row, col] == 2)
        {
            return new List<int>(8);
        }

        List<int> dirs = new List<int>(8);
        for(int i = 0; i < 8; ++i)

        {
            int[] direction = directionMap(i);
            if (isDirLegal(row, col, direction))
            {
                dirs.Add(i);
            }
        }

        return dirs;
    }

    private bool isDirLegal(int row, int col, int[] direction)
    {
        int xPos = row + direction[0];
        int yPos = col + direction[1];
        int otherColor = this.current % 2 + 1;
        int count = 0;
        
        //    pointer is in bounds                                    not traversing blanks      
        while(xPos > -1 && xPos < SIZE && yPos > -1 && yPos < SIZE && position[xPos, yPos] != 0 && position[xPos, yPos] != 3 && (count != 0 || position[xPos, yPos] == otherColor))
        {
            if(position[xPos, yPos] == current)
            {
                return true;
            }

            xPos += direction[0];
            yPos += direction[1];
            ++count;
        }
        
        return false;
    }

    private int[] directionMap(int direction) => direction switch
    {
        0 => new int[] {-1, 0},     //up
        1 => new int[] {-1, 1},     //up right
        2 => new int[] {0, 1},      //right
        3 => new int[] {1, 1},      //down right
        4 => new int[] {1, 0},      //down
        5 => new int[] {1, -1},     //down left
        6 => new int[] {0, -1},     //left
        7 => new int[] {-1, -1},    //up left
        _ => throw new ArgumentOutOfRangeException("Direction: " + direction + " not between 0, 7 inclusive")
    };

        //Making a move

    public void makeMove(int row, int col)
    {
        if(position[row, col] != 3)
        {
            return;
        }
    
        List<int> legals = new(8);
        legals = legalDirections(row, col);
        position[row, col] = current;
    
        foreach(int dir in legals)
        {
            flipPieces(row, col, directionMap(dir));
        }
    
        nextTurn();
    }

    public void makeMove(int[] move) 
    {
        makeMove(move[0], move[1]);
    }

    private void flipPieces(int row, int col, int[] direction)
    {
        int nextRow = row + direction[0];
        int nextCol = col + direction[1];
        int otherColor = current % 2 + 1;
        
        while(position[nextRow, nextCol] == otherColor && nextCol > -1 && nextCol < SIZE && nextRow > -1 && nextRow < SIZE)
        {
            position[nextRow, nextCol] = position[nextRow, nextCol] % 2 + 1;
            nextRow += direction[0];
            nextCol += direction[1];
        }
    }

    public void nextTurn() 
    {
        current = current % 2 + 1;
        findLegalMoves();
    }
    
    public List<int[]> getLegalMoves()
    {
        return legalMoves;
    }

    public int getSize()
    {
        return SIZE;
    }
        
    public int getPieceAt(int row, int col)
    {
        return position[row, col];
    }

    /**
     * 0 - draw
     * 1 - black win
     * 2 - white win
     */
    public int getResult()
    {
        int blackCount = 0;
        int whiteCount = 0;
    
        for(int i = 0; i < SIZE; ++i)
        {
            for(int j = 0; j < SIZE; ++j)
            {
                if(position[i, j] == 1)
                {
                    ++blackCount;
                }
                else if(position[i, j] == 2)
                {
                    ++whiteCount;
                }
            }
        }
        
        if(blackCount > whiteCount) //Black won
        {
            return 1;
        }
        else if(whiteCount > blackCount) //White won
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    public int getCurrent()
    {
        return current;
    }
}
