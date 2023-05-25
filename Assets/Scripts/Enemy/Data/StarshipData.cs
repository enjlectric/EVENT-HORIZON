using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/StarshipData", fileName = "StarshipData")]
public class StarshipData : EnemyData<StarshipConfig>
{
    public ParticleSystem WingExplosionFX;
    public EnemyData HugeGunModuleData;
    public Enjlectric.ScriptableData.Concrete.ScriptableDataInt BackgroundEventInt;
    public EnemyData Demon;

    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);

        var _config = (StarshipConfig)behaviour.emitterContainer;

        behaviour.spriteRenderer.sortingOrder = 1;
        _config.AddWingEmitters(this);
        _config.HugeGunFrozenPosition = Vector2.right * 5.5f;

        _config.BlackHole.transform.parent = null;

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

        var _config = (StarshipConfig)behaviour.emitterContainer;

        _config.HugeGunModule.ForceInvulnerable(1);

        if (behaviour.state == 0)
        {
            if (behaviour.substate == 0)
            {
                if (behaviour.transform.position.x > MaskManager.GetPosition(0, EnemySpawn.ScreenEdge.Right, -4).x)
                {
                    behaviour.speed = Vector2.left * 2f;
                }
                else
                {
                    behaviour.SwitchSubstate(1);
                }
            } else
            {
                behaviour.speed = new Vector2(0, Mathf.Sin(behaviour.substateTimer) * 3 - behaviour.transform.position.y);

                if (behaviour.HasSurpassedSubstate(1, 1))
                {
                    Shoot(behaviour, Random.Range(2, 6));
                }
                if (behaviour.HasSurpassedSubstate(0.8f, 0.8f))
                {
                    Shoot(behaviour, 0, 0);
                }
            }

            _config.FreezeHugeGun(behaviour);
        }
        else if (behaviour.state == 1)
        {
            if (behaviour.substate == 0)
            {
                if (behaviour.HasSurpassedSubstate(1.1f))
                {
                    behaviour.speed = Vector2.zero;
                    _config.BlackHole.SetActive(true);
                }

                if (behaviour.HasSurpassedSubstate(5f))
                {
                    _config.BlackHole.SetActive(false);
                    Shoot(behaviour, 1, 0);
                }

                if (behaviour.HasSurpassedSubstate(5.8f))
                {
                    behaviour.SwitchSubstate(1);
                }
            } else if (behaviour.substate == 1)
            {
                if (behaviour.substate >= 1f)
                {
                    if (behaviour.HasSurpassedSubstate(0.65f, 0.65f))
                    {
                        Shoot(behaviour, 0, 1);
                    }
                    behaviour.speed = new Vector2(0, Mathf.Sin(behaviour.substateTimer) * 3 - behaviour.transform.position.y);
                }

                if (behaviour.HasSurpassedSubstate(9))
                {
                    behaviour.SwitchSubstate(2);
                }
            } else if (behaviour.substate == 2)
            {
                behaviour.speed = new Vector2(0, Mathf.Sin(behaviour.substateTimer) * 3 - behaviour.transform.position.y);

                if (behaviour.HasSurpassedSubstate(1, 1))
                {
                    Shoot(behaviour, Random.Range(2, 6));
                }
                if (behaviour.HasSurpassedSubstate(0.8f, 0.8f))
                {
                    Shoot(behaviour, 0, 0);
                }
                if (behaviour.HasSurpassedSubstate(0.35f, 0.95f))
                {
                    Shoot(behaviour, 0, 1);
                }
                if (behaviour.HasSurpassedSubstate(7))
                {
                    behaviour.SwitchSubstate(0);
                }
            }
            _config.FreezeHugeGun(behaviour);
        }
        else if (behaviour.state == 2)
        {
            if (behaviour.substate == 0)
            {
                if (behaviour.HasSurpassedSubstate(1.1f))
                {
                    behaviour.speed = Vector2.zero;
                    _config.BlackHole.SetActive(true);
                    behaviour.SwitchSubstate(1);
                }
            } else if (behaviour.substate == 1)
            {
                behaviour.speed = (MaskManager.GetPositionRelativeToCam(0,0) - behaviour.transform.position) * 0.5f;
                if (behaviour.HasSurpassedSubstate(2.2f))
                {
                    _config.BlackHole.SetActive(true);
                }
                if (behaviour.HasSurpassedSubstate(3f))
                {
                    Shoot(behaviour, 1, 1);
                    behaviour.SwitchSubstate(2);
                    behaviour.speed = Vector2.zero;
                }
            } else if (behaviour.substate == 2)
            {
                if (behaviour.HasSurpassedSubstate(0.5f, 0.5f))
                {
                    Shoot(behaviour, 0, 3);
                    Shoot(behaviour, 0, 2);
                }
            }
        }
        else if (behaviour.state == 3)
        {
            if (behaviour.substate == 0)
            {
                var t = Mathf.Min(behaviour.substateTimer / 3.0f, 1);
                behaviour.transform.position = Vector2.Lerp(MaskManager.GetPositionRelativeToCam(0, 0), MaskManager.GetPositionRelativeToCam(0.7f, 0), t);
                _config.HugeGunFrozenPosition = Vector2.right * Mathf.Lerp(6, -3, t * t);

                if (behaviour.HasSurpassedSubstate(3))
                {
                    SFX.EnemyKingDockMachine.Play();
                }
                if (behaviour.HasSurpassedSubstate(4))
                {
                    behaviour.SwitchSubstate(1);
                }
                _config.FreezeHugeGun(behaviour);
            } else if (behaviour.substate == 1)
            {
                behaviour.AimSprite(behaviour.transform, Quaternion.Euler(0, 0, 3 * Mathf.Sin(behaviour.substateTimer)) * ((Vector2)behaviour.transform.position + Vector2.up));
                behaviour.AimSprite(_config.HugeGunModule.transform, Quaternion.Euler(0, 0, 3 * Mathf.Sin(behaviour.substateTimer)) * ((Vector2)behaviour.transform.position + Vector2.up));
                behaviour.speed = new Vector2(0, Mathf.Sin(behaviour.substateTimer) * 3 - behaviour.transform.position.y);
                _config.HugeGunModule.speed = behaviour.speed;
                if (behaviour.HasSurpassedSubstate(1, 1))
                {
                    Shoot(_config.HugeGunModule, Random.Range(0, 2), Random.Range(0, 2));
                }
                if (behaviour.HasSurpassedSubstate(2, 2))
                {
                    Shoot(_config.HugeGunModule, Random.Range(2, 4));
                }
                if (behaviour.HasSurpassedSubstate(1))
                {
                    BackgroundEventInt.Value++;
                }
            }
        }
        else if (behaviour.state == 4)
        {
            References.DestroyObjectsOfType<BulletBehaviour>();
            behaviour.ForceInvulnerable(1);
            var t = Mathf.Min(behaviour.substateTimer / 3.0f, 1);
            behaviour.transform.position = Vector2.Lerp(behaviour.transform.position, MaskManager.GetPositionRelativeToCam(0.7f, 0), t * t);
            if (behaviour.HasSurpassedSubstate(3))
            {
                References.CreateObject<EnemyBehaviour>(Demon, MaskManager.GetPositionRelativeToCam(1, 0), Quaternion.identity);
            }
            if (behaviour.HasSurpassedSubstate(3.3f))
            {
                behaviour.Kill();
            }
        }


        _config.BlackHole.transform.position = behaviour.transform.position;

        base.OnUpdate(behaviour);
    }

    public override void OnPhaseTransition(EnemyBehaviour behaviour)
    {
        References.DestroyObjectsOfType<BulletBehaviour>();
        SFX.ExplosionMiniboss.Play();
        var _config = (StarshipConfig)behaviour.emitterContainer;
        _config.BlackHole.SetActive(false);
        behaviour.speed = Vector2.zero;
        behaviour.spriteRenderer.transform.localRotation = Quaternion.identity;
        behaviour.transform.localRotation = Quaternion.identity;
        AbortRoutines(behaviour);
        behaviour.TiltSprite(0);
        
        behaviour.SwitchState(behaviour.state + 1);
        if (behaviour.state == 3)
        {
            foreach (var wing in _config.wings)
            {
                Instantiate(WingExplosionFX, wing.transform.position, Quaternion.identity);
                Destroy(wing.gameObject);
            }
            _config.RemoveWingEmitters();
            BackgroundEventInt.Value++;
        }

        if (behaviour.state == 4)
        {
            CoroutineManager.Start(((DebrisData)_config.HugeGunModule.data).KillForReal(_config.HugeGunModule));
        }
        base.OnPhaseTransition(behaviour);
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        References.DestroyObjectsOfType<BulletBehaviour>();

        AudioManager.ChangeMusic(null, 8);
        var _config = (StarshipConfig)behaviour.emitterContainer;
        Destroy(_config.BlackHole.gameObject);
        behaviour.speed = new Vector2(Random.Range(-1.0f, 1.0f), 0.2f);
        yield return base.OnKill(behaviour);
    }
}
