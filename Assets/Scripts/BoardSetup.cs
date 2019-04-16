using UnityEngine;

public class BoardSetup : MonoBehaviour
{
    public IntArray IntAr;
    public GameObjectArray GobAr;
    public GameObject GridBox;
    public GridHandler GridHandler;
    public GameObjectArray GridGobAr;

    public ScripObInt Width, Height;

    [SerializeField]
    private int gridX, gridY;

    private int randomNumber, firstNumber = 0, secondNumber = 0;


    public void SetupAtStart()
    {
        SetupBoard();
        SpawnGameObjects();
    }

    public void NewBoard()
    {
        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                Destroy(GobAr.Gob2D[x, y]);
                Destroy(GridGobAr.Gob2D[x, y]);
            }
        }
        SetupBoard();
        SpawnGameObjects();
    }

    //Separated to easily hook up custom boards for testing;
    void SetupBoard()
    {
        gridX = Width.GetValue();
        gridY = Height.GetValue();
        IntAr.ints = new int[gridX, gridY];
        GobAr.Gob2D = new GameObject[gridX, gridY];
        GridGobAr.Gob2D = new GameObject[gridX, gridY];
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                GetRandomNumber(false);
                IntAr.ints[x, y] = randomNumber;
            }
            firstNumber = 0;
            secondNumber = 0;
        }
        for (int y = 2; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                GetRandomNumber(true, x, y);
                IntAr.ints[x, y] = randomNumber;
            }
            firstNumber = 0;
            secondNumber = 0;
        }
    }
    private void SpawnGameObjects()
    {
        for (int y = 0; y < gridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                int v = IntAr.ints[x, y];
                GobAr.Gob2D[x, y] = Instantiate(GobAr.GameObjects[v - 1], new Vector3(2 * x - gridX, 2 * y - gridY, 0), transform.rotation);
                GridGobAr.Gob2D[x, y] = Instantiate(GridBox, new Vector3(2 * x - gridX, 2 * y - gridY, 0), transform.rotation);
                GridBoxBehavior grid = GridGobAr.Gob2D[x, y].GetComponent<GridBoxBehavior>();
                grid.Setup(x, y, GridHandler);

            }
        }
    }

    private void GetRandomNumber(bool checkBelow, int x = 0, int y = 0)
    {
        randomNumber = Random.Range(1, GobAr.GameObjects.Length + 1);
        if (!checkBelow)
        {
            if (randomNumber == firstNumber && randomNumber == secondNumber)
            {
                GetRandomNumber(checkBelow);
            }
            else
            {
                secondNumber = firstNumber;
                firstNumber = randomNumber;
            }
        }
        else
        {
            if (randomNumber == firstNumber && randomNumber == secondNumber || randomNumber == IntAr.ints[x, y - 1] && randomNumber == IntAr.ints[x, y - 2])
            {
                GetRandomNumber(checkBelow, x, y);
            }
            else
            {
                secondNumber = firstNumber;
                firstNumber = randomNumber;
            }
        }
    }

    public void CreateNewRow()
    {
        firstNumber = 0;
        secondNumber = 0;
        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                if (IntAr.ints[x, y] == 0)
                {
                    GetRandomNumber(false);
                    IntAr.ints[x, y] = randomNumber;
                    GobAr.Gob2D[x, y] = Instantiate(GobAr.GameObjects[randomNumber - 1], new Vector3(2 * x - gridX, 2 * y - gridY, 0), transform.rotation);
                }
            }
        }
    }
}
