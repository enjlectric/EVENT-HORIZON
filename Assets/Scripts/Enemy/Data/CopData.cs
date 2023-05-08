using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Enemy/CopData", fileName = "CopData")]
public class CopData : EnemyData<EnemyConfig>
{
    public float patternDuration = 1;
    public float swerve = 0;
    public float sinkSpeed = 1.5f;

    private List<int> animationSequence = new List<int>() {0,1,2,3};

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (behaviour.dead)
        {
            return;
        }

        if (behaviour.substate == 0)
        {
            behaviour.speed = Vector2.down * (1 - behaviour.substateTimer * 0.5f) * 6;
            if (behaviour.speed.magnitude < sinkSpeed)
            {
                behaviour.speed = Vector2.down * sinkSpeed;
                behaviour.substateTimer = patternDuration;
                behaviour.substate = 1;
                behaviour.SetFrame(1);
            }
        }
        else
        {
            if (behaviour.substateTimer >= patternDuration)
            {
                Shoot(behaviour);
                behaviour.substateTimer = 0;
            }
        }

        if (swerve > 0)
        {
            behaviour.speed.x = Mathf.Cos(Mathf.Deg2Rad * (behaviour.stateTimer / patternDuration * swerve) * 360) * 9;
            behaviour.TiltSprite(behaviour.speed.x * -5f);
        }

        SpritesheetAnimation(behaviour, animationSequence, 4);
        base.OnUpdate(behaviour);
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        behaviour.SetFrame(2);
        behaviour.speed = new Vector2(Random.Range(-1.0f, 1.0f), 0.2f);
        yield return base.OnKill(behaviour);
    }
}
