﻿using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {

	private int x;
	private int y;

    //Creating a public property to access that variable, only a 'get' since not all pieces are allowed to move, so they shouldn't change
	public int X
	{
		get { return x; }
		set {
			if (IsMovable ()) {
				x = value;
			}
		}
	}

	public int Y
	{
		get { return y; }
		set {
			if (IsMovable ()) {
				y = value;
			}
		}
	}

	private Grid.PieceType type;

	public Grid.PieceType Type
	{
		get { return type; }
	}

	private Grid grid;

	public Grid GridRef
	{
		get { return grid; }
	}

	private MovablePiece movableComponent;

	public MovablePiece MovableComponent
	{
		get { return movableComponent; }
	}

	private ColorPiece colorComponent;

	public ColorPiece ColorComponent
	{
		get { return colorComponent; }
	}

	private ClearablePiece clearableComponent;

	public ClearablePiece ClearableComponent {
		get { return clearableComponent; }
	}

	void Awake()
	{
		movableComponent = GetComponent<MovablePiece> ();
		colorComponent = GetComponent<ColorPiece> ();
		clearableComponent = GetComponent<ClearablePiece> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //to be able to initialise some of these variables
	public void Init(int _x, int _y, Grid _grid, Grid.PieceType _type)
	{
		x = _x;
		y = _y;
		grid = _grid;
		type = _type;
	}

    //When mouse enters an element
	void OnMouseEnter()
	{
        grid.EnterPiece(this); 
	}

    //Mouse is pressed inside an element (For VR when it is touched)
	void OnMouseDown()
	{
        grid.PressPiece(this); //"this" is a refrence to this gamepiece component.
	}

    //When mouse is hovered (For VR it would be when touch is released)
	void OnMouseUp()
	{
		grid.ReleasePiece ();
	}

	public bool IsMovable()
	{
		return movableComponent != null;
	}

    //Check if a piece coloured or not
	public bool IsColored()
	{
		return colorComponent != null;
	}

	public bool IsClearable()
	{
		return clearableComponent != null;
	}
}
