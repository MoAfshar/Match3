using UnityEngine;
using System.Collections;

public class ClearablePiece : MonoBehaviour {

    //hold the clear animation to play the animation
	public AnimationClip clearAnimation;

	private bool isBeingCleared = false;

	public bool IsBeingCleared {
		get { return isBeingCleared; }
	}

    //refrence to the gamePiece we're attached to, access properties of the piece and have refrence to the grid object
    //used protected becaue extending this class and derived classes need to have access to this variable
	protected GamePiece piece;

	void Awake() {
		piece = GetComponent<GamePiece> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Clear()
	{
		isBeingCleared = true;
		StartCoroutine (ClearCoroutine ());
	}

	private IEnumerator ClearCoroutine()
	{
		Animator animator = GetComponent<Animator> ();

		if (animator) {
			animator.Play (clearAnimation.name);

			yield return new WaitForSeconds (clearAnimation.length);

			Destroy (gameObject);
		}
	}
}
