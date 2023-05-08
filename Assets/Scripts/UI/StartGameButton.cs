using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartGameButton : MonoBehaviour
{

    public Enjlectric.ScriptableData.Concrete.ScriptableDataFloat VignetteOpacity;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataTexture2D VignetteTexture;

    public Transform LogoTransform;

    public Texture2D TransitionTexture;
    public void Click()
    {
        CoroutineManager.Start(StartGame());
    }

    IEnumerator StartGame()
    {
        SFX.StartGame.Play();
        AudioManager.ChangeMusic(null, 999);
        VignetteTexture.Value = TransitionTexture;

        EventSystem.current.enabled = false;
        LogoTransform.DOLocalMoveY(2, 1.35f).SetEase(Ease.InBack);
        LogoTransform.DOScale(0, 1.35f).SetEase(Ease.InBack);
        yield return new WaitForSeconds(1.0f);
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 0.75f;
            VignetteOpacity.Value = t;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        Manager.instance.StartRun();
    }
}
