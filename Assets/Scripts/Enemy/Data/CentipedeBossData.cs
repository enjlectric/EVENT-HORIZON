using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/CentipedeBossData", fileName = "CentipedeBossData")]
public class CentipedeBossData : EnemyData<CentipedeConfig>
{
    private CentipedeConfig _config;

    public GameObject MidPrefab;
    public GameObject EndPrefab;
    public int SegmentCount;
    public int DistanceFrames;
    public ParticleSystem SegmentExplosion;

    private float multiplier = 1;
    private bool scheduleMoreSegments = false;

    public bool RegularEnemy = false;

    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);

        if (RegularEnemy)
        {
            behaviour._hp = health * 0.5f;
        }

        _config = (CentipedeConfig)behaviour.emitterContainer;

        behaviour.speed = Vector2.left;

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

        if (behaviour.state == 0)
        {
            behaviour.speed = Quaternion.Euler(0, 0, Mathf.Sin(behaviour.stateTimer * multiplier) * 30) * behaviour.startSpeed * 4;
        }
        else if (behaviour.state == 1)
        {
            behaviour.speed = Quaternion.Euler(0, 0, behaviour.stateTimer * 15 * multiplier) * behaviour.startSpeed * 4;
        } else if (behaviour.state == 99)
        {
            if (behaviour.HasSurpassedState(3))
            {
                var x = Random.Range(-0.1f, 0.6f);
                var y = Random.Range(-0.5f, 0.5f);
                var dir = Vector2.zero;

                if (Random.Range(0, 2) == 1)
                {
                    x = (Random.Range(0, 2) - 0.5f) * 2;
                    dir.x = -Mathf.Sign(x);
                }
                else
                {
                    y = (Random.Range(0, 2) - 0.5f) * 2;
                    dir.y = -Mathf.Sign(y);
                }
                var pos = MaskManager.GetPositionRelativeToCam(x, y);
                behaviour.transform.position = pos - 6 * new Vector3(dir.x, dir.y);
                if (scheduleMoreSegments)
                {
                    scheduleMoreSegments = false;
                    _config.SpawnMoreSegments(4);
                }
                multiplier = (Random.Range(0, 2) - 0.5f) * 2;
                behaviour.startSpeed = dir;
                if (behaviour._hp < health * 0.66f)
                {
                    behaviour.startSpeed = Quaternion.Euler(0, 0, Random.Range(-17, 18)) * behaviour.startSpeed;
                }
                behaviour.SwitchState(Random.Range(0, 2));
            }
        }

        if (behaviour.state < 99)
        {
            if (MaskManager.IsInBounds(_config.Segments[3].transform.position) && !MaskManager.IsInBounds(behaviour.transform.position))
            {
                if (RegularEnemy)
                {
                }
                else
                {
                    behaviour.SwitchState(99);
                }
            }
        } else if (behaviour.state == 100)
        {
        }

        var mod = 0.75f + 1f * (behaviour._hp / health);
        if (behaviour.HasSurpassedState(mod, mod))
        {
            var lim = 1;
            var lim2 = 0;
            if (behaviour._hp < health * 0.75f)
            {
                lim = 2;
            }
            if (behaviour._hp < health * 0.4f)
            {
                lim2 = 1;
            }
            List<int> candidates = new List<int>();
            int i = 0;
            foreach (var emitter in behaviour.emitterContainer.emitters)
            {
                if (MaskManager.IsInBounds(emitter.transform.position))
                {
                    candidates.Add(i);
                }
                i++;
            }
            for (int i2 = 0; i2 < 5 - (behaviour._hp / health) * 4; i2++)
            {
                if (candidates.Count > 0)
                {
                    var idx = Random.Range(0, candidates.Count);
                    Shoot(behaviour, candidates[idx], Random.Range(lim2, lim));
                    candidates.RemoveAt(idx);
                }
            }
        }

        _config.OnUpdate();
        behaviour.AimSprite(behaviour.spriteRenderer.transform, behaviour.transform.position + Quaternion.Euler(0,0,-90) * new Vector3(behaviour.speed.x, behaviour.speed.y));

        base.OnUpdate(behaviour);
    }

    public override void OnPhaseTransition(EnemyBehaviour behaviour)
    {
        AbortRoutines(behaviour);
        scheduleMoreSegments = true;
        base.OnPhaseTransition(behaviour);
    }

    internal override IEnumerator OnDespawn(EnemyBehaviour behaviour)
    {
        foreach (var segment in _config.Segments)
        {
            Destroy(segment.gameObject);
        }
        return base.OnDespawn(behaviour);
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        AudioManager.ChangeMusic(null, 8);
        yield return _config.KillSegments(SegmentExplosion);
        behaviour.speed = new Vector2(Random.Range(-1.0f, 1.0f), 0.2f);
        yield return base.OnKill(behaviour);
    }
}
