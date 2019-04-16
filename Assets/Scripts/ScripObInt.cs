using UnityEngine;

[CreateAssetMenu(menuName = "ScripOb Int")]
public class ScripObInt : ScriptableObject
{
    public int value;

    public void SetValue(string SetValue)
    {
        value = int.Parse(SetValue);
    }

    public int GetValue()
    {
        return value;
    }

}