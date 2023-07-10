using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Game/Level/StageIntro", fileName = "StageIntro")]
public class StageIntroObject : GameSection
{
    public Enjlectric.ScriptableData.Types.ScriptableDataInt Event;
    public int Type;

    internal override IEnumerator ExecutionRoutine()
    {
        Event.Value = Type;

        Manager.instance.thisLevelCombo = 0;
        Manager.instance.currentCombo = 0;
        Manager.instance.thisLevelHits = 0;
        Manager.instance.thisLevelKills = 0;
        yield return new WaitForSeconds(duration);
    }
}