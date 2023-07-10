using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StageEnd : MonoBehaviour
{
    public Enjlectric.ScriptableData.Types.ScriptableDataInt TriggerThis;

    public Text KillsText;
    public Text KillComboText;
    public Text HitsText;
    public Text KillsBonusText;
    public Text KillComboBonusText;
    public Text HitsBonusText;
    public Text sumText;

    public Transform background;
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
        Manager.LockPlayerInput(true);
        _cg.alpha = 1;
        CoroutineManager.Start(CutsceneIntro());
        TriggerThis.SetValueWithoutNotify(0);
    }

    private IEnumerator CutsceneIntro()
    {
        transform.localScale = Vector3.one - Vector3.up;
        yield return new WaitForSeconds(3.5f);
        SFX.UI_ShowResults.Play();
        transform.DOScaleY(1, 0.8f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(1.5f);

        var killsBonus = Manager.instance.thisLevelKills * 10;
        var hitsBonus = Manager.instance.thisLevelHits * 500;
        var hitsComboBonus = Mathf.Max(Manager.instance.thisLevelCombo, Manager.instance.currentCombo);
        hitsComboBonus = Mathf.CeilToInt(hitsComboBonus * 50);

        KillsText.text = Manager.instance.thisLevelKills.ToString();
        KillsBonusText.text = "+" + killsBonus.ToString();
        KillsText.transform.parent.DOScaleY(1, 0.3f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(0.15f);

        KillComboText.text = Mathf.Max(Manager.instance.thisLevelCombo, Manager.instance.currentCombo).ToString();
        KillComboBonusText.text = "+" + hitsComboBonus.ToString();
        KillComboBonusText.transform.parent.DOScaleY(1, 0.3f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(0.15f);

        HitsText.text = Manager.instance.thisLevelHits.ToString();
        HitsBonusText.text = "-" + hitsBonus.ToString();
        HitsText.transform.parent.DOScaleY(1, 0.3f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(0.75f);

        Manager.instance.AddPoints(killsBonus - hitsBonus + hitsComboBonus);

        sumText.text = (killsBonus - hitsBonus + hitsComboBonus).ToString();
        sumText.transform.parent.DOScaleY(1, 0.3f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(1.5f);
        transform.DOScaleY(0, 0.25f).SetEase(Ease.InBack);

        float t = 0;
        while (t < 1 && Manager.instance.player != null)
        {
            t += Time.deltaTime;
            Manager.instance.player._speed = Vector2.right * 350 * (t * t);
            yield return null;
        }
    }
}