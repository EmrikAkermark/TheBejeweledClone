using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHandler : MonoBehaviour
{
    public IntArray IntAr;
    public GameObjectArray GobAr;
    public BoardSetup BoardSetup;
    public ScripObBool CanInteract;

    [SerializeField]
    private int minRowLength = 3;
    private int width, height;
    private int startID;
    private int storedFirstX, storedFirstY, storedSecondX, storedSecondY;
    private int firstID, secondID;
    private GameObject firstGob, secondGob;

    //Stores all coordinates that creates a row
    private List<int[]> clearableTiles = new List<int[]>();
    //Temporarily stores coordinates that creates one row
    private List<int[]> rowTiles = new List<int[]>();
    //Stores all gameobjects that will be deleted
    private List<GameObject> deletedGobs = new List<GameObject>();


    public void SetupAtStart()
    {
        width = IntAr.ints.GetLength(0);
        height = IntAr.ints.GetLength(1);
    }

    //Called from the grid
    public void SwitchPlaces(int firstX, int firstY, int secondX, int secondY)
    {
        firstID = IntAr.ints[firstX, firstY];
        firstGob = GobAr.Gob2D[firstX, firstY];
        secondID = IntAr.ints[secondX, secondY];
        secondGob = GobAr.Gob2D[secondX, secondY];

        IntAr.ints[firstX, firstY] = secondID;
        IntAr.ints[secondX, secondY] = firstID;

        GobAr.Gob2D[firstX, firstY] = secondGob;
        GobAr.Gob2D[secondX, secondY] = firstGob;

        Vector3 firstPos = firstGob.transform.position;
        Vector3 secondPos = secondGob.transform.position;

        firstGob.transform.position = secondPos;
        secondGob.transform.position = firstPos;
    }

    private void CheckRight(int X, int Y) {
        if (!(X >= width))
        {
            if (IntAr.ints[X, Y] == startID) {
                AddCoordinate(X, Y, 1);
                CheckRight(X + 1, Y);
            }
        }
    }

    private void CheckUp(int X, int Y)
    {
        if (!(Y >= height))
        {
            if (IntAr.ints[X, Y] == startID)
            {
                AddCoordinate(X, Y, 1);
                CheckUp(X, Y + 1);
            }
        }
    }

    private void AddCoordinate(int X, int Y, int list)
    {
        int[] coordinate = new int[2];
        coordinate[0] = X;
        coordinate[1] = Y;

        switch (list)
        {
            case 0:
                clearableTiles.Add(coordinate);
                break;
            case 1:
                rowTiles.Add(coordinate);
                break;   
        }
    }

    //Called from the grid.
    public void StartHandling(int firstX, int firstY, int secondX, int secondY)
    {
        storedFirstX = firstX;
        storedFirstY = firstY;
        storedSecondX = secondX;
        storedSecondY = secondY;
        SwitchPlaces(storedFirstX, storedFirstY, storedSecondX, storedSecondY);
        StartCoroutine(CheckIfLegalMove());
    }

    private IEnumerator CheckIfLegalMove()
    {
        CanInteract.SetValue(false);
        yield return new WaitForSeconds(1);
        CheckBoardForRows();
        if(clearableTiles.Count > 0)
        {
            yield return new WaitForSeconds(1);
            FillBlanks();
            yield return new WaitForSeconds(1);
            CreateTiles();
            clearableTiles.Clear();
            for (int i = 0; i < deletedGobs.Count; i++)
            {
                Destroy(deletedGobs[i]);
            }
            deletedGobs.Clear();
            yield return WorkTheBoard();
        }
        else
        {
            yield return new WaitForSeconds(1);
            SwitchPlaces(storedSecondX, storedSecondY, storedFirstX, storedFirstY);
            CanInteract.SetValue(true);
        }
    }

    private IEnumerator WorkTheBoard()
    {
        yield return new WaitForSeconds(1);
        CheckBoardForRows();
        if (clearableTiles.Count > 0)
        {
            yield return new WaitForSeconds(1);
            FillBlanks();
            yield return new WaitForSeconds(1);
            CreateTiles();
            clearableTiles.Clear();
            for (int i = 0; i < deletedGobs.Count; i++)
            {
                Destroy(deletedGobs[i]);
            }
            deletedGobs.Clear();
            yield return WorkTheBoard();
        }
        else
        {
            CanInteract.SetValue(true);
            yield return null;
        }
    }

    public void FillBlanks()
    {
        int steps = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //If a blank space is found...
                if (IntAr.ints[x, y] == 0)
                {
                    //Count the steps until a not blank space is found...
                    for (int y0 = y; y0 < height; y0++)
                    {
                        if (IntAr.ints[x, y0] != 0)
                        {
                            //And move all spaces from this point down the amount of steps that was counted
                            for (int y1 = y0; y1 < height; y1++)
                            {
                                IntAr.ints[x, y1 - steps] = IntAr.ints[x, y1];
                                IntAr.ints[x, y1] = 0;

                                if (GobAr.Gob2D[x, y1] != null)
                                {
                                    GobAr.Gob2D[x, y1].transform.position = new Vector3(2 * x - width, 2 * y1 - height - 2 * steps, 0);
                                }

                                GobAr.Gob2D[x, y1 - steps] = GobAr.Gob2D[x, y1];
                                GobAr.Gob2D[x, y1] = null;
                            }
                            //This takes us out of the "Collapse blank spaces" loop and back to the search for blanks loops
                            //Otherwise, the whole vertical row will be collapsed into the lowest blank space
                            break;
                        }
                        steps++;
                    }
                    //Resetting steps so they don't carry over to the next loop
                    steps = 0;
                }
            }
        }
    }

    private void CreateTiles()
    {
        BoardSetup.CreateNewRow();
    }

    //The two lists (rowList and clearList) are used together to easily find a single row (with the rowList) and then keeping track of the coordinates (clearList)
    private void CheckBoardForRows()
    {
        int skips = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if(skips > 0)
                {
                    skips--;
                    continue;
                }
                if(IntAr.ints[x, y] != 0)
                {
                    startID = IntAr.ints[x, y];
                    CheckRight(x, y);
                    skips = rowTiles.Count - 1;
                    if (rowTiles.Count >= minRowLength)
                    {
                        clearableTiles.AddRange(rowTiles);
                    }
                    rowTiles.Clear();
                }
            }
        }

        skips = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (skips > 0)
                {
                    skips--;
                    continue;
                }
                if (IntAr.ints[x, y] != 0)
                {
                    startID = IntAr.ints[x, y];
                    CheckUp(x, y);
                    skips = rowTiles.Count - 1;
                    if (rowTiles.Count >= minRowLength)
                    {
                        clearableTiles.AddRange(rowTiles);
                    }
                    rowTiles.Clear();
                }
            }
        }

        int[] coordinate;

        //First clearable spaces are marked on the board, so applying doubles won't matter
        for (int i = 0; i < clearableTiles.Count; i++)
        {
            coordinate = clearableTiles[i];
            IntAr.ints[coordinate[0], coordinate[1]] = 0;
        }
        //Then we can go through the board and visually clear the spaces. This is where you could hook up a points counter
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(IntAr.ints[x, y] == 0)
                {
                    GobAr.Gob2D[x, y].SetActive(false);
                    deletedGobs.Add(GobAr.Gob2D[x, y]);
                }
            }
        }
    }
}