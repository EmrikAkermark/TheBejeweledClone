using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameObject Array")]

public class GameObjectArray : ScriptableObject
{
    public GameObject[] GameObjects;
    public GameObject[,] Gob2D;
}