using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/SquareData", fileName = "RacerData")]
public class SquareData : TankData
{
    public EnemyData onDeathSpawn;

    public Vector4 EdgeOffsets;
    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);
        behaviour.speed = speed;
        behaviour.substate = 99;
    }

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (behaviour.dead)
        {
            return;
        }

        if (behaviour.substate == 99)
        {
            Vector2 pos = (Vector2)behaviour.transform.position + behaviour.speed.normalized;
            if (behaviour.speed.x > 0)
            {
                pos.x = pos.x + EdgeOffsets.z;
            } else
            {
                pos.x = pos.x - EdgeOffsets.x;
            }

            if (behaviour.speed.y > 0)
            {
                pos.y = pos.y + EdgeOffsets.w;
            } else
            {
                pos.y = pos.y - EdgeOffsets.y;
            }

            if (!MaskManager.IsInBoundsH(pos) && MaskManager.IsInBoundsH(behaviour.transform.position))
            {
                behaviour.speed.x = -behaviour.speed.x;
                behaviour.SwitchSubstate(98);
                Shoot(behaviour);
            }
            if (!MaskManager.IsInBoundsV(pos) && MaskManager.IsInBoundsV(behaviour.transform.position))
            {
                behaviour.speed.y = -behaviour.speed.y;
                behaviour.SwitchSubstate(98);
                Shoot(behaviour);
            }
        } else if (behaviour.substate == 98)
        {
            behaviour.transform.position -= new Vector3(behaviour.speed.x, behaviour.speed.y)* Time.deltaTime;
            if (behaviour.substateTimer >= 1)
            {
                behaviour.SwitchSubstate(99);
            }
        }

        base.OnUpdate(behaviour);
    }
    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        for (int i = 0; i < 4; i++)
        {
            EnemyBehaviour b1 = References.CreateObject<EnemyBehaviour>(onDeathSpawn, behaviour.transform.position, Quaternion.Euler(0, 0, 90 * i));
            b1.speed = Vector2.one * 4;
            b1.speedMultiplier.x *= (i >= 1 && i <= 2) ? -1 : 1;
            b1.speedMultiplier.y *= Mathf.FloorToInt(i/2) == 0 ? 1 : -1;
        }
        yield return base.OnKill(behaviour);
    }
}
