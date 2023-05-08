using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EnemySpawn
{
    [AssetList(AssetNamePrefix = "Enemy_")]
    public List<EnemyData> enemies = new List<EnemyData>();
    public BulletPatternData.BulletListType enemyListSearchType;

    public enum ScreenEdge { Top, Left, Right, Bottom, Center }

    public ScreenEdge spawnEdge = ScreenEdge.Top;
    [Range(-2.1f, 2.1f)]
    [Tooltip("Xoffset for center spawn")]
    public float spawnPos = 0;
    [Tooltip("Yoffset for center spawn")]
    public float spawnDepth = 0;
    public float spawnPosModBetweenSpawns = 0;
    public float spawnDepthModBetweenSpawns = 0;

    public float initialDelay;
    public float timeBetweenSpawns;
    public int numberOfSpawns = 1;
    public float SpawnRotation;

    public ParticleSystem SpawnParticles;
    public float SpawnDelay = 0;

    public bool DoneSpawning { get; private set; }

    public bool Cleared => DoneSpawning && spawns.TrueForAll(s => s.gameObject.activeSelf == false);

    private List<EnemyBehaviour> spawns = new List<EnemyBehaviour>();

    public IEnumerator SpawnRoutine()
    {
        int enemySequence = 0;

        var delay = new WaitForSeconds(timeBetweenSpawns);
        spawns.Clear();
        if (initialDelay > 0)
        {
            yield return new WaitForSeconds(initialDelay);
        }

        for (int i = 0; i < numberOfSpawns; i++)
        {
            CoroutineManager.Start(DelayedSpawn(enemySequence, i));

            enemySequence = (enemySequence + 1) % enemies.Count;

            yield return delay;
        }

        DoneSpawning = true;
    }

    private IEnumerator DelayedSpawn(int enemySequence, int i)
    {
        var pos = MaskManager.GetPosition(spawnPos + i * spawnPosModBetweenSpawns, spawnEdge, spawnDepth + i * spawnDepthModBetweenSpawns);
        if (SpawnParticles != null)
        {
            Object.Instantiate(SpawnParticles, (Vector3)pos, Quaternion.Euler(0, 0, SpawnRotation));
        }
        yield return new WaitForSeconds(SpawnDelay);
        EnemyData data = enemies[enemySequence];
        switch (enemyListSearchType)
        {
            case BulletPatternData.BulletListType.Reverse: data = enemies[enemies.Count - 1 - enemySequence]; break;
            case BulletPatternData.BulletListType.Random: data = enemies[Random.Range(0, enemies.Count)]; break;
        }
        EnemyBehaviour behaviour = References.CreateObject<EnemyBehaviour>(data, pos, Quaternion.Euler(0,0,SpawnRotation));
        spawns.Add(behaviour);
    }
}

[CreateAssetMenu(menuName = "Game/Level/Wave", fileName = "EnemyWave")]
public class EnemyWave : GameSection
{
    public bool SkipClearingPriorRoutine = false;
    public List<EnemySpawn> enemySpawnList = new List<EnemySpawn>();
    public bool killEnemiesOnTimeout;

    private List<Coroutine> routines = new List<Coroutine>();

    internal override IEnumerator ExecutionRoutine()
    {
        int completedRoutines = 0;
        if (!SkipClearingPriorRoutine)
        {
            routines.ForEach(r => Manager.instance.StopCoroutine(r));
            routines.Clear();
        }
        UnityAction finishRoutine = () => completedRoutines++;
        float startTime = Time.time;

        foreach(var spawn in enemySpawnList)
        {
            routines.Add(CoroutineManager.Start(WaitFinish(spawn, finishRoutine)));
        }

        while (duration == 0 || startTime + duration > Time.time)
        {
            if (routines.Count <= completedRoutines && enemySpawnList.TrueForAll(l => l.Cleared))
            {
                break;
            }
            yield return null;
        }

        if (Manager.instance.player != null && Manager.instance.player._hp > 0)
        {
            Finish();
        }
    }

    public override void Finish()
    {
        if (!SkipClearingPriorRoutine)
        {
            routines.ForEach(r => Manager.instance.StopCoroutine(r));
            routines.Clear();
        }

        if (killEnemiesOnTimeout)
        {
            References.DeactivateAll<EnemyBehaviour>();
            References.DeactivateAll<BulletBehaviour>(b => b.gameObject.layer == 7);
        }

        base.Finish();
    }

    private IEnumerator WaitFinish(EnemySpawn spawn, UnityAction callback)
    {
        yield return spawn.SpawnRoutine();
        callback();
    }
}
