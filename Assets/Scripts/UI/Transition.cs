using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    public Enjlectric.ScriptableData.Types.ScriptableDataFloat TransitionOpacity;
    public Enjlectric.ScriptableData.Types.ScriptableDataTexture2D TransitionTexture;
    public Image TransitionObj;

    private float value = 0;

    private void Start()
    {
        value = TransitionOpacity.Value;
        TransitionObj.material.SetFloat("_Cutoff", value);
        TransitionObj.material.SetTexture("_TransitionTex", TransitionTexture.Value);
    }

    private void Awake()
    {
        TransitionTexture.OnValueChanged.AddListener(SetTexture);
    }

    private void OnDestroy()
    {
        TransitionTexture.OnValueChanged.RemoveListener(SetTexture);
    }

    private void SetTexture()
    {
        TransitionObj.material.SetTexture("_TransitionTex", TransitionTexture.Value);
    }

    // Update is called once per frame
    private void Update()
    {
        if (TransitionOpacity.Value > value)
        {
            value += Time.deltaTime * 4;
            value = Mathf.Min(value, TransitionOpacity.Value);
            TransitionObj.material.SetFloat("_Cutoff", value);
        }
        else if (TransitionOpacity.Value < value)
        {
            value -= Time.deltaTime * 4;
            value = Mathf.Max(value, TransitionOpacity.Value);
            TransitionObj.material.SetFloat("_Cutoff", value);
        }
    }
}