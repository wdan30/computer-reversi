using System;
using UnityEngine;

public class PieceScript : MonoBehaviour
{
    [SerializeField] private int row;
    [SerializeField] private int col;
    [SerializeField] private int index;
    private int color; 

    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = gameObject.transform.position;

        row = (int)Math.Round(pos.x - 0.5 + 4);
        col = (int)Math.Round(pos.y - 0.5 + 4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private Color colorMap(int color) => color switch
    {
        1 => new Color(0.2f, 0.2f, 0.2f),
        2 => Color.white,
        3 => new Color(0.2f, 0.2f, 0f, 0.5f),
        _ => Color.clear,          
    };

    public void setColor(int color)
    {
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = colorMap(color);
    }

    public void incrColor()
    {
        color = (color + 1) % 3;
    }

    public void setIndex(int num)
    {
        index = num;
    }
}
