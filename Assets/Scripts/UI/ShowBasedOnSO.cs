using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBasedOnSO : MonoBehaviour
{
    public Enjlectric.ScriptableData.Types.ScriptableDataBool Value;

    // Start is called before the first frame update
    private void Awake()
    {
        Value.OnValueChanged.AddListener(SetVisibility);
    }

    // Start is called before the first frame update
    private void OnDestroy()
    {
        Value.OnValueChanged.RemoveListener(SetVisibility);
    }

    // Update is called once per frame
    private void SetVisibility()
    {
        gameObject.SetActive(Value.Value);
    }
}