using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level/BossWarn", fileName = "BossWarn")]
public class BossWarning : GameSection
{
    [TextArea] public string EnglishString;
    [TextArea] public string JapaneseString;
    public Enjlectric.ScriptableData.Types.ScriptableDataString StringValue;
    public bool TurnOffMusic = true;
    public bool PlayBossMusic = true;

    internal override IEnumerator ExecutionRoutine()
    {
        StringValue.Value = EnglishString;

        if (TurnOffMusic)
        {
            AudioManager.ChangeMusic(null, 2);
        }
        yield return new WaitForSeconds(2.5f);

        if (PlayBossMusic)
        {
            AudioManager.ChangeMusic(BGM.BossIntro, BGM.BossLoop);
        }

        yield return new WaitForSeconds(duration);
    }
}