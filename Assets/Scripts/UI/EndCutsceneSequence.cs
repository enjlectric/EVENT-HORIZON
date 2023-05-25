using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Localization;

[System.Serializable]
public class EndingImageTextSet
{
    public Sprite sprite;
    public List<string> texts = new List<string>();
    public float opacity = 1;
    public float delay = 1;
    public float textdelay = 1;
}

public class EndCutsceneSequence : MonoBehaviour
{
    public Image BackgroundImage;
    public Text text;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataFloat OverlayOpacity;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataInt Points;
    public Text thanksText;
    public Text pointsText;
    public UnityEngine.Localization.Components.LocalizeStringEvent StringLocalization;


    public List<EndingImageTextSet> Sequence;

    public GameObject ReturnButton;

    // Start is called before the first frame update
    void Start()
    {
        CoroutineManager.Start(EndingCutscene());
        OverlayOpacity.Value = 1;
    }

    IEnumerator EndingCutscene()
    {
        yield return new WaitForSeconds(0.5f);
        OverlayOpacity.Value = 0;
        foreach (var ei in Sequence)
        {
            BackgroundImage.sprite = ei.sprite;
            BackgroundImage.DOFade(ei.opacity, 1.5f);
            text.DOFade(ei.opacity, 1.5f);
            foreach (var t in ei.texts)
            {
                if (t != string.Empty)
                {
                    text.DOComplete();
                    text.color = text.color.SetAlpha(1);
                }
                text.text = string.Empty;

                string realText = string.Empty;
                
                if (t != string.Empty)
                {
                    StringLocalization.SetEntry(t);
                    realText = StringLocalization.StringReference.GetLocalizedString();
                    text.DOText(realText, realText.Length * 0.1f).SetEase(Ease.Linear);
                }
                yield return new WaitForSeconds(ei.textdelay + realText.Length * 0.1f);
            }

            yield return new WaitForSeconds(ei.delay + 1.5f);
        }

        thanksText.DOText(Points.Value.ToString(), 2).SetEase(Ease.Linear);
        pointsText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        ReturnButton.gameObject.SetActive(true);
    }
}
