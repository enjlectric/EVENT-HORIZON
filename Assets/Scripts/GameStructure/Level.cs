using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level/Level", fileName = "Level")]
public class Level : ScriptableObject
{
    public List<GameSection> sections = new List<GameSection>();

    private bool _done;

    public BGM bgmIntro;
    public BGM bgmLoop;

    public List<Sprite> sprites;
    public List<Vector3> positions;
    public Sprite roadSprite;
    public Material roadMaterial;

    private int _currentWave = 0;

    public IEnumerator DoLevel()
    {
        _done = false;

        AudioManager.ChangeMusic(bgmIntro, bgmLoop);

#if !UNITY_EDITOR
        RecursiveExecute(Manager.instance.StartWave);
#else
        RecursiveExecute(0);
#endif

        while (!_done)
        {
            yield return null;
        }
    }

    public void FinishCurrentWave()
    {
        sections[_currentWave].Finish();
    }

    private void RecursiveExecute(int idx)
    {
        if (idx >= sections.Count)
        {
            _done = true;
            return;
        }
        _currentWave = idx;
        sections[idx].Execute(() => RecursiveExecute(idx + 1));
    }
}
