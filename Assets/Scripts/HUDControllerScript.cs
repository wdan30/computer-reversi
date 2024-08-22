using UnityEngine;
using TMPro;

public class HUDControllerScript : MonoBehaviour
{
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text skipText;
    [SerializeField] private TMP_Text winText;

    private BoardScript boardScript;
    private int computerColour;

    // Start is called before the first frame update
    void Start()
    {
        computerColour = MenuControllerScript.getComputerColour();
        boardScript = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardScript>();
    }

    // Update is called once per frame
    void Update()
    {
        setTurnText(boardScript.getBoard().getCurrent());
    }

    public void setTurnText(int turn)
    {
        turnText.text = (turn == computerColour && MenuControllerScript.getMode() == 1) ? "Computer's turn" : "Your turn";
    }

    public void setSkipText(bool show)
    {
        if(show)
        {
            skipText.text = "No legal moves,\r\nskipping turn";
        }
        else
        {
            skipText.text = "";
        }
    }

    public void setWinText(int result)
    {
        if (result == 0)
        {
            winText.text = "Draw!";
        }
        else if (result == 1)
        {
            winText.text = "Black wins!";
        }
        else
        {
            winText.text = "White wins!";
        }
    }
}
