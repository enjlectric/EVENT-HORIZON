using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvanceEvent : MonoBehaviour
{
    public Enjlectric.ScriptableData.Types.ScriptableDataInt StageEvent;

    public void Continue()
    {
        if (StageEvent.Value == 0)
        {
            StageEvent.Value++;
        }
    }
}