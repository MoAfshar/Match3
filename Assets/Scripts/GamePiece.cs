﻿using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {
    private int x;
    private int y;

    //Creating a public property to access that variable, only a 'get' since not all pieces are allowed to move, so they shouldn't change
    public int X
    {
        get { return x; }
        set
        {
            if(isMovable())
            {
                x = value;
            }
        }
    }

    public int Y
    {
        get { return y; }
        set
        {
            if (isMovable())
            {
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

    public Grid gridRef
    {
        get { return grid; }
    }

    private MovablePiece movableComponent;

    public MovablePiece MovableComponent
    {
        get { return movableComponent; }
    }

    void Awake()
    {
        movableComponent = GetComponent<MovablePiece>();
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

    public bool isMovable()
    {
        return movableComponent != null;
    }
}
