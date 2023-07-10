using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Game/Level/StageExit", fileName = "StageExit")]
public class StageExitObject : GameSection
{
    public Enjlectric.ScriptableData.Types.ScriptableDataInt Event;
    public int Type;

    internal override IEnumerator ExecutionRoutine()
    {
        Event.Value = Type;

        yield return new WaitForSeconds(duration + 3f);
    }
}