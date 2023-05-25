using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagementFunctions : MonoBehaviour
{
    public void StartGame()
    {
        Manager.instance.StartRun();
    }

    public void ToMainMenu()
    {
        if (Manager.instance.player != null)
        {
            Manager.instance.player.InputOverride = true;
        }
        SFX.UI_Confirm.Play();
        Manager.instance.GoToMainMenu();
    }

    public void EndGame()
    {
        SFX.UI_Confirm.Play();
        Application.Quit();
    }
}
