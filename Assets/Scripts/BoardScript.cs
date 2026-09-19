using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardScript : MonoBehaviour
{
    private readonly int SIZE = 8;   
    private Board board;
    private GameObject[] pieces;
    private Camera cam;
    private bool isOver = false;
    private bool isCoroutineRunning = false;
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
        if(isOver || isCoroutineRunning)
        {
            return;
        }

        StartCoroutine(doMove());
    }

    IEnumerator doMove()
    {
        isCoroutineRunning = true;
        if(!isOver && board.getLegalMoves().Count > 0)
        {
            skips = 0;
            yield return StartCoroutine(doTurn());
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
            StartCoroutine(ResetGameAfterDelay(3.0f));
        }

        isCoroutineRunning = false;
        yield return null;
    }

    IEnumerator ResetGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    IEnumerator doTurn()
    {
        if(mode == 0 || board.getCurrent() != computerColour)
        {
            bool moveMade = false;
            while(!moveMade)
            {
                // Ensure we only process clicks if it is still the player's turn
                if(board.getCurrent() != computerColour && Input.GetMouseButtonDown(0))
                {
                    Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
                    int x = (int) Math.Floor(pos.y) * -1 + 3;
                    int y = (int) Math.Floor(pos.x) + 4;
                    
                    // Validate move exists before making it to prevent exceptions
                    bool isValidMove = false;
                    foreach(int[] move in board.getLegalMoves())
                    {
                        if(move[0] == x && move[1] == y)
                        {
                            isValidMove = true;
                            break;
                        }
                    }

                    if(isValidMove)
                    {
                        prevMove = new int[] {x, y};

                        if(board.getPieceAt(x, y) == 3)
                        {
                            hudController.setTurnText(computerColour);
                        }

                        board.makeMove(x, y);
                        moveMade = true;
                    }
                }

                yield return null;
            }
        }
        else
        {
            yield return StartCoroutine(computerThink());
            board.makeMove(prevMove);
        }   

        yield return null;
    }

    IEnumerator computerThink()
    {
        TreeSearch.setRoot(new Node(board));
        
        // Run iterations over multiple frames to avoid blocking the main thread
        int totalIterations = 2000; // Total MCTS iterations
        int iterationsPerFrame = TreeSearch.GetIterationsPerFrame();
        
        for (int i = 0; i < totalIterations; i += iterationsPerFrame)
        {
            TreeSearch.analyzeIterative(iterationsPerFrame);
            yield return null; // Wait for next frame
        }

        // Get the result after iterations are done
        // Note: TreeSearch needs a method to get the result node now
        prevMove = findMovePlayed(TreeSearch.getBestNode().getPosition());
        yield return null;
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
