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

    //What piece we click on
    private GamePiece pressedPiece;
    private GamePiece enteredPiece;

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
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                //x coordinate of our diagonal piece
                                int diagX = x + diag;

                                if (inverse)
                                {
                                    diagX = x - diag;
                                }

                                if (diagX >= 0 && diagX < xDim)
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];

                                    if (diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;

                                        for (int aboveY = y; aboveY >= 0; aboveY--)
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

    //To swap two pieces we have to check if they are adjecent
    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        //if piece1 and piece2 have the same x coordinate and their y coordinate is of one space of each other
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1) || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    //Swap the two pieces if they are adjacent
    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if(piece1.IsMovable() && piece2.IsMovable())
        {
            //assign to each others positions
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;

            //check if either swapped pieces create a match
            if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null)
            {
                //move the pieces from one space of the grid to the other
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);
            }
            else
            {
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
            }
         
        }
    }

    public void PressPiece(GamePiece piece)
    {
        pressedPiece = piece;
    }

    //piece that we are hovering over
    public void EnterPiece(GamePiece piece)
    {
        enteredPiece = piece;
    }

    //when released the mouse
    public void ReleasePiece()
    {
        if(IsAdjacent(pressedPiece, enteredPiece))
        {
            SwapPieces(pressedPiece, enteredPiece);
        }
    }

    //takes in a gamePiece and the new position it is moving to, return a list of gamePieces that are part of a match
    //A match is 3 or more colours that match in a row or in a L or T shape
    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        if (piece.IsColoured())
        {
            ColourPiece.ColourType colour = piece.ColourComponent.Colour;
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            //traverse horrizontally and then vertically, each adjacent piece we find that is the same colour will get added to the array of the potential pieces
            //we will stop when we reach a different colour, if we have enough pieces to form a match they will get added to the matchingPieces list 

            //First check horizontal
            horizontalPieces.Add(piece);

            //if dir = 0 we go one way, if dir = 1 we go the other way
            for(int dir = 0; dir <= 1; dir++)
            {
                //xOffset is how far away our adjacent piece is from our central piece
                for(int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x; 
                    if(dir == 0) //going left
                    {
                        x = newX - xOffset;
                    }
                    else //going right
                    {
                        x = newX + xOffset;
                    }

                    if(x < 0 || x >= xDim) //if it goes out of bounds
                    {
                        break;
                    }

                    //we check if the piece is coloured and if the piece is the same colour as our current piece
                    //y coordinate is the same and the x coordinate is the one we just calculated
                    if (pieces[x, newY].IsColoured() && pieces[x, newY].ColourComponent.Colour == colour)
                    {
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //Traverse vertically if we found a match (for L and T shape)
            if(horizontalPieces.Count >= 3)
            {
                for(int i = 0; i < horizontalPieces.Count; i++)
                {
                    for(int dir = 0; dir <= 1; dir++)
                    {
                        for(int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;
                            if(dir == 0) //UP
                            {
                                y = newY - yOffset;
                            } else //DOWN
                            {
                                y = newY + yOffset;
                            }
                            
                            if(y < 0 || y >= yDim)
                            {
                                break;
                            }

                            if(pieces[horizontalPieces[i].X, y].IsColoured() && pieces[horizontalPieces[i].X, y].ColourComponent.Colour == colour)
                            {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if(verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else
                    {
                        for(int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }

                        break;
                    }
                }
            }

            if(horizontalPieces.Count >= 3)
            {
                for(int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }

            if(matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

            //Didn't find anything horizontally first, 
            //so now check vertically
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            //if dir = 0 we go one way, if dir = 1 we go the other way
            for (int dir = 0; dir <= 1; dir++)
            {
                //yOffset is how far away our adjacent piece is from our central piece
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;
                    if (dir == 0) //going UP
                    {
                        y = newY - yOffset;
                    }
                    else //going DOWN
                    {
                        y = newY + yOffset;
                    }

                    if (y < 0 || y >= yDim) //if it goes out of bounds
                    {
                        break;
                    }

                    //we check if the piece is coloured and if the piece is the same colour as our current piece
                    //x coordinate is the same and the y coordinate is the one we just calculated
                    if (pieces[newX, y].IsColoured() && pieces[newX, y].ColourComponent.Colour == colour)
                    {
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }

            //Traverse horizontally if we found a match (for L and T shape)
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < yDim; xOffset++)
                        {
                            int x;
                            if (dir == 0) //LEFT
                            {
                                x = newX - xOffset;
                            }
                            else //RIGHT
                            {
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= xDim)
                            {
                                break;
                            }

                            if (pieces[x, verticalPieces[i].Y].IsColoured() && pieces[x, verticalPieces[i].Y].ColourComponent.Colour == colour)
                            {
                                verticalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }

                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

        }

        return null;
    }
}

