using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootableData : PoolableData
{
    public bool shootable;
    public bool takesContactDamage;
    public bool isSolid;
    public float ownContactDamage;
    public float damage;
    public float health;
    public float armor;
    public float invulnerableTime;
    
    [MinMaxSlider(0, 20, true)]
    public Vector2 lifetime;
}
