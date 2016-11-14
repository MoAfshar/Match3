using UnityEngine;
using System.Collections;

public class MovablePiece : MonoBehaviour {

    //Adding a refrence to the gamePiece script since moving is directly connecting to a game piece
    private GamePiece piece;

    //get the refrence to the gamePiece, using awake to get this refrense ASAP
    void Awake()
    {
        piece = GetComponent<GamePiece>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    //This moves the piece to its new position
    public void Move(int newX, int newY)
    {
        //updating the coordinate
        piece.X = newX;
        piece.Y = newY;

        //getting the position for the gameObject, locally because piece is a child of the grid
        piece.transform.localPosition = piece.gridRef.getWorldPosition(newX, newY);
    }
}
