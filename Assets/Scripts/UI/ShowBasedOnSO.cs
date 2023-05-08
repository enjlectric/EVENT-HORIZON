using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBasedOnSO : MonoBehaviour
{
    public Enjlectric.ScriptableData.Concrete.ScriptableDataBool Value;
    // Start is called before the first frame update
    void Awake()
    {
        Value.OnValueChanged.AddListener(SetVisibility);
    }
    // Start is called before the first frame update
    void OnDestroy()
    {
        Value.OnValueChanged.RemoveListener(SetVisibility);
    }

    // Update is called once per frame
    void SetVisibility()
    {
        gameObject.SetActive(Value.Value);
    }
}
