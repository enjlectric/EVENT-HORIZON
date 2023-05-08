using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireTracks : PoolableObject<TireTracksData>
{
    public SpriteRenderer spriteRenderer;

    public override PoolableData GetData()
    {
        return null;
    }

    public override void Initialize()
    {
        spriteRenderer.sprite = data.sprite;
    }

    public override void OnUpdate()
    {
        transform.position -= Vector3.up * Manager.instance.GameSpeed * Manager.deltaTime;

        if (transform.position.y < -20)
        {
            References.DestroyObject(this);
        }
    }
}
