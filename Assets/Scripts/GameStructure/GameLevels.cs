using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level/Mode", fileName = "GameMode")]
public class GameLevels : ScriptableObject
{
    [InlineEditor]
    public List<Level> levels = new List<Level>();

    private int _currentLevel = 0;

    public void FinishCurrentWave()
    {
        levels[_currentLevel].FinishCurrentWave();
    }

    public IEnumerator Execute()
    {
        #if UNITY_EDITOR
        for (int i = Manager.instance.StartLevel; i < levels.Count; i++)
#else
        for (int i = 0; i < levels.Count; i++)
#endif
        {
            _currentLevel = i;
            yield return levels[i].DoLevel();
        }
    }
}
