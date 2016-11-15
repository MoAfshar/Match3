using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

    public enum PieceType
    {
        EMPTY,
        NORMAL,
        BUBBLE,
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

    //time between calls to FillStep
    public float fillTime;

    //have an array of struct which can be edited in the inspector
    public PiecePrefab[] piecePrefabs;
    public GameObject backgroundPrefab;

    //Associate each piecetype with a prefab but dictionaries cannot be displayed in the inspector so create a struct
    private Dictionary<PieceType, GameObject> piecePrefableDict;

    //2D array of game objects
    private GamePiece[,] pieces;

    private bool inverse = false;

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
                //Start empty
                SpawnNewPiece(x, y, PieceType.EMPTY);
            }
        }

        Destroy(pieces[4, 4].gameObject);
        SpawnNewPiece(4, 4, PieceType.BUBBLE);

        Destroy(pieces[1, 4].gameObject);
        SpawnNewPiece(1, 4, PieceType.BUBBLE);

        Destroy(pieces[5, 4].gameObject);
        SpawnNewPiece(5, 4, PieceType.BUBBLE);

        Destroy(pieces[8, 4].gameObject);
        SpawnNewPiece(8, 4, PieceType.BUBBLE);

        StartCoroutine(Fill());
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

    //Spawn a new piece
    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = (GameObject)Instantiate(piecePrefableDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newPiece.transform.parent = transform;

        //Store the game piece component in our pieces array
        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        pieces[x, y].Init(x, y, this, type);

        return pieces[x, y];
    }

    //Only move each piece by one space, true if any pieces were moved and false if not
    public bool FillStep()
    {
        bool movedPiece = false;

        //-2 since we don't care about the bottom row since they cant be moved down, remember 0 is at the top
        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                //if not inverted
                int x = loopX;
                //if inverted
                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }
                //Get the game piece at the current position and check if it's movable
                GamePiece piece = pieces[x, y];

                if (piece.IsMovable())
                {
                    GamePiece pieceBelow = pieces[x, y + 1];

                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        //Swapping a movable piece with an empty piece
                        piece.MovableComponent.Move(x, y + 1, fillTime);
                        pieces[x, y + 1] = piece;
                        //Have to now make the piece above it empty since it moved down
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else
                    {
                        //move Diagnally
                        for(int diag = -1; diag <= 1; diag++)
                        {
                            if(diag != 0)
                            {
                                //x coordinate of our diagonal piece
                                int diagX = x + diag;

                                if (inverse)
                                {
                                    diagX = x - diag;
                                }

                                if(diagX >= 0 && diagX < xDim)
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];

                                    if(diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;

                                        for(int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];
                                            if (pieceAbove.IsMovable())
                                            {
                                                break;
                                            }
                                            else if (!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }

                                        if (!hasPieceAbove)
                                        {
                                            Destroy(diagonalPiece.gameObject); ;
                                            piece.MovableComponent.Move(diagX, y + 1, fillTime);
                                            pieces[diagX, y + 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //Top row is a special case, 
        for (int x = 0; x < xDim; x++)
        {
            GamePiece pieceBelow = pieces[x, 0];

            if (pieceBelow.Type == PieceType.EMPTY)
            {
                //Destroy the piece below before moving into it
                Destroy(pieceBelow.gameObject);
                //The new piece in the -1 row, (the row above the top row)
                GameObject newPiece = (GameObject)Instantiate(piecePrefableDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newPiece.transform.parent = transform;

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
                pieces[x, 0].ColourComponent.SetColour((ColourPiece.ColourType)Random.Range(0, pieces[x, 0].ColourComponent.NumColours));
                movedPiece = true;
            }
        }

        return movedPiece;
    }

        //Call this function until all the board is filled
        public IEnumerator Fill()
    {
        while (FillStep())
        {
            inverse = !inverse;
            yield return new WaitForSeconds(fillTime);
        }
    }

    }

