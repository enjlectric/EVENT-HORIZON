using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/RacerData", fileName = "RacerData")]
public class RacerData : TankData
{
    public float headRotationPerSecond = 0;
    public float headRotationStart = 0;

    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);
    }
    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        var config = (RacerEnemyConfig)behaviour.emitterContainer;
        if (headRotationPerSecond == 0)
        {
            config.head.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector3.up, (Manager.instance.player.transform.position - config.head.transform.position).normalized));
        } else
        {
            config.head.localEulerAngles = new Vector3(0, 0, headRotationStart + behaviour.stateTimer * headRotationPerSecond);
        }

        base.OnUpdate(behaviour);

        behaviour.TiltSprite(behaviour.speed.x * -3f);
    }
}
