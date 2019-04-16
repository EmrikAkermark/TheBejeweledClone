using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoxBehavior : MonoBehaviour
{
    private GridHandler gridHandler;

    private int x, y;

    //Used at start
    public void Setup(int X, int Y, GridHandler handler)
    {
        x = X;
        y = Y;
        gridHandler = handler;
    }

    private void OnMouseDown()
    {
        gridHandler.GridPressed(x, y);
    }
}