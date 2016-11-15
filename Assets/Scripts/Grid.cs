using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

    public enum PieceType
    {
        NORMAL,
        //how many piecetypes there are
        COUNT,
    };

    [System.Serializable] //flag so that our custom struct shows in the inspector 
    public struct PiecePrefab
    {
        public PieceType type; //Our key
        public GameObject prefab; //Our value
    };

    //x & y dimentions of our grid
    public int xDim;
    public int yDim;

    //have an array of struct which can be edited in the inspector
    public PiecePrefab[] piecePrefabs;
    public GameObject backgroundPrefab;

    //Associate each piecetype with a prefab but dictionaries cannot be displayed in the inspector so create a struct
    private Dictionary<PieceType, GameObject> piecePrefableDict;

    //2D array of game objects
    private GamePiece[,] pieces;

    // Use this for initialization
    void Start()
    {
        piecePrefableDict = new Dictionary<PieceType, GameObject>();

        //Copy the values from our array into our dictionary 
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecePrefableDict.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefableDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }

        //add background tiles for more visibility
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                background.transform.parent = transform;
            }
        }

        pieces = new GamePiece[xDim, yDim];
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject newPiece = (GameObject)Instantiate(piecePrefableDict[PieceType.NORMAL], Vector3.zero, Quaternion.identity);
                newPiece.name = "Piece(" + x + "," + y + ")";
                newPiece.transform.parent = transform;

                //storing the game piece component of the new piece game object in our pieces array
                pieces[x, y] = newPiece.GetComponent<GamePiece>();
                pieces[x, y].Init(x, y, this, PieceType.NORMAL);

                if (pieces[x, y].IsMovable())
                {
                    pieces[x, y].MovableComponent.Move(x, y);
                }

                if (pieces[x, y].IsColoured())
                {
                    //Set it to a random colour
                    pieces[x, y].ColourComponent.SetColour((ColourPiece.ColourType)Random.Range(0, pieces[x, y].ColourComponent.NumColours));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Centre the grid, convert grid coordinate to a world position
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim / 2.0f + x, transform.position.y + yDim / 2.0f - y);
    }
}
