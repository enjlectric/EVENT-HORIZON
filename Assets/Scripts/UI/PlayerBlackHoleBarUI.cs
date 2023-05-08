using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBlackHoleBarUI : MonoBehaviour
{
    public Image BarFillImage;
    public Color ChargeColor;
    public Color ChargedColor;
    public Color OverchargeColor;
    public Color White;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataFloat LimitMeter;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataFloat LimitMeterDamage;

    public float OverchargePercent = 1;
    public float offset = 0.5f;
    private CanvasGroup _cg;

    private void Start()
    {
        _cg = GetComponentInParent<CanvasGroup>();
        BarFillImage.fillAmount = 0;
    }

    // Start is called before the first frame update
    void Update()
    {
        BarFillImage.fillAmount = BarFillImage.fillAmount + (LimitMeterDamage.Value - BarFillImage.fillAmount) * Time.deltaTime * 4;

        if (LimitMeter.Value > OverchargePercent - 0.1f)
        {
            BarFillImage.color = ((Time.time % 0.15f) < 0.075f) ? White : OverchargeColor;
        }
        else if (LimitMeterDamage.Value < 0)
        {
            BarFillImage.color = ChargeColor;
        } else
        {
            BarFillImage.color = ChargedColor;
        }

        _cg.alpha = BarFillImage.fillAmount > 0.05f ? 1 : 0;
    }

    private void LateUpdate()
    {
        if (Manager.instance.player != null)
        {
            transform.parent.transform.position = Camera.main.WorldToScreenPoint(Manager.instance.player.transform.position + Vector3.up * offset);
        }
    }
}
