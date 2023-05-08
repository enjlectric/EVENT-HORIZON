using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : PoolableObject<PickupData>
{
    public SpritesheetAnimation anim;
    public ParticleSystem collectionEffect;

    public override void Initialize()
    {
        anim.sprites = data.sprites;
        _speed = Quaternion.Euler(0, 0, Random.Range(25, 100)) * Vector2.up * Random.Range(3f, 7f);
    }

    public override void OnUpdate()
    {
        if ((Manager.instance.player.transform.position - transform.position).magnitude < 1)
        {
            References.DestroyObject(this);
            data.GrantPlayer();
            var sys = Instantiate(collectionEffect, null);
            sys.transform.position = transform.position;
        }
        _speed += Vector2.down * Time.deltaTime * 2;

        base.OnUpdate();
    }
}
