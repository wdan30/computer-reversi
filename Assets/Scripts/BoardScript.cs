using System;
using System.Collections;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    private readonly int SIZE = 8;   
    private Board board;
    private GameObject[] pieces;
    private Camera cam;
    private bool isOver = false;
    private int skips;
    private int[] prevMove;
    private int mode; //0 is twoPlayer, 1 is onePlayer
    private int computerColour = 1;
    private HUDControllerScript hudController;

    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    [SerializeField] private float squareHeight;
    [SerializeField] private float squareWidth;
    [SerializeField] private GameObject square;
    [SerializeField] private GameObject piece;

    // Start is called before the first frame update
    void Start()
    {
        mode = MenuControllerScript.getMode();
        computerColour = MenuControllerScript.getComputerColour();
        pieces = new GameObject[SIZE * SIZE];
        board = new Board(SIZE);
        cam = Camera.main;
        hudController = GameObject.FindGameObjectWithTag("HUDController").GetComponent<HUDControllerScript>();
        drawBoard();
        initPieces();
        setColors();
    }

    void Update()
    {
        if(isOver)
        {
            return;
        }

        StartCoroutine(doMove());
    }

    IEnumerator doMove()
    {
        if(!isOver && board.getLegalMoves().Count > 0)
        {
            skips = 0;
            StartCoroutine(doTurn());
            setColors();
        }
        else if(!isOver && skips == 0)
        {
            ++skips;

            hudController.setSkipText(true);
            hudController.setTurnText(board.getCurrent());

            board.nextTurn();
            yield return new WaitForSeconds(0.75f);

            hudController.setTurnText(board.getCurrent());
            hudController.setSkipText(false);

            setColors();
        }
        else if(!isOver && skips == 1)
        {
            hudController.setWinText(board.getResult());
            isOver = true;
        }

        yield return null;
    }

    IEnumerator doTurn()
    {
        if(mode == 0 || board.getCurrent() != computerColour)
        {
            while(true)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
                    int x = (int) Math.Floor(pos.y) * -1 + 3;
                    int y = (int) Math.Floor(pos.x) + 4;
                    prevMove = new int[] {x, y};

                    if(board.getPieceAt(x, y) == 3)
                    {
                        hudController.setTurnText(computerColour);
                    }

                    board.makeMove(x, y);

                    yield break;
                }

                yield return null;
            }
        }
        else
        {
            TreeSearch.setRoot(new Node(board));

            yield return computerThink();

            board.makeMove(prevMove);
        }   

        yield return null;
    }

    object computerThink()
    {
        Node nextPos = TreeSearch.analyze(2);
        prevMove = findMovePlayed(nextPos.getPosition());
        return null;
    }

    int[] findMovePlayed(Board newPosition)
    {
        for(int i = 0; i < board.getSize(); ++i)
        {
            for(int j = 0; j < board.getSize(); ++j)
            {
                if((board.getPieceAt(i, j) == 0 || board.getPieceAt(i, j) == 3) && (newPosition.getPieceAt(i, j) == 1 || newPosition.getPieceAt(i, j) == 2))
                {
                    return new int[] {i , j};
                }
            }
        }

        throw new Exception("No new move found.");
    }

    void drawBoard()
    {
        for(int i = 0; i < SIZE; i++) 
        {
            for(int j = 0; j < SIZE; j++) 
            {
                Instantiate(square, new Vector2(i * squareWidth + xOffset, j * squareHeight + yOffset), Quaternion.identity);
            }
        }
    }
    
    void initPieces()
    {
        for(int i = 0; i < SIZE; i++) 
        {
            for(int j = 0; j < SIZE; j++) 
            {
                var newPiece = Instantiate(piece, new Vector2(i * squareWidth + xOffset, j * squareHeight + yOffset), Quaternion.identity);
                pieces[SIZE * (SIZE - j - 1) + i] = newPiece;
                newPiece.GetComponent<PieceScript>().setIndex(SIZE * (SIZE - j) + i);
            }
        }
    } 
    
    void setColors()
    {
        for(int i = 0; i < SIZE * SIZE; i++) 
        {
            pieces[i].GetComponent<PieceScript>().setColor(board.getPieceAt(i / 8, i % 8));
        }
    }

    public Board getBoard()
    {
        return board;
    }
}
