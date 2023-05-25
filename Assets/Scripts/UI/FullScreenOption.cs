using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class FullScreenOption : Selectable
{
    public GameObject Arrows;
    public List<FullScreenMode> availableResolutions = new List<FullScreenMode>();
    public UnityEngine.Localization.Components.LocalizeStringEvent Text;
    public List<string> langStringKeys = new List<string>();

    protected override void OnEnable()
    {
        base.OnEnable();
#if !UNITY_EDITOR
        UpdateValue(0);
#endif
    }
    public void UpdateValue(int step)
    {
        var res = Screen.fullScreenMode;
        int idx = availableResolutions.IndexOf(res);
        idx = idx + step;
        if (idx == availableResolutions.Count)
        {
            idx = 0;
        }

        if (idx == -1)
        {
            idx = availableResolutions.Count - 1;
        }
        Screen.SetResolution(Screen.width, Screen.height, availableResolutions[idx]);
        Text.SetEntry(langStringKeys[idx]);
        Text.RefreshString();
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
