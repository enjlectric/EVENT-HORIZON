using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/TriangleData", fileName = "TriangleData")]
public class TriangleData : EnemyData<TriangleConfig>
{
    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);
        behaviour.speed = Vector3.zero;
        TriangleConfig ec = behaviour.emitterContainer as TriangleConfig;
        ec.TargetPosition = MaskManager.GetPositionRelativeToCam(Random.Range(0.7f, 0.9f), Random.Range(-0.95f, 0.95f));
    }

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (behaviour.dead)
        {
            return;
        }

        TriangleConfig ec = behaviour.emitterContainer as TriangleConfig;

        if (behaviour.substate == 0)
        {
            behaviour.transform.position = Vector3.Lerp(behaviour.transform.position, ec.TargetPosition, behaviour.substateTimer);
            if (behaviour.HasSurpassedSubstate(1f))
            {
                behaviour.SwitchSubstate(1);
                Shoot(behaviour);
            }
        }
        else if (behaviour.substate == 1)
        {
            if (behaviour.substateTimer >= 1)
            {
                behaviour.SwitchSubstate(0);
                ec.TargetPosition = MaskManager.GetPositionRelativeToCam(Random.Range(0.75f, 0.9f), Random.Range(-0.75f, 0.75f));
            }
        }

        base.OnUpdate(behaviour);
    }
}