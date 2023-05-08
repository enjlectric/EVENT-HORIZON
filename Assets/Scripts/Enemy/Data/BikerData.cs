using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/BikerData", fileName = "BikerData")]
public class BikerData : EnemyData<EnemyConfig>
{
    public float yeehawDuration;
    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (behaviour.dead)
        {
            return;
        }

        if (behaviour.state == 0)
        {
            behaviour.speed = Vector2.up * (1 - behaviour.stateTimer * 0.5f) * 25;
            if (behaviour.speed.magnitude < 1.5f)
            {
                behaviour.speed = Vector2.zero;
                behaviour.SwitchState(1);
                behaviour.stateTimer = 0;
                behaviour.SetFrame(1);
            }
        }
        else if (behaviour.state == 1)
        {
            float aimDir = Mathf.Sign(Manager.instance.player.transform.position.x - behaviour.transform.position.x);
            float mult = 1;
            
            if (aimDir != Mathf.Sign(behaviour.speed.x))
            {
                mult = 1.5f;
            }
            behaviour.speed.x += Mathf.Clamp(aimDir * Time.deltaTime * 15 * mult, -3, 3);

            if (behaviour.substate == 0 && behaviour.substateTimer >= 3)
            {
                behaviour.SwitchSubstate(1);
                behaviour.SetFrame(2);
            } else if (behaviour.substate == 1) {
                if (behaviour.substateTimer >= 0.5f)
                {
                    Shoot(behaviour);
                    behaviour.SwitchSubstate(2);
                }
            } else if (behaviour.substate == 2 && behaviour.substateTimer >= yeehawDuration)
            {
                behaviour.SwitchSubstate(0);
                behaviour.SetFrame(1);
            }

            behaviour.KeepOnScreen();
        }

        behaviour.TiltSprite(behaviour.speed.x * -5);


        base.OnUpdate(behaviour);
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        behaviour.SetFrame(2);
        behaviour.speed = new Vector2(Random.Range(-1.0f, 1.0f), 0.2f);
        yield return base.OnKill(behaviour);
    }
}
