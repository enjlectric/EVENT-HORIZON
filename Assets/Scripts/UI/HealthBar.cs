using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image bar;
    public CanvasGroup group;

    private Tweener _fillTween;
    private Tweener _shakeTween;

    public void Initialize(Color color)
    {
        bar.fillAmount = 0;
        bar.color = color;
        _fillTween?.Complete();
        _fillTween = bar.DOFillAmount(1, 1.45f).SetEase(Ease.OutQuad);
    }
    public void ReduceTo(float newFillAmount)
    {
        _fillTween?.Complete();
        bar.fillAmount = newFillAmount;
        _shakeTween?.Complete();
        if (bar.fillAmount > 0)
        {
            _shakeTween = bar.transform.DOShakePosition(0.5f, 0.1f, 15);
        } else
        {
            transform.DOLocalMoveY(transform.localPosition.y - 12, 1).SetEase(Ease.InBack);
            transform.DOLocalMoveX(transform.localPosition.x - 2f, 1).SetEase(Ease.OutQuad);
            group.DOFade(0, 1).SetEase(Ease.OutQuad).OnComplete(() => Destroy(gameObject));
        }
    }
}
