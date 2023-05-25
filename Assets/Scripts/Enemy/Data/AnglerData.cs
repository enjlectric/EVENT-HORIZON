using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/AnglerData", fileName = "AnglerData")]
public class AnglerData : EnemyData<EnemyConfig>
{
    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);
        behaviour.spriteRenderer.sortingLayerName = "EnemyForeground";

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

        if (behaviour.state == 0)
        {
            behaviour.speed = Vector2.left * 3.1f;
            if (behaviour.HasSurpassedState(3))
            {
                behaviour.SwitchState(1);
            }
        } else if (behaviour.state == 1)
        {
            behaviour.speed = new Vector2(0, Mathf.Sin(behaviour.stateTimer * 2) * 1.35f - behaviour.transform.position.y);

            if (behaviour.HasSurpassedSubstate(2))
            {
                Shoot(behaviour, 0, 0);
                Shoot(behaviour, 1, 0);
                Shoot(behaviour, 2, 0);
            }

            if (behaviour.HasSurpassedSubstate(0.4f, 0.4f))
            {
                Shoot(behaviour, 3, 0);
            }

            if (behaviour.HasSurpassedSubstate(6))
            {
                behaviour.substateTimer = 0;
            }
        }
        else if (behaviour.state == 2)
        {
            behaviour.speed = new Vector2(Mathf.Max(behaviour.speed.x - 5 * Manager.deltaTime, -14), 0);

            if (behaviour.transform.position.x <= -15)
            {
                var pos = behaviour.transform.position;
                pos.x = MaskManager.GetPositionRelativeToCam(1.4f, 0).x;
                pos.y = Manager.instance.player.transform.position.y;
                behaviour.transform.position = pos;
                Shoot(behaviour, 3, 0);
            }
        }

        base.OnUpdate(behaviour);
    }

    public override void OnPhaseTransition(EnemyBehaviour behaviour)
    {
        SFX.ExplosionMiniboss.Play();
        References.DestroyObjectsOfType<BulletBehaviour>();
        behaviour.speed = 6 * Vector2.right;
        behaviour.SwitchState(2);

        AbortRoutines(behaviour);
        base.OnPhaseTransition(behaviour);
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        behaviour.speed = new Vector2(Random.Range(-1.0f, 1.0f), 0.2f);
        yield return base.OnKill(behaviour);
    }
}
