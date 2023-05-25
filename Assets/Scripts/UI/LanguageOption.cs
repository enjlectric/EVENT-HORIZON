using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageOption : Selectable
{
    public GameObject Arrows;
    public string PlayerPrefsKey = "selected-locale";

    public void UpdateValue(int step)
    {
        Locale locale = LocalizationSettings.SelectedLocale;
        int idx = LocalizationSettings.AvailableLocales.Locales.IndexOf(locale);
        idx = idx + step;
        if (idx == LocalizationSettings.AvailableLocales.Locales.Count)
        {
            idx = 0;
        }

        if (idx == -1)
        {
            idx = LocalizationSettings.AvailableLocales.Locales.Count - 1;
        }
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[idx];
        PlayerPrefs.SetString(PlayerPrefsKey, locale.name);
        SFX.UI_Move.Play();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        UnityEngine.InputSystem.UI.InputSystemUIInputModule input = (UnityEngine.InputSystem.UI.InputSystemUIInputModule) EventSystem.current.currentInputModule;
        input.move.action.performed += OnMove;
        Arrows.gameObject.SetActive(true);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<Vector2>();
        if (Mathf.Abs(value.y) < 0.2f && Mathf.Abs(value.x) > 0.5f)
        {
            UpdateValue((int)Mathf.Sign(value.x));
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        UnityEngine.InputSystem.UI.InputSystemUIInputModule input = (UnityEngine.InputSystem.UI.InputSystemUIInputModule)EventSystem.current.currentInputModule;
        input.move.action.performed -= OnMove;
        Arrows.gameObject.SetActive(false);
    }
}
