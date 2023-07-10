using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMoneyText : MonoBehaviour
{
    public Enjlectric.ScriptableData.Types.ScriptableDataInt coins;
    public Text text;

    // Start is called before the first frame update
    private void Start()
    {
        coins.OnValueChanged.AddListener(UpdateTex);
        UpdateTex();
    }

    private void OnDestroy()
    {
        coins.OnValueChanged.RemoveListener(UpdateTex);
    }

    // Update is called once per frame
    private void UpdateTex()
    {
        text.text = coins.Value.ToString("000000");
    }
}