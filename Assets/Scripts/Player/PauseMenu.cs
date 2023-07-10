using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public Enjlectric.ScriptableData.Types.ScriptableDataBool PauseBool;

    // Start is called before the first frame update
    private void Awake()
    {
        PauseBool.OnValueChanged.AddListener(PauseUnpause);
    }

    private void OnDestroy()
    {
        PauseBool.OnValueChanged.RemoveListener(PauseUnpause);
    }

    private void PauseUnpause()
    {
        if (PauseBool.Value)
        {
            SFX.UI_Pause.Play();
            //Manager.SetUIInput();
            Time.timeScale = 0;
            AudioManager.SetMusicVolume(0.5f);
            AudioManager.PauseSFX(true);
        }
        else
        {
            SFX.UI_Unpause.Play();
            //Manager.SetWorldInput();
            Time.timeScale = 1;
            AudioManager.PauseSFX(false);
        }
    }
}