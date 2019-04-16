using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterScript : MonoBehaviour
{
    public ScripObInt BoardWidth, BoardHeight;

    public BoardSetup BoardSetup;
    public TileHandler TileHandler;

    public void StartGame()
    {
        BoardSetup.SetupAtStart();
        TileHandler.SetupAtStart();
    }

    public void Newgame()
    {
        BoardSetup.NewBoard();
        TileHandler.SetupAtStart();
    }
}
