using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMoneyText : MonoBehaviour
{
    public Enjlectric.ScriptableData.Concrete.ScriptableDataInt coins;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        coins.OnValueChanged.AddListener(UpdateTex);
        UpdateTex();
    }
    void OnDestroy()
    {
        coins.OnValueChanged.RemoveListener(UpdateTex);
    }

    // Update is called once per frame
    void UpdateTex()
    {
        text.text = coins.Value.ToString("000000");
    }
}
