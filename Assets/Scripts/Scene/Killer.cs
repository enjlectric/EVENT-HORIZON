using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            return;
        }
        if (collision.gameObject.GetComponentInParent<PoolableObject>() is PoolableObject obj)
        {
            if (obj == Manager.instance.player)
            {
                return;
            }

            if (obj is BulletBehaviour b && b.data.isLaser)
            {
                return;
            }
            References.DestroyObject(obj);
        } else
        {
            Destroy(collision.gameObject);
        }
    }
}
