using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/HandData", fileName = "HandData")]
public class HandData : TankData
{
    public override void OnHurt(EnemyBehaviour behaviour)
    {
        base.OnHurt(behaviour);
        SFX.EnemyShieldRetaliate.Play();
        Shoot(behaviour, 0, 0);
    }
}
