using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level/BackgroundEvent", fileName = "BackgroundEvent")]
public class BackgroundEvent : GameSection
{
    public int SetProgressValue = -1;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataInt ProgressValue;

    internal override IEnumerator ExecutionRoutine()
    {
        ProgressValue.Value = SetProgressValue == -1 ? ProgressValue.Value + 1 : SetProgressValue;
        yield return new WaitForSeconds(duration);
    }
}
