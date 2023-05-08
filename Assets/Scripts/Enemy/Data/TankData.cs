using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/TankData", fileName = "TankData")]
public class TankData : EnemyData<EnemyConfig>
{
    public bool skipIntroState = false;
    public float introMultiplier = 1;
    public float shotInterval = 1;
    public AnimationCurve speedXTime = AnimationCurve.Constant(0, 1, 1);
    public AnimationCurve speedYTime = AnimationCurve.Constant(0, 1, 1);
    public Vector2 speedTimeMultiplier;
    public List<int> SpritesheetAnimationIndices = new List<int>();
    public bool IgnoreOffscreenCheck = false;

    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);

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

        if (behaviour.substate == 0)
        {
            behaviour.speed = Vector2.left * (1 - behaviour.stateTimer) * 6 * introMultiplier;
            if (skipIntroState || behaviour.speed.magnitude < 1.5f)
            {
                behaviour.speed = speed;
                behaviour.startSpeed = speed;
                behaviour.SwitchSubstate(1);
                behaviour.substateTimer = shotInterval;
            }
        } else if (behaviour.substate == 1)
        {
            behaviour.speed = behaviour.startSpeed * new Vector2(speedXTime.Evaluate((speedTimeMultiplier.x * behaviour.stateTimer) % 1), speedYTime.Evaluate((speedTimeMultiplier.y * behaviour.stateTimer) % 1));
            if (behaviour.substateTimer >= shotInterval && shotInterval > 0)
            {
                if (IgnoreOffscreenCheck || MaskManager.IsInBounds(behaviour.transform.position))
                {
                    Shoot(behaviour);
                }
                behaviour.substateTimer -= shotInterval;
            }
        }

        SpritesheetAnimation(behaviour, SpritesheetAnimationIndices, 6);
        

        base.OnUpdate(behaviour);
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        behaviour.speed = new Vector2(Random.Range(-1.0f, 1.0f), 0.2f);
        yield return base.OnKill(behaviour);
    }
}
