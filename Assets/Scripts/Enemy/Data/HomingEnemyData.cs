using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/HomingEnemyData", fileName = "HomingEnemyData")]
public class HomingEnemyData : EnemyData<EnemyConfig>
{
    public float HomeSnappiness = 1;
    public AnimationCurve speedCurve = AnimationCurve.Constant(0, 1, 1);
    public float speedTimeMultiplier;
    public List<int> SpritesheetAnimationIndices = new List<int>();
    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);
        behaviour.speed = Vector2.Lerp(behaviour.transform.up, (Manager.instance.player.transform.position - behaviour.transform.position).normalized, HomeSnappiness) * speed.magnitude;
        behaviour.transform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(behaviour.speed.y, behaviour.speed.x) * Mathf.Rad2Deg - 90);
    }

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        base.OnUpdate(behaviour);

        behaviour.speed = speedCurve.Evaluate((speedTimeMultiplier * behaviour.stateTimer) % 1) * behaviour.startSpeed;

        SpritesheetAnimation(behaviour, SpritesheetAnimationIndices, 6);
    }
}
