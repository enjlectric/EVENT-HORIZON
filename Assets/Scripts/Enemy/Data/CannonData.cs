using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/CannonData", fileName = "CannonData")]
public class CannonData : PersistentData
{
    public Vector2 AngleRange;
    public bool Invulnerable;
    private float ModularClamp(float val, float min, float max, float rangemin = 0f, float rangemax = 360f)
    {
        var modulus = Mathf.Abs(rangemax - rangemin);
        if ((val %= modulus) < 0f) val += modulus;
        return Mathf.Clamp(val + Mathf.Min(rangemin, rangemax), min, max);
    }

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (Invulnerable)
        {
            behaviour.ForceInvulnerable(1);
        }
        base.OnUpdate(behaviour);
        if (behaviour.stateTimer % ShootInterval > 1f)
        {
            if (AngleRange.x < 0 && AngleRange.y > 0)
            {
                CannonConfig cfg = (CannonConfig)behaviour.emitterContainer;
                var vec = Vector2.Lerp(cfg.Cannon.transform.up, (Manager.instance.player.transform.position - behaviour.transform.position).normalized, Time.deltaTime * 3);
                cfg.Cannon.transform.localEulerAngles = Vector3.forward * Mathf.Clamp(Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg - 90, AngleRange.x, AngleRange.y);
            } else
            {
                CannonConfig cfg = (CannonConfig)behaviour.emitterContainer;
                var vec = Vector2.Lerp(cfg.Cannon.transform.up, (Manager.instance.player.transform.position - behaviour.transform.position).normalized, Time.deltaTime * 3);
                cfg.Cannon.transform.localEulerAngles = Vector3.forward * ModularClamp(Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg - 90, AngleRange.x, AngleRange.y);
            }
        }
    }
}
