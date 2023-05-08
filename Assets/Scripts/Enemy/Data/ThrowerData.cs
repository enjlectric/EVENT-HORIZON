using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/ThrowerData", fileName = "ThrowerData")]
public class ThrowerData : EnemyData<EnemyConfig>
{
    public float waitTime = 2;
    public float holdWaitTime = 0.5f;

    private EnemyBehaviour _behaviour;

    public override void OnStart(EnemyBehaviour behaviour)
    {
        _behaviour = behaviour;
        base.OnStart(behaviour);
        _behaviour.SetFrame(0);
        behaviour.spriteRenderer.sortingOrder = 4;
    }

    public void ForceStateThrowing()
    {
        _behaviour.SwitchState(1);
    }

    public void Kill()
    {
        if (_behaviour != null)
        {
            _behaviour.Kill();
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
            if (behaviour.HasSurpassedState(holdWaitTime))
            {
                behaviour.SwitchState(1);
            }
        }
        else
        {
            switch (behaviour.substate)
            {
                case 0:
                    if (behaviour.HasSurpassedSubstate(holdWaitTime)) // Reach
                    {
                        _behaviour.SetFrame(3);
                        Shoot(behaviour);
                        behaviour.SwitchSubstate(1);
                    }
                    break;
                case 1:
                    if (behaviour.HasSurpassedSubstate(0.2f)) // Hold
                    {
                        _behaviour.SetFrame(3);
                        behaviour.SwitchSubstate(2);
                    }
                    break;
                case 2:
                    if (behaviour.HasSurpassedSubstate(0.6f)) // Throw
                    {
                        _behaviour.SetFrame(1);
                        behaviour.SwitchSubstate(3);
                    }
                    break;
                case 3:
                    if (behaviour.HasSurpassedSubstate(waitTime)) // Throw
                    {
                        _behaviour.SetFrame(2);
                        behaviour.SwitchSubstate(4);
                    }
                    break;
                case 4:
                    if (behaviour.HasSurpassedSubstate(0.2f)) // Throw
                    {
                        _behaviour.SetFrame(0);
                        behaviour.SwitchState(1);
                    }
                    break;
            }
        }


        base.OnUpdate(behaviour);
    }

    public void SetParent(Transform t)
    {
        _behaviour.transform.SetParent(t);
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        behaviour.SetFrame(5);
        behaviour.speed = new Vector2(Random.Range(-1.0f, 1.0f), 0.2f);
        yield return base.OnKill(behaviour);
    }
}
