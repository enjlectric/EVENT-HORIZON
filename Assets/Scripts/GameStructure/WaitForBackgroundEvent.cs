using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level/WaitForBackgroundEvent", fileName = "WaitForBackgroundEvent")]
public class WaitForBackgroundEvent : GameSection
{
    public Enjlectric.ScriptableData.Types.ScriptableDataInt ProgressValue;

    internal override IEnumerator ExecutionRoutine()
    {
        var progressValue = ProgressValue.Value;

        while (progressValue == ProgressValue.Value)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }
}