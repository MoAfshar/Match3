using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorPiece : MonoBehaviour {

    //All the possible colours that a piece could be
	public enum ColorType
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
	public struct ColorSprite
	{
		public ColorType color;
		public Sprite sprite;
	};

	public ColorSprite[] colorSprites;

	private ColorType color;

    //colour of the piece, to allow us easily compare it to other pieces for a colour match
	public ColorType Color
	{
		get { return color; }
		set { SetColor (value); }
	}

	public int NumColors
	{
		get { return colorSprites.Length; }
	}

	private SpriteRenderer sprite;
	private Dictionary<ColorType, Sprite> colorSpriteDict;

	void Awake()
	{
		sprite = transform.Find ("piece").GetComponent<SpriteRenderer> ();

		colorSpriteDict = new Dictionary<ColorType, Sprite> ();

        //mapping colours to sprites
		for (int i = 0; i < colorSprites.Length; i++) {
			if (!colorSpriteDict.ContainsKey (colorSprites [i].color)) {
				colorSpriteDict.Add (colorSprites [i].color, colorSprites [i].sprite);
			}
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetColor(ColorType newColor)
	{
		color = newColor;

		if (colorSpriteDict.ContainsKey (newColor)) {
			sprite.sprite = colorSpriteDict [newColor];
		}
	}
}
