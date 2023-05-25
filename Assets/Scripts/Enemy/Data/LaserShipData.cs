using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/LaserShipData", fileName = "LaserShipData")]
public class LaserShipData : EnemyData<LaserShipConfig>
{
    public DebrisData TopData;
    public DebrisData MidData;
    public DebrisData BottomData;
    public int BarrierSegmentCount;
    public ParticleSystem BarrierSegmentExplosionFX;
    public GameObject BarrierPrefab;
    public GameObject AlienSpeechParticles;
    public bool Hard;

    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);

        var _config = (LaserShipConfig)behaviour.emitterContainer;

        _config.DistanceLeft = -2;
        _config.DistanceTopToBottom = 3.6f;

        _config.TopShip = References.CreateObject<EnemyBehaviour>(TopData, behaviour.transform.position, Quaternion.identity);
        _config.Barrier = References.CreateObject<EnemyBehaviour>(MidData, behaviour.transform.position, Quaternion.identity);
        _config.BottomShip = References.CreateObject<EnemyBehaviour>(BottomData, behaviour.transform.position, Quaternion.identity);

        for (int i = 0; i < BarrierSegmentCount; i++)
        {
            var barrier = Instantiate(BarrierPrefab, behaviour.transform.position, Quaternion.identity);
            _config.BarrierSprites.Add(barrier.transform);
        }
    }

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (behaviour.dead)
        {
            return;
        }

        var _config = (LaserShipConfig)behaviour.emitterContainer;

        if (behaviour.state == 0)
        {
            if (Hard)
            {
                _config.TopShip.ForceInvulnerable(250);
                _config.BottomShip.ForceInvulnerable(250);
            }
            _config.Barrier.ForceInvulnerable(250);
            behaviour.speed = Vector2.left * 2.1f;
            _config.DistanceLeft = _config.DistanceLeft + 2.6f * Manager.deltaTime;
            if (behaviour.HasSurpassedState(3))
            {
                behaviour.SwitchState(1);
            }

            _config.OnUpdate(behaviour);
        } else if (behaviour.state == 1)
        {
            float mod = 3;
            if (Hard)
            {
                mod = 2;
                _config.TopShip.ForceInvulnerable(250);
                _config.BottomShip.ForceInvulnerable(250);
            }
            _config.Barrier.ForceInvulnerable(250);
            behaviour.speed = Vector2.zero;

            if (!_config.TopShip.dead && behaviour.HasSurpassedState(1.5f, 6))
            {
                _config.TopShip.emitterContainer.emitters[0].EvaluateAllPatterns();
            }

            if (!_config.BottomShip.dead && behaviour.HasSurpassedState(4.5f, 6))
            {
                _config.BottomShip.emitterContainer.emitters[0].EvaluateAllPatterns();
            }

            if (_config.TopShip.dead || _config.BottomShip.dead)
            {
                mod *= 0.5f;
            }

            behaviour.speed = new Vector2(Mathf.Sin(behaviour.stateTimer * 3.25f) * 0.5f, Mathf.Cos(behaviour.stateTimer * 2) * 2.5f);

            if (behaviour.HasSurpassedState(1, mod))
            {
                Shoot(behaviour, Random.Range(0, 4), 0);
            }

            if (Hard && behaviour._hp < health * 0.5f)
            {
                Manager.instance.FinishCurrentWave();
                SFX.ExplosionBlackHole.Play();
                behaviour.speed = Vector2.zero;
                behaviour.SwitchState(4);
            }

            if (_config.TopShip.dead && _config.BottomShip.dead)
            {
                behaviour.SwitchState(2);
            }

            _config.OnUpdate(behaviour);
        }
        else if (behaviour.state == 2)
        {
            behaviour.speed = Vector2.zero;
            if (behaviour.HasSurpassedState(1))
            {
                if (Hard)
                {
                    _config.TopShip.ForceInvulnerable(0);
                    _config.BottomShip.ForceInvulnerable(0);
                }
                _config.Barrier.ForceInvulnerable(0);
                CoroutineManager.Start(((DebrisData)_config.TopShip.data).KillForReal(_config.TopShip));
                CoroutineManager.Start(((DebrisData)_config.BottomShip.data).KillForReal(_config.BottomShip));
                CoroutineManager.Start(((DebrisData)_config.Barrier.data).KillForReal(_config.Barrier));
                _config.Barrier = null;

                foreach (var barrier in _config.BarrierSprites)
                {
                    SFX.ExplosionMiniboss.Play();
                    Instantiate(BarrierSegmentExplosionFX, barrier.transform.position, Quaternion.identity);
                    References.Destroy(barrier.gameObject);
                }
            }
            if (behaviour.HasSurpassedState(1.5f))
            {
                behaviour.SwitchState(3);
            }
        }
        else if (behaviour.state == 3)
        {
            var target = (Vector2)MaskManager.GetPositionRelativeToCam(0.6f, 0) + new Vector2(Mathf.Sin(behaviour.stateTimer * 5.25f) * 0.5f, Mathf.Cos(behaviour.stateTimer * 3) * 2.5f);
            behaviour.speed = target - (Vector2)behaviour.transform.position;

            if (behaviour.HasSurpassedState(0.3f, 0.6f))
            {
                Shoot(behaviour, Random.Range(0, 4), 1);
            }
        } else if (behaviour.state == 4)
        {
            behaviour.ForceInvulnerable(1);
            if (behaviour.HasSurpassedState(1))
            {
                CoroutineManager.Start(((DebrisData)_config.TopShip.data).KillForReal(_config.TopShip));
                CoroutineManager.Start(((DebrisData)_config.BottomShip.data).KillForReal(_config.BottomShip));
            }
            if (behaviour.HasSurpassedState(2))
            {
                if (_config.Barrier != null)
                {
                    _config.Barrier.ForceInvulnerable(0);
                    CoroutineManager.Start(((DebrisData)_config.Barrier.data).KillForReal(_config.Barrier));
                    _config.Barrier = null;
                    foreach (var barrier in _config.BarrierSprites)
                    {
                        Instantiate(BarrierSegmentExplosionFX, barrier.transform.position, Quaternion.identity);
                        References.Destroy(barrier.gameObject);
                    }
                    SFX.ExplosionMiniboss.Play();
                }
            }

            if (behaviour.HasSurpassedSubstate(1))
            {
                SFX.EnemyAlienTalk.Play();
                AudioManager.ChangeMusic(null, 0.5f);
                Instantiate(AlienSpeechParticles, behaviour.transform);
            }

            if (behaviour.substateTimer > 2)
            {
                behaviour.speed = Vector2.right * (behaviour.stateTimer);

                if (behaviour.HasSurpassedState(8))
                {
                    References.DestroyObject(behaviour);
                }
            }
        }

        base.OnUpdate(behaviour);
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        if (behaviour.state < 3)
        {
            var _config = (LaserShipConfig)behaviour.emitterContainer;
            if (_config.Barrier != null)
            {
                _config.Barrier.ForceInvulnerable(0);
                CoroutineManager.Start(((DebrisData)_config.TopShip.data).KillForReal(_config.TopShip));
                CoroutineManager.Start(((DebrisData)_config.BottomShip.data).KillForReal(_config.BottomShip));
                CoroutineManager.Start(((DebrisData)_config.Barrier.data).KillForReal(_config.Barrier));
                foreach (var barrier in _config.BarrierSprites)
                {
                    Instantiate(BarrierSegmentExplosionFX, barrier.transform.position, Quaternion.identity);
                    References.Destroy(barrier.gameObject);
                }
                SFX.ExplosionMiniboss.Play();
            }
        }
        behaviour.speed = new Vector2(Random.Range(-1.0f, 1.0f), 0.2f);
        yield return base.OnKill(behaviour);
    }
}
