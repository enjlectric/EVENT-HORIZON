using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicOnSceneLoad : MonoBehaviour
{
    public BGM intro;
    public BGM loop;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;
        AudioManager.ChangeMusic(intro, loop);
    }
}
