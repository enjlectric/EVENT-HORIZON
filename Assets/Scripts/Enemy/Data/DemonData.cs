using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/DemonData", fileName = "DemonData")]
public class DemonData : EnemyData<DemonConfig>
{
    public GameObject MidPrefab;
    public GameObject EndPrefab;
    public int SegmentCount;
    public float DistanceFrames;
    public ParticleSystem SegmentExplosion;
    public ParticleSystem ScreechEffect;
    public Enjlectric.ScriptableData.Types.ScriptableDataInt BackgroundEventInt;
    public bool IsBoss;

    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);

        var _config = (DemonConfig)behaviour.emitterContainer;

        _config.SpawnSegments(behaviour, MidPrefab, EndPrefab, SegmentCount, DistanceFrames);
        behaviour.spriteRenderer.sortingOrder = -5;

        foreach (var ren in behaviour.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            if (ren != behaviour.spriteRenderer && ren != behaviour.HealthBarRenderer)
            {
                behaviour.AddRenderer(ren);
            }
        }
        behaviour.speed = 12 * Vector2.left;
    }

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (behaviour.dead)
        {
            return;
        }

        var _config = (DemonConfig)behaviour.emitterContainer;

        if (behaviour.state == 0)
        {
            behaviour.speed = behaviour.speed * (1 - Manager.deltaTime * 2f);

            if (behaviour.HasSurpassedState(1))
            {
                Instantiate(ScreechEffect, behaviour.transform.position + Vector3.left * 1, Quaternion.identity);
                SFX.EnemyBeastRoar.Play();
            }

            if (behaviour.HasSurpassedState(3))
            {
                if (IsBoss)
                {
                    behaviour.SwitchState(1);
                }
                else
                {
                    BackgroundEventInt.Value++;
                }
            }

            if (behaviour.HasSurpassedState(6))
            {
                BackgroundEventInt.Value++;
                behaviour.Despawn();
            }
        }
        else if (behaviour.state == 1)
        {
            if (behaviour.substate == 0)
            {
                behaviour.speed.y = Mathf.Sin(Mathf.Sin(behaviour.stateTimer * 2) * 1.15f - behaviour.transform.position.y);
                var speedY = behaviour.speed.y;
                if (behaviour.substateTimer < 4)
                {
                    behaviour.speed = behaviour.speed * (1 - Manager.deltaTime * 2f);
                    if (behaviour.HasSurpassedSubstate(2))
                    {
                        Shoot(behaviour, 0, 0);
                    }
                }
                else
                {
                    if (behaviour.HasSurpassedSubstate(4))
                    {
                        Shoot(behaviour, 1, 0);
                        Shoot(behaviour, 1, 1);
                        behaviour.speed = Vector2.right * 4;
                    }
                    if (behaviour.HasSurpassedSubstate(0.65f, 0.65f))
                    {
                        Shoot(behaviour, 1, 0);
                        Shoot(behaviour, 1, 1);
                    }

                    if (behaviour.HasSurpassedSubstate(1.3f, 1.3f))
                    {
                        Shoot(behaviour, 2 + (Mathf.FloorToInt(behaviour.substateTimer / 1.3f) * 2) % 5, 0);
                    }
                    if (behaviour.substateTimer < 6)
                    {
                        behaviour.speed = behaviour.speed * (1 - Manager.deltaTime * 2f);
                    }
                    else if (behaviour.substateTimer < 12)
                    {
                        behaviour.speed = Vector2.left * Mathf.Min((behaviour.substateTimer - 6) * 4, 1);
                    }
                    else
                    {
                        behaviour.speed = Vector2.left * 1 + Vector2.right * Mathf.Min((behaviour.substateTimer - 12) * 4, 2f);
                        if (behaviour.transform.position.x > MaskManager.GetPositionRelativeToCam(0.6f, 0).x)
                        {
                            behaviour.substateTimer = 0;
                        }
                    }
                }
                behaviour.speed.y = speedY;
            }
        }
        else if (behaviour.state == 2)
        {
            if (behaviour.substate == 0)
            {
                behaviour.speed = Vector2.left * 1 + Vector2.right * Mathf.Min((behaviour.substateTimer) * 4, 2f);
                if (behaviour.transform.position.x > MaskManager.GetPositionRelativeToCam(0.6f, 0).x)
                {
                    behaviour.SwitchSubstate(1);
                }
            }
            else if (behaviour.substate == 1)
            {
                behaviour.speed.y = Mathf.Sin(Mathf.Sin(behaviour.stateTimer * 2) * 1.15f - behaviour.transform.position.y);
                var speedY = behaviour.speed.y;
                behaviour.speed = behaviour.speed * (1 - Manager.deltaTime * 2f);
                if (behaviour.HasSurpassedSubstate(1.4f, 1.4f))
                {
                    Shoot(behaviour, 7, 0);
                }
                if (behaviour.HasSurpassedSubstate(1.3f, 1.3f))
                {
                    Shoot(behaviour, 2 + (Mathf.FloorToInt(behaviour.substateTimer / 1.3f) * 2) % 5, 0);
                }
                behaviour.speed.y = speedY;
            }
        }
        else if (behaviour.state == 3)
        {
            if (behaviour.HasSurpassedSubstate(2))
            {
                Instantiate(ScreechEffect, behaviour.transform.position + Vector3.left * 1, Quaternion.identity);
                SFX.EnemyBeastRoar.Play();
            }
            if (behaviour.stateTimer > 11)
            {
                if (behaviour.transform.position.x > Manager.instance.player.transform.position.x)
                {
                    behaviour.speed.x = behaviour.speed.x - Manager.deltaTime * 14;
                    behaviour.speed.y = 0;
                }
                else
                {
                    behaviour.speed = behaviour.speed * (1 - Manager.deltaTime * 2f);

                    if (behaviour.substate == 0)
                    {
                        BackgroundEventInt.Value++;
                        behaviour.substate = 1;
                        behaviour.substateTimer = 150f;
                    }
                    else
                    {
                        //if (behaviour.HasSurpassedSubstate(152.5f))
                        //{
                        //    _config.KillSegments();
                        //    behaviour.Despawn();
                        //}
                    }
                }
            }
            else if (behaviour.substateTimer > 4)
            {
                var target = Manager.instance.player.transform.position + Vector3.right * 8;
                target.x = Mathf.Min(target.x, MaskManager.GetPositionRelativeToCam(0.6f, 0).x);
                behaviour.speed = (target - behaviour.transform.position) * 2;

                if (behaviour.HasSurpassedSubstate(8))
                {
                    Shoot(behaviour, 0, 0);
                }

                if (behaviour.HasSurpassedSubstate(8.7f))
                {
                    Manager.instance.player.ForceInvulnerable(4);
                }

                if (behaviour.HasSurpassedSubstate(9.5f))
                {
                    foreach (var col in behaviour.GetComponentsInChildren<Collider2D>())
                    {
                        col.enabled = false;
                    }
                    Manager.LockPlayerInput(true);
                }

                if (behaviour.HasSurpassedSubstate(10f))
                {
                    Manager.instance.player.HurtEffect();
                    AudioManager.ChangeMusic(null, 2.25f);
                }
            }
        }

        _config.OnUpdate();
        base.OnUpdate(behaviour);
    }

    public override void OnPhaseTransition(EnemyBehaviour behaviour)
    {
        base.OnPhaseTransition(behaviour);
        SFX.ExplosionMiniboss.Play();

        behaviour.SwitchState(behaviour.state + 1);
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        behaviour.speed = new Vector2(Random.Range(-1.0f, 1.0f), 0.2f);
        yield return base.OnKill(behaviour);
    }
}