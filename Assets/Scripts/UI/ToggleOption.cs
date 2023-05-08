using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleOption : MonoBehaviour
{
    public Enjlectric.ScriptableData.Concrete.ScriptableDataBool SaveGameValue;

    public Toggle ToggleA;

    public void OnEnable()
    {
        ToggleA.SetIsOnWithoutNotify(SaveGameValue.Value);
    }

    public void UpdateValue(bool newValue)
    {
        SaveGameValue.Value = newValue;
        //SFX.UIButtonConfirm.Play();
    }
}
