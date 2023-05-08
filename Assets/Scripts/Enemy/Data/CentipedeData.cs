using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/CentipedeData", fileName = "CentipedeData")]
public class CentipedeData : EnemyData<CentipedeConfig>
{
    public GameObject MidPrefab;
    public GameObject EndPrefab;
    public int SegmentCount;
    public int DistanceFrames;
    public ParticleSystem SegmentExplosion;


    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);

        var _config = (CentipedeConfig)behaviour.emitterContainer;

        _config.SpawnSegments(behaviour, MidPrefab, EndPrefab, SegmentCount, DistanceFrames);

        foreach (var ren in behaviour.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            if (ren != behaviour.spriteRenderer && ren != behaviour.HealthBarRenderer)
            {
                behaviour.AddRenderer(ren);
            }
        }
    }

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (behaviour.dead)
        {
            return;
        }

        if (behaviour.HasSurpassedState(1.153f, 10.24151f) && Random.Range(0, 10) == 0)
        {
            SFX.EnemyCentipedeSpawn.Play();
        }

        if (behaviour.state == 0)
        {
            behaviour.speed = Quaternion.Euler(0, 0, Mathf.Sin(behaviour.stateTimer * 4) * 30) * behaviour.startSpeed;
        }
        var _config = (CentipedeConfig)behaviour.emitterContainer;

        _config.OnUpdate();
        behaviour.AimSprite(behaviour.spriteRenderer.transform, behaviour.transform.position + Quaternion.Euler(0,0,-90) * new Vector3(behaviour.speed.x, behaviour.speed.y));

        base.OnUpdate(behaviour);
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        var _config = (CentipedeConfig)behaviour.emitterContainer;
        yield return _config.KillSegments(SegmentExplosion);
        behaviour.speed = new Vector2(Random.Range(-1.0f, 1.0f), 0.2f);
        yield return base.OnKill(behaviour);
    }
}
