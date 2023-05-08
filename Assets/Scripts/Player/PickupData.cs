using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickupData : PoolableData
{
    public float value;
    public List<Sprite> sprites;

    public abstract void GrantPlayer();
}
