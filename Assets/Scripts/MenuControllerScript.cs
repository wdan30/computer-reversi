using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuControllerScript : MonoBehaviour
{
    [SerializeField] private TMP_Text modeButtonText;
    [SerializeField] private TMP_Text colourButtonText;
    [SerializeField] private TMP_Text startButtonText;
    [SerializeField] private Button colourButton;
    [SerializeField] private string sceneName = "BoardScene";

    private static int mode = 1;
    private static int computerColour = 1;

    public void ChangeMode()
    {
        mode = (mode + 1) % 2;

        if(mode == 1)
        {
            modeButtonText.text = " One player";
            colourButton.enabled = true;
        }
        else
        {
            modeButtonText.text = " Two player";
            colourButtonText.text = "Player goes first";

            colourButton.enabled = false;
            computerColour = 2;
        }
    }

    public void ChangeColour()
    {
        computerColour = computerColour % 2 + 1;

        if (computerColour == 2)
        {
            colourButtonText.text = "Player goes first";
        }
        else
        {
            colourButtonText.text = "Computer goes first";
        }
    }
    
    public void StartGame()
    {
        startButtonText.text = "Loading...";
        SceneManager.LoadScene(sceneName);
    }
    public static int getMode()
    {
        return mode;
    }

    public static int getComputerColour()
    {
        return computerColour;
    }
}
