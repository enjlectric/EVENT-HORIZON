using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

public class Stage1LongIntro : SerializedMonoBehaviour
{
    public Enjlectric.ScriptableData.Concrete.ScriptableDataString Language;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataTexture2D TransitionTexture;
    public Texture2D TransitionTexture2D;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataFloat VignetteValue;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataInt WhiteFlashValue;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataInt BackgroundProgress;

    private Coroutine cutscene;

    public List<GameObject> HideWhenFlashing = new List<GameObject>();

    public List<Text> texts = new List<Text>();

    private Vector3 playerOffset = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        VignetteValue.Value = 1;
        Language.Value = "en";
        cutscene = CoroutineManager.Start(Cutscene());
        Manager.instance.player.InputOverride = true;
    }

    private void LateUpdate()
    {
        Manager.instance.player.transform.position = MaskManager.GetPositionRelativeToCam(-1.15f, 0) + playerOffset;
    }
    public void SkipIntro()
    {
        if (cutscene != null)
        {
            CoroutineManager.Abort(cutscene);
            cutscene = null;
            playerOffset.x = 6;
            playerOffset.y = Mathf.Sin(2) * -2;
            WhiteFlashValue.Value = 3;
            Manager.instance.player.InputOverride = false;
            VignetteValue.Value = 0;
            BackgroundProgress.Value++;
            Manager.instance.player.transform.position = MaskManager.GetPositionRelativeToCam(-1.15f, 0) + playerOffset;
            foreach (var e in HideWhenFlashing)
            {
                e.gameObject.SetActive(false);
            }

            AudioManager.ChangeMusic(BGM.None, BGM.Stage1Loop);
        }
    }

    private IEnumerator Cutscene()
    {
        List<string> strings = new List<string>();
        for (int i = 0; i < texts.Count; i++)
        {
            strings.Add(texts[i].text);
            texts[i].text = string.Empty;
        }
        TransitionTexture.Value = TransitionTexture2D;
        texts[0].DOText(strings[0], 2);
        float t = 0;
        while (t < 1.4f)
        {
            t += Time.deltaTime;
            VignetteValue.Value = 0.5f * (2 - t);
            yield return null;
        }
        yield return new WaitForSeconds(1.7f);
        for (int i = 1; i < texts.Count; i++)
        {
            texts[i].DOText(strings[i], 2);
            if (i < texts.Count - 1)
            {
                yield return new WaitForSeconds(3.05f);
            } else
            {
                yield return new WaitForSeconds(2.45f);
            }
        }
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            playerOffset.x = Mathf.Lerp(6, 0, 1 - t * t);
            playerOffset.y = Mathf.Lerp(Mathf.Sin(t * 2) * -2, 0, 1 - t * t);
            yield return null;
        }
        WhiteFlashValue.Value = 3;
        Manager.instance.player.InputOverride = false;
        VignetteValue.Value = 0;
        BackgroundProgress.Value++;
        cutscene = null;
        foreach (var e in HideWhenFlashing)
        {
            e.gameObject.SetActive(false);
        }
    }
}
