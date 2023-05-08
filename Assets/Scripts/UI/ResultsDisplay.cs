using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResultsDisplay : MonoBehaviour
{
    public Text title;
    public Text value;
    public Image underline;
    public CanvasGroup ownGroup;

    public void Reset()
    {
        if (ownGroup != null)
        {
            ownGroup.alpha = 0;
        }
    }
    
    public void Show(string achieved)
    {
        ownGroup.DOFade(1, 0.5f).SetEase(Ease.OutQuad);
        value.text = achieved;
        transform.localScale = Vector3.one * 0.5f;
        transform.DOScale(1, 0.6f).SetEase(Ease.OutBack);
    }
}
