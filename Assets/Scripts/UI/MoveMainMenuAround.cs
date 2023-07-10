using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MoveMainMenuAround : MonoBehaviour
{
    public Transform WorldBackgroundRoot;
    public SpriteRenderer Logo;
    public CanvasGroup MainButtonGroup;
    public CanvasGroup CreditsUI;
    public CanvasGroup OptionsUI;

    public Enjlectric.ScriptableData.Types.ScriptableDataFloat VignetteFloat;

    private void Awake()
    {
        CreditsUI.transform.localPosition = Vector3.left * 115;
        OptionsUI.transform.localPosition = Vector3.right * 115;
        VignetteFloat.Value = 1;
    }

    private void Start()
    {
        VignetteFloat.Value = 0;
    }

    private void CompleteAll()
    {
        WorldBackgroundRoot.DOComplete();
        MainButtonGroup.DOComplete();
        Logo.DOComplete();
        CreditsUI.DOComplete();
        OptionsUI.DOComplete();
    }

    public void GoToCredits()
    {
        CompleteAll();
        float duration = 0.75f;
        MainButtonGroup.blocksRaycasts = false;
        MainButtonGroup.interactable = false;
        CreditsUI.blocksRaycasts = true;
        CreditsUI.interactable = true;
        SFX.UI_Confirm.Play();

        WorldBackgroundRoot.DOLocalMoveX(6, duration).SetEase(Ease.OutQuad);
        Logo.DOFade(0, duration).SetEase(Ease.OutQuad);
        MainButtonGroup.DOFade(0, duration).SetEase(Ease.OutQuad);
        MainButtonGroup.transform.DOLocalMoveX(115, duration).SetEase(Ease.OutQuad);
        CreditsUI.DOFade(1, duration).SetEase(Ease.OutQuad);
        CreditsUI.transform.DOLocalMoveX(0, duration).SetEase(Ease.OutQuad);
        EventSystem.current.SetSelectedGameObject(CreditsUI.transform.GetChild(0).gameObject);
    }

    public void CreditsToMain()
    {
        CompleteAll();
        float duration = 0.75f;
        MainButtonGroup.blocksRaycasts = true;
        MainButtonGroup.interactable = true;
        CreditsUI.blocksRaycasts = false;
        CreditsUI.interactable = false;
        EventSystem.current.SetSelectedGameObject(MainButtonGroup.transform.GetChild(2).gameObject);
        SFX.UI_Confirm.Play();

        WorldBackgroundRoot.DOLocalMoveX(0, duration).SetEase(Ease.OutQuad);
        Logo.DOFade(1, duration).SetEase(Ease.OutQuad);
        MainButtonGroup.DOFade(1, duration).SetEase(Ease.OutQuad);
        MainButtonGroup.transform.DOLocalMoveX(0, duration).SetEase(Ease.OutQuad);
        CreditsUI.DOFade(0, duration).SetEase(Ease.OutQuad);
        CreditsUI.transform.DOLocalMoveX(-115, duration).SetEase(Ease.OutQuad);
    }

    public void GoToOptions()
    {
        CompleteAll();
        float duration = 0.75f;
        MainButtonGroup.blocksRaycasts = false;
        MainButtonGroup.interactable = false;
        OptionsUI.blocksRaycasts = true;
        OptionsUI.interactable = true;
        SFX.UI_Confirm.Play();

        WorldBackgroundRoot.DOLocalMoveX(-6, duration).SetEase(Ease.OutQuad);
        Logo.DOFade(0, duration).SetEase(Ease.OutQuad);
        MainButtonGroup.DOFade(0, duration).SetEase(Ease.OutQuad);
        MainButtonGroup.transform.DOLocalMoveX(-115, duration).SetEase(Ease.OutQuad);
        OptionsUI.DOFade(1, duration).SetEase(Ease.OutQuad);
        OptionsUI.transform.DOLocalMoveX(0, duration).SetEase(Ease.OutQuad);
        EventSystem.current.SetSelectedGameObject(OptionsUI.transform.GetChild(0).gameObject);
    }

    public void OptionsToMain()
    {
        CompleteAll();
        float duration = 0.75f;
        MainButtonGroup.blocksRaycasts = true;
        MainButtonGroup.interactable = true;
        OptionsUI.blocksRaycasts = false;
        OptionsUI.interactable = false;
        SFX.UI_Confirm.Play();
        EventSystem.current.SetSelectedGameObject(MainButtonGroup.transform.GetChild(1).gameObject);

        WorldBackgroundRoot.DOLocalMoveX(0, duration).SetEase(Ease.OutQuad);
        Logo.DOFade(1, duration).SetEase(Ease.OutQuad);
        MainButtonGroup.DOFade(1, duration).SetEase(Ease.OutQuad);
        MainButtonGroup.transform.DOLocalMoveX(0, duration).SetEase(Ease.OutQuad);
        OptionsUI.DOFade(0, duration).SetEase(Ease.OutQuad);
        OptionsUI.transform.DOLocalMoveX(115, duration).SetEase(Ease.OutQuad);
    }
}