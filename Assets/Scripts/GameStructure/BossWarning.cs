using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level/BossWarn", fileName = "BossWarn")]
public class BossWarning : GameSection
{
    [TextArea] public string EnglishString;
    [TextArea] public string JapaneseString;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataString StringValue;

    internal override IEnumerator ExecutionRoutine()
    {
        StringValue.Value = EnglishString;

        if (EnglishString[0] == '!')
        {
            AudioManager.ChangeMusic(null, 2);
        }
        yield return new WaitForSeconds(2.5f);

        if (EnglishString[0] == '!')
        {
            AudioManager.ChangeMusic(BGM.BossIntro, BGM.BossLoop);
        }

        yield return new WaitForSeconds(duration);
    }
}
