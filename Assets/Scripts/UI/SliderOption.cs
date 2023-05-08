using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderOption : MonoBehaviour
{
    public Enjlectric.ScriptableData.Concrete.ScriptableDataFloat SaveGameValue;

    public Slider SliderA;

    public void OnEnable()
    {
        SliderA.SetValueWithoutNotify(SaveGameValue.Value);
    }

    public void UpdateValue(float newValue)
    {
        SaveGameValue.Value = newValue;
        SFX.UI_Move.Play();
        //SFX.UIButtonConfirm.Play();
    }
}
