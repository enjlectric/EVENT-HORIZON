using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningPulse : MonoBehaviour
{
    public Image image;
    public Gradient gradient;

    public void Reset()
    {
        if (image != null)
        {
            image.transform.localScale = Vector3.zero;
            image.color = new Color(1, 1, 1, 0);
        }
    }

    public void DoSequence()
    {
        if (gameObject.activeInHierarchy)
        {
            CoroutineManager.Start(Sequence());
        }
    }

    private IEnumerator Sequence()
    {
        Reset();
        image.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutFlash);
        image.DOFade(1, 0.25f).SetEase(Ease.OutQuad);
        image.DOGradientColor(gradient, 5).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(4.5f);
        image.DOFade(0, 0.5f).SetEase(Ease.OutQuad);
    }
}
