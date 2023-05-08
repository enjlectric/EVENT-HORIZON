using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/BossCopData", fileName = "BossCopData")]
public class BossCopData : EnemyData<EnemyConfig>
{
    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (behaviour.dead)
        {
            return;
        }

        if (behaviour.state == 0)
        {
            if (behaviour.substateTimer < 3.5f)
            {
                behaviour.speed = Vector2.up * 1.5f;
            } else if (behaviour.substateTimer < 4f)
            {
                behaviour.speed = Vector2.zero;
            } else
            {
                behaviour.speed = Vector2.up * (1 - (behaviour.substateTimer - 4)) * 42;
                if (behaviour.speed.magnitude < 1.5f)
                {
                    behaviour.speed = Vector2.zero;
                    behaviour.SwitchState(1);
                    behaviour.stateTimer = 0;
                }
            }
        }
        else if (behaviour.state == 1)
        {
            if (behaviour.HasSurpassedSubstate(1.5f, 1.5f))
            {
                Shoot(behaviour, 0, 1);
            }

            if (behaviour.HasSurpassedState(3))
            {
                Shoot(behaviour, 0, 0);
                behaviour.stateTimer = 0;
            }


            float aimDir = Mathf.Sign(Manager.instance.player.transform.position.x - behaviour.transform.position.x);
            float mult = 1;

            behaviour.TiltSprite(behaviour.speed.x * -0.5f);

            if (aimDir != Mathf.Sign(behaviour.speed.x))
            {
                mult = 1.5f;
            }
            behaviour.speed.x += Mathf.Clamp(aimDir * Time.deltaTime * 15 * mult, -1, 1);

            behaviour.KeepOnScreen();
        }
        else if (behaviour.state == 2)
        {
            if (behaviour.HasSurpassedState(3))
            {
                Shoot(behaviour, 1, 2);
                Shoot(behaviour, 2, 2);
                Shoot(behaviour, 3, 2);
                Shoot(behaviour, 4, 2);
                Shoot(behaviour, 0, 0);
                behaviour.stateTimer = 0;
            }


            float aimDir = Mathf.Sign(Manager.instance.player.transform.position.x - behaviour.transform.position.x);
            float mult = 1;

            behaviour.TiltSprite(behaviour.speed.x * -0.5f);

            if (aimDir != Mathf.Sign(behaviour.speed.x))
            {
                mult = 1.5f;
            }
            behaviour.speed.x += Mathf.Clamp(aimDir * Time.deltaTime * 15 * mult, -1, 1);

            behaviour.KeepOnScreen();
        }
        else if (behaviour.state == 3)
        {
            if (behaviour.HasSurpassedSubstate(1f, 1f))
            {
                Shoot(behaviour, 1, 0);
                Shoot(behaviour, 2, 0);
                Shoot(behaviour, 3, 0);
                Shoot(behaviour, 4, 0);
            }

            if (behaviour.HasSurpassedState(3))
            {
                Shoot(behaviour, 0, 2);
                Shoot(behaviour, 0, 3);
                behaviour.stateTimer = 0;
            }

            behaviour.speed = -behaviour.transform.position;
            if (behaviour.speed.magnitude > 1)
            {
                behaviour.speed.Normalize();
            }

            behaviour.KeepOnScreen();

            base.OnUpdate(behaviour);
        }
        else if (behaviour.state == 4)
        {
            if (behaviour.HasSurpassedSubstate(0.75f, 0.75f))
            {
                Shoot(behaviour, 1, 1);
                Shoot(behaviour, 2, 1);
                Shoot(behaviour, 3, 1);
                Shoot(behaviour, 4, 1);
            }

            if (behaviour.HasSurpassedState(1, 4))
            {
                Shoot(behaviour, 0, 4);
                Shoot(behaviour, 0, 5);
            }

            if (behaviour.HasSurpassedState(4))
            {
                behaviour.stateTimer = 0;
            }

            behaviour.speed = -behaviour.transform.position;
            if (behaviour.speed.magnitude > 1)
            {
                behaviour.speed.Normalize();
            }

            behaviour.KeepOnScreen();

            base.OnUpdate(behaviour);
        }
    }

    public override void OnPhaseTransition(EnemyBehaviour behaviour)
    {
        behaviour.speed = Vector2.zero;
        AbortRoutines(behaviour);
        behaviour.TiltSprite(0);
        behaviour.SwitchState(behaviour.state + 1);
        base.OnPhaseTransition(behaviour);
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        //behaviour.SetFrame(2);
        behaviour.speed = new Vector2(Random.Range(-1.0f, 1.0f), 0.2f);
        yield return base.OnKill(behaviour);
    }
}
