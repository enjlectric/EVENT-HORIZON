using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StageIntro : MonoBehaviour
{
    public Enjlectric.ScriptableData.Types.ScriptableDataInt TriggerThis;

    public Transform background;
    public Text text;
    private CanvasGroup _cg;

    // Start is called before the first frame update
    private void Awake()
    {
        TriggerThis.OnValueChanged.AddListener(DoCutscene);
        _cg = GetComponent<CanvasGroup>();
        _cg.alpha = 0;
    }

    private void OnDestroy()
    {
        TriggerThis.OnValueChanged.RemoveListener(DoCutscene);
    }

    private void DoCutscene()
    {
        _cg.alpha = 1;
        if (TriggerThis.Value == 1)
        {
            CoroutineManager.Start(CutsceneIntro());
        }
        else
        {
            CoroutineManager.Start(Cutscene());
        }
        TriggerThis.SetValueWithoutNotify(0);
    }

    private IEnumerator CutsceneIntro()
    {
        var t = text.text;
        text.text = "";
        text.DOText(t, 1.5f);
        background.transform.localScale = Vector3.one + Vector3.left;
        background.DOScaleX(1, 1.5f);

        yield return new WaitForSeconds(1.5f);

        yield return Cutscene();
    }

    private IEnumerator Cutscene()
    {
        yield return new WaitForSeconds(2);
        transform.DOScaleY(0, 0.25f).SetEase(Ease.InBack);
    }
}