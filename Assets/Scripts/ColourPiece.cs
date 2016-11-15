using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColourPiece : MonoBehaviour
{

    //All the possible colours that a piece could be
    public enum ColourType
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANY,
        COUNT
    };

    //Assign a sprite to a colour
    [System.Serializable]
    public struct colourSprite
    {
        public ColourType colour;
        public Sprite sprite;
    };

    public colourSprite[] colourSprites;

    private ColourType colour;

    //colour of the piece, to allow us easily compare it to other pieces for a colour match
    public ColourType Colour
    {
        get { return colour; }
        set { SetColour(value); }
    }

    public int NumColours
    {
        get { return colourSprites.Length; }
    }

    private SpriteRenderer sprite;
    private Dictionary<ColourType, Sprite> colourSpriteDict;

    void Awake()
    {
        sprite = transform.Find("piece").GetComponent<SpriteRenderer>();

        colourSpriteDict = new Dictionary<ColourType, Sprite>();

        //mapping colours to sprites
        for (int i = 0; i < colourSprites.Length; i++)
        {
            if (!colourSpriteDict.ContainsKey(colourSprites[i].colour))
            {
                colourSpriteDict.Add(colourSprites[i].colour, colourSprites[i].sprite);
            }
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetColour(ColourType newColour)
    {
        colour = newColour;
        if (colourSpriteDict.ContainsKey(newColour))
        {
            sprite.sprite = colourSpriteDict[newColour];
        }
    }
}
