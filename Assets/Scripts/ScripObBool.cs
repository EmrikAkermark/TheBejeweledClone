using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Bool")]
public class ScripObBool : ScriptableObject
{
    private bool startValue = true;

    private bool value;

    private void OnEnable()
    {
        value = startValue;
    }

    public bool GetValue()
    {
        return value;
    }

    public void SetValue(bool SetValue)
    {
        value = SetValue;
    }
}