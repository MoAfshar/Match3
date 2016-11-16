using UnityEngine;
using System.Collections;

public class MovablePiece : MonoBehaviour {

    //Adding a refrence to the gamePiece script since moving is directly connecting to a game piece
	private GamePiece piece;
    //A variable to store a refrence to moveCouroutine
	private IEnumerator moveCoroutine;

    //get the refrence to the gamePiece, using awake to get this refrense ASAP
	void Awake() {
		piece = GetComponent<GamePiece> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //This moves the piece to its new position
	public void Move(int newX, int newY, float time)
	{
		if (moveCoroutine != null) {
			StopCoroutine (moveCoroutine);
		}

		moveCoroutine = MoveCoroutine (newX, newY, time);
		StartCoroutine (moveCoroutine);
	}

    //Interplate between starting and ending positions for the piece (Interploation means getting a value between two other values based on a parameter)
	public IEnumerator MoveCoroutine(int newX, int newY, float time)
	{
		piece.X = newX;
		piece.Y = newY;

		Vector3 startPos = transform.position;
		Vector3 endPos = piece.GridRef.GetWorldPosition (newX, newY);

		for (float t = 0; t <= 1 * time; t += Time.deltaTime) {
			piece.transform.position = Vector3.Lerp (startPos, endPos, t / time);
			yield return 0;
		}

		piece.transform.position = piece.GridRef.GetWorldPosition (newX, newY);
	}
}
