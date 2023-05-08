using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class GameSection : ScriptableObject
{
    public float duration;
    public bool clearEnemyProjectilesOnWin;

    private UnityAction _completionCallback;
    private Coroutine _routine;

    public void Execute(UnityAction callback)
    {
        _routine = CoroutineManager.Start(ExecutionWrapper());
        _completionCallback = callback;
    }

    private IEnumerator ExecutionWrapper()
    {
        yield return ExecutionRoutine();
        if (Manager.instance.player == null || Manager.instance.player._hp > 0)
        {
            Finish();
        }
    }

    internal abstract IEnumerator ExecutionRoutine();

    public virtual void Finish()
    {
        CoroutineManager.Abort(_routine);

        if (clearEnemyProjectilesOnWin)
        {
            References.DestroyObjectsOfType<BulletBehaviour>();
        }

        _completionCallback.Invoke();
    }
}
