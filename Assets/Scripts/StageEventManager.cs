using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageEventManager : MonoBehaviour
{
    public Enjlectric.ScriptableData.Concrete.ScriptableDataInt StageProgress;

    public List<UnityEvent> Events = new List<UnityEvent>();

    void Awake()
    {
        StageProgress.SetValueWithoutNotify(0);
        StageProgress.OnValueChanged.AddListener(InvokeEvents);
    }

    private void OnDestroy()
    {
        StageProgress.OnValueChanged.RemoveListener(InvokeEvents);
    }

    private void InvokeEvents()
    {
        if (Events.Count >= StageProgress.Value && StageProgress.Value > 0)
        {
            Events[StageProgress.Value - 1].Invoke();
        }
    }
}
