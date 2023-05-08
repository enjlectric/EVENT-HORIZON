using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverSound : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{

    public SFX HoverSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverSound.Play();
    }

    public void OnSelect(BaseEventData eventData)
    {
        HoverSound.Play();
        //transform.DOComplete();
        //transform.DOPunchPosition(Vector3.right * 3f, 0.1f).SetEase(Ease.OutQuad);
    }

    public void SelectThis()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
