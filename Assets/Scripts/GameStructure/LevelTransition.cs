using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Game/Level/LoadScene", fileName = "LoadScene")]
public class LevelTransition : GameSection
{
    public int BuildIndex;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataFloat TransitionOpacity;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataTexture2D TransitionTexture;
    public Texture2D InTexture;
    public Texture2D OutTexture;
    public bool RefillHealth = false;

    private bool _isWaiting = false;
    internal override IEnumerator ExecutionRoutine()
    {
        TransitionTexture.Value = InTexture;
        yield return new WaitForSeconds(duration);

        TransitionOpacity.Value = 0;
        AudioManager.ChangeMusic(null, 2.5f);
        float t = 0;
        while (t < 1)
        {
            t = t + 2 * Time.deltaTime;

            TransitionOpacity.Value = t;
            yield return null;
        }
        _isWaiting = true;
        if (RefillHealth)
        {
            Manager.instance.player.Health.ResetToDefault();
        }
        SceneManager.activeSceneChanged += StopWaiting;
        Manager.LockPlayerInput(false);
        SceneManager.LoadScene(BuildIndex);
        References.DestroyObjectsOfType<BulletBehaviour>();
        References.DestroyObjectsOfType<EnemyBehaviour>();
        while (_isWaiting)
        {
            yield return null;
        }
        SceneManager.activeSceneChanged -= StopWaiting;
        if (!RefillHealth)
        {
            TransitionTexture.Value = OutTexture;
            t = 0;
            while (t < 1)
            {
                t = t + 2 * Time.deltaTime;

                TransitionOpacity.Value = 1 - t;
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    void StopWaiting(Scene a, Scene b)
    {
        _isWaiting = false;
    }
}