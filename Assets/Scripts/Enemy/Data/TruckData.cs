using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/TruckData", fileName = "TruckData")]
public class TruckData : EnemyData<TruckEnemyConfig>
{
    public ThrowerData thrower;
    public ThrowerData thrower2;

    private TruckEnemyConfig _config;
    private ThrowerData _guy;

    private float _x;

    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);

        _config = (TruckEnemyConfig) behaviour.emitterContainer;

        behaviour.AddRenderer(_config.backPieceRenderer);
        behaviour.AddRenderer(_config.laserTelegraphRoot);
        behaviour.SetFrame(2, _config.backPieceRenderer);
        _x = -behaviour.transform.position.x;
        behaviour.SetFrame(0);
    }

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (behaviour.dead)
        {
            return;
        }

        if (behaviour.state == 0)
        {
            if (behaviour.stateTimer < 3.5f)
            {
                behaviour.speed = Vector2.up * 1.5f;
            }
            else if (behaviour.stateTimer < 4f)
            {
                behaviour.speed = Vector2.zero;
            }
            else
            {
                behaviour.speed = Vector2.up * (1 - (behaviour.substateTimer - 4)) * 16;
                if (behaviour.speed.magnitude < 1)
                {
                    behaviour.speed = Vector2.zero;
                    behaviour.SwitchState(1);
                    behaviour.stateTimer = -8;
                }
            }
        }
        else if (behaviour.state == 1)
        {
            if (behaviour.HasSurpassedState(0.45f, 0.45f))
            {
                Shoot(behaviour, 1, 0);
                Shoot(behaviour, 2, 0);
                Shoot(behaviour, 3, 0);
                Shoot(behaviour, 4, 0);
            }

            switch(behaviour.substate)
            {
                case 0:
                    behaviour.speed = Vector2.up;
                    if (behaviour.HasSurpassedSubstate(15))
                    {
                        behaviour.SwitchSubstate(1);
                    }
                    break;
                case 1:
                case 3:
                    behaviour.speed = Vector2.zero;
                    if (behaviour.HasSurpassedSubstate(3))
                    {
                        behaviour.SwitchSubstate((behaviour.substate + 1) % 4);
                    }
                    break;
                case 2:
                    behaviour.speed = Vector2.down;
                    if (behaviour.HasSurpassedSubstate(15))
                    {
                        behaviour.SwitchSubstate(3);
                    }
                    break;
            }
        } else if (behaviour.state == 2)
        {
            switch (behaviour.substate)
            {
                case 0:
                    if (behaviour.transform.position.y < 27)
                    {
                        behaviour.speed.y += 10 * Time.deltaTime;
                    } else
                    {
                        behaviour.speed.y = 0;
                        behaviour.transform.position = new Vector3(0, behaviour.transform.position.y, behaviour.transform.position.z);
                        behaviour.SetFrame(1, _config.backPieceRenderer);
                        _guy = (ThrowerData) References.CreateObject<EnemyBehaviour>(thrower, _config.tntThrowerRoot.position).GetData();
                        _guy.SetParent(_config.tntThrowerRoot);
                        behaviour.SwitchSubstate(1);
                    }
                    break;
                case 1:
                    if (behaviour.transform.position.y > 15)
                    {
                        behaviour.speed.y = -1;
                    } else
                    {
                        behaviour.speed.y = 0;
                    }
                    break;
            }
        }
        else if (behaviour.state == 3)
        {
            if (behaviour.stateTimer >= 0) {
                if (behaviour.HasSurpassedState(0.45f, 0.45f))
                {
                    Shoot(behaviour, 10, 0);
                    Shoot(behaviour, 11, 0);
                    Shoot(behaviour, 12, 0);
                    Shoot(behaviour, 13, 0);
                }
            }

            switch (behaviour.substate)
            {
                case 0:
                    behaviour.speed = Vector2.down;
                    if (behaviour.stateTimer < 0)
                    {
                        behaviour.speed *= 4.5f;
                    }
                    if (behaviour.HasSurpassedSubstate(15))
                    {
                        behaviour.SwitchSubstate(1);
                    }
                    break;
                case 1:
                case 3:
                    behaviour.speed = Vector2.zero;
                    if (behaviour.HasSurpassedSubstate(3))
                    {
                        behaviour.SwitchSubstate((behaviour.substate + 1) % 4);
                    }
                    break;
                case 2:
                    behaviour.speed = Vector2.up;
                    if (behaviour.HasSurpassedSubstate(15))
                    {
                        behaviour.SwitchSubstate(3);
                    }
                    break;
            }

            if (behaviour.transform.position.x > _x)
            {
                behaviour.speed.x = -2;
            }
        }
        else if (behaviour.state == 4)
        {

            switch (behaviour.substate)
            {
                case 0:
                    if (behaviour.transform.position.y < 27)
                    {
                        behaviour.speed.y += 10 * Time.deltaTime;
                    }
                    else
                    {
                        behaviour.speed.y = 0;
                        behaviour.transform.position = new Vector3(0, behaviour.transform.position.y, behaviour.transform.position.z);
                        behaviour.SetFrame(1, _config.backPieceRenderer);
                        _guy = (ThrowerData)References.CreateObject<EnemyBehaviour>(thrower, _config.tntThrowerRoot.position).GetData();
                        _guy.SetParent(_config.tntThrowerRoot);
                        behaviour.SwitchSubstate(1);
                    }
                    break;
                case 1:
                    if (behaviour.transform.position.y > 6)
                    {
                        behaviour.speed.y = -2;
                    }
                    else
                    {
                        behaviour.speed.y = 0;
                    }
                    break;
            }
            if (behaviour.HasSurpassedState(0.5f, 0.5f))
            {
                Shoot(behaviour, 4, 0);
                Shoot(behaviour, 13, 0);
            }
        }
        else if (behaviour.state == 5)
        {
            if (behaviour.transform.position.y > 0)
            {
                behaviour.speed.y = -2;
            }
            else
            {
                behaviour.speed.y = 0;
            }
            if (behaviour.HasSurpassedState(3))
            {
                behaviour.SetFrame(3, _config.laserTelegraphRoot);
            }

            if (behaviour.HasSurpassedState(4))
            {
                _config.laserLoop.Play();
                behaviour.SetFrame(3, _config.laserTelegraphRoot);
                Shoot(behaviour, 0, 0);
            }

            if (behaviour.HasSurpassedState(20))
            {
                behaviour.stateTimer = 3.5f;
            }

            if (behaviour.stateTimer < 4)
            {
                if (behaviour.HasSurpassedState(0.3f, 0.3f))
                {
                    Shoot(behaviour, 5, 0);
                    Shoot(behaviour, 6, 0);
                    Shoot(behaviour, 7, 0);
                    Shoot(behaviour, 8, 0);
                    Shoot(behaviour, 9, 0);
                }
            }
        }



        base.OnUpdate(behaviour);
    }

    public override void OnPhaseTransition(EnemyBehaviour behaviour)
    {
        behaviour.speed = Vector2.zero;
        AbortRoutines(behaviour);
        behaviour.TiltSprite(0);
        if (_guy != null)
        {
            _guy.Kill();
        }
        behaviour.SwitchState(behaviour.state + 1);
        if (behaviour.state == 3)
        {
            behaviour.stateTimer = -4;
            behaviour.SetFrame(2, _config.backPieceRenderer);
        }

        if (behaviour.state == 5)
        {
            Instantiate(deathEffect, behaviour.transform.position, Quaternion.identity);
            _config.backPieceRoot.SetParent(null);
            _config.backPieceRenderer.DOKill();
            _config.backPieceRenderer.color = new Color(1,1,1,0.5f);
            _config.backPieceRenderer.DOFade(0, 3);
            behaviour.RemoveRenderer(_config.backPieceRenderer);
            var rb = _config.backPieceRoot.gameObject.AddComponent<Rigidbody2D>();
            behaviour.StartCoroutine(KillBackpiece());
            rb.velocity = Vector2.one * 6;
            behaviour.stateTimer = -3;
            behaviour.SetFrame(2, _config.backPieceRenderer);
        }
        base.OnPhaseTransition(behaviour);
    }

    private IEnumerator KillBackpiece()
    {
        yield return new WaitForSeconds(3);
        Destroy(_config.backPieceRoot.gameObject);
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        behaviour.speed = new Vector2(Random.Range(-1.0f, 1.0f), 0.2f);
        yield return base.OnKill(behaviour);
    }
}
