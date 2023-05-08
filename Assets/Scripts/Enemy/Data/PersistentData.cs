using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : EnemyData<EnemyConfig>
{
    public int ShootInterval = 1;

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
        foreach (var col in behaviour.gameObject.GetComponentsInChildren<Collider2D>())
        {
            col.gameObject.layer = 10;
        }
    }

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        base.OnUpdate(behaviour);

        if (MaskManager.IsInBounds(behaviour.transform.position))
        {
            if (behaviour.HasSurpassedState(ShootInterval, ShootInterval))
            {
                Shoot(behaviour);
            }
        } else
        {
            behaviour.stateTimer -= Manager.deltaTime;
            behaviour.substateTimer -= Manager.deltaTime;
            behaviour.animationTimer -= Manager.deltaTime;
            behaviour.ForceInvulnerable(0.1f);
        }
    }
}
