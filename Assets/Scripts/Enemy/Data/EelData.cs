using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/EelData", fileName = "EelData")]
public class EelData : TankData
{
    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);
    }

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (behaviour.dead)
        {
            return;
        }

        if (shotInterval > 0 && (behaviour.substate == 0 || behaviour.HasSurpassedSubstate(shotInterval)))
        {
            behaviour.SetFrame(1);
        } else if (shotInterval > 0f && behaviour.HasSurpassedSubstate(5f))
        {
            behaviour.SetFrame(0);
        }

        behaviour.TiltSprite(Mathf.Sin(behaviour.stateTimer * 0.75f) * 35f, behaviour.transform);

        base.OnUpdate(behaviour);
    }
}
