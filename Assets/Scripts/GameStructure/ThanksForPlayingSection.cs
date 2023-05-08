using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level/End", fileName = "End")]
public class ThanksForPlayingSection : GameSection
{
    internal override IEnumerator ExecutionRoutine()
    {
        UIManager.instance.thanksForPlayingImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        UIManager.instance.thanksForPlayingImage.gameObject.SetActive(false);

        Manager.instance.EndRun();
    }
}
