using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ScreenSizeOption : Selectable
{
    public GameObject Arrows;
    public Text Resolution;
    public List<Vector2Int> availableResolutions = new List<Vector2Int>();

    protected override void OnEnable()
    {
        base.OnEnable();
#if !UNITY_EDITOR
        Resolution.text = Screen.width + "x" + Screen.height;
#endif
    }
    public void UpdateValue(int step)
    {
        int idx = 0;
        foreach(var res2 in availableResolutions)
        {
            if (res2.x == Screen.width && res2.y == Screen.height)
            {
                break;
            }
            idx++;
        }
        idx = idx + step;
        if (idx == availableResolutions.Count)
        {
            idx = 0;
        }

        if (idx == -1)
        {
            idx = availableResolutions.Count - 1;
        }
        Screen.SetResolution(availableResolutions[idx].x, availableResolutions[idx].y, Screen.fullScreenMode);
        Resolution.text = availableResolutions[idx].x + "x" + availableResolutions[idx].y;
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
