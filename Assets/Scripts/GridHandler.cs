using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    public TileHandler TileHandler;
    public ScripObBool CanInteract;

    private bool secondPress = false;
    private int firstX, firstY;
    private int secondX, secondY;

    public void GridPressed(int X, int Y)
    {
        if(CanInteract.GetValue() == false)
        {
            Debug.LogWarning("Can't interact yet!");
            return;
        }
        if(!secondPress)
        {
            firstX = X;
            firstY = Y;
            secondPress = true;
        }
        else
        {
            secondPress = false; 
            secondX = X;
            secondY = Y;
            int diffX;
            int diffY;
            diffX = firstX - secondX;
            diffY = firstY - secondY;
            switch (diffX)
            {
                case -1:
                    if(diffY != 0)
                    {
                        return;
                    }
                    TileHandler.StartHandling(firstX, firstY, secondX, secondY);
                    break;
                case 0:
                    switch (diffY)
                    {
                        case -1:
                            TileHandler.StartHandling(firstX, firstY, secondX, secondY);
                            break;

                        case 1:
                            TileHandler.StartHandling(firstX, firstY, secondX, secondY);
                            break;

                        default:
                            return;
                    }
                    break;
                case 1:
                    if (diffY != 0)
                    {
                        return;
                    }
                    TileHandler.StartHandling(firstX, firstY, secondX, secondY);
                    break;
                default:
                    return;
            }
        }
    }
}
