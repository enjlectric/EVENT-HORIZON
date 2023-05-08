using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public Enjlectric.ScriptableData.Concrete.ScriptableDataBool PauseBool;
    // Start is called before the first frame update
    void Awake()
    {
        PauseBool.OnValueChanged.AddListener(PauseUnpause);
    }
    void OnDestroy()
    {
        PauseBool.OnValueChanged.RemoveListener(PauseUnpause);
    }

    void PauseUnpause()
    {
        if (PauseBool.Value)
        {
            SFX.UI_Pause.Play();
            //Manager.SetUIInput();
            Time.timeScale = 0;
            AudioManager.SetMusicVolume(0.5f);
            AudioManager.PauseSFX(true);
        } else
        {
            SFX.UI_Unpause.Play();
            //Manager.SetWorldInput();
            Time.timeScale = 1;
            AudioManager.PauseSFX(false);
        }
    }
}
