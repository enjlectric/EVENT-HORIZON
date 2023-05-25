using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AlertOverlay : MonoBehaviour
{
    public CanvasGroup Background;
    public CanvasGroup Exclamation;
    public CanvasGroup AlertText;
    private LayoutElement BackgroundLayout;
    public Text Text;
    public UnityEngine.Localization.Components.LocalizeStringEvent localizeStringEvent;

    public Enjlectric.ScriptableData.Concrete.ScriptableDataString TextString;
    // Start is called before the first frame update
    void Start()
    {
        Text.text = "";
        Background.alpha = 0;
        BackgroundLayout = Background.GetComponent<LayoutElement>();
        Exclamation.alpha = 0;
        AlertText.alpha = 0;
        TextString.OnValueChanged.AddListener(StartAlert);
    }

    private void OnDestroy()
    {
        TextString.OnValueChanged.RemoveListener(StartAlert);
    }

    public void StartAlert()
    {
        CoroutineManager.Start(AlertRoutine());
    }

    private IEnumerator AlertRoutine()
    {
        localizeStringEvent.SetEntry(TextString.Value);
        localizeStringEvent.RefreshString();

        var ts = localizeStringEvent.StringReference.GetLocalizedString();

        if (ts[0] == '!')
        {
            ts = (ts.TrimStart('!'));
            SFX.UI_Alert_Big.Play();
        } else if (ts[0] == '?')
        {
            ts = (ts.TrimStart('?'));
        } else 
        {
            SFX.UI_Alert_Small.Play();
        }

        Text.text = string.Empty;

        Exclamation.DOFade(1, 0.35f).SetEase(Ease.OutQuad);
        Exclamation.transform.localPosition = 0 * Vector3.left;
        AlertText.transform.localPosition = 0 * Vector3.right;
        Exclamation.transform.localScale = Vector3.one * 2;
        Exclamation.transform.DOScale(Vector3.one, 0.45f).SetEase(Ease.InBack);
        yield return new WaitForSeconds(1.3f);
        Exclamation.transform.DOLocalMoveX(-52, 0.5f).SetEase(Ease.OutQuad);
        AlertText.DOFade(1, 1).SetEase(Ease.OutQuad);
        AlertText.transform.DOLocalMoveX(8, 0.5f).SetEase(Ease.OutQuad);
        Background.DOFade(1, 1).SetEase(Ease.OutQuad);
        Background.transform.localPosition = Vector3.zero;
        BackgroundLayout.minWidth = 0;
        BackgroundLayout.minHeight = 4;
        BackgroundLayout.DOMinSize(new Vector2(130, 4), 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.5f);
        BackgroundLayout.DOMinSize(new Vector2(130, 34), 0.5f).SetEase(Ease.OutQuad);
        Text.DOText(ts, 0.75f);
        yield return new WaitForSeconds(3);
        transform.DOScaleY(0, 0.25f).SetEase(Ease.OutQuint);
        yield return new WaitForSeconds(0.25f);
        Text.text = "";
        Background.alpha = 0;
        BackgroundLayout = Background.GetComponent<LayoutElement>();
        Exclamation.alpha = 0;
        AlertText.alpha = 0;
        transform.localScale = Vector3.one;
    }
}
