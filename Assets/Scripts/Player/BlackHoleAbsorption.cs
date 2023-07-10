using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlackHoleAbsorption : MonoBehaviour
{
    public Enjlectric.ScriptableData.Types.ScriptableDataFloat LimitMeter;
    public Enjlectric.ScriptableData.Types.ScriptableDataFloat LimitMeterDamage;

    public ParticleSystem AbsorptionSystemPrefab;

    public bool IsEnemy = false;

    private EnemyBehaviour _behaviour;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsEnemy)
        {
            if ((collision.gameObject.layer == 9))
            {
                if (collision.GetComponent<BulletBehaviour>() is BulletBehaviour b && !b.data.isLaser)
                {
                    b.Kill();
                    if (_behaviour == null)
                    {
                        _behaviour = GetComponentInParent<EnemyBehaviour>();
                    }
                    SFX.PlayerBlackHoleAbsorb.Play();
                    var sys = Instantiate(AbsorptionSystemPrefab, null);
                    sys.transform.position = collision.transform.position;
                }
            }
            return;
        }
        if (collision.GetComponent<Pickup>() is Pickup p)
        {
            LimitMeterDamage.Value += 0.05f;
            p.data.GrantPlayer();
            var sys = Instantiate(AbsorptionSystemPrefab, transform);
            sys.transform.position = collision.transform.position;
        }

        if ((collision.gameObject.layer == 8 || collision.gameObject.layer == 7))
        {
            if (collision.GetComponent<BulletBehaviour>() is BulletBehaviour b && !b.data.isLaser)
            {
                if (b.data.damage == 0)
                {
                    LimitMeterDamage.Value += 0.2f;
                } else
                {
                    LimitMeterDamage.Value += b.data.damage * 0.1f;
                }

                var val = Mathf.Clamp(Mathf.FloorToInt(LimitMeterDamage.Value * 4), 0, 3);
                switch (val)
                {
                    case 3:
                        SFX.PlayerBlackHoleAbsorbStage4.Play();
                        break;
                    case 2:
                        SFX.PlayerBlackHoleAbsorbStage3.Play();
                        break;
                    case 1:
                        SFX.PlayerBlackHoleAbsorbStage2.Play();
                        break;
                    case 0:
                        SFX.PlayerBlackHoleAbsorb.Play();
                        break;
                }
                b.Kill();
                var sys = Instantiate(AbsorptionSystemPrefab, null);
                sys.transform.position = collision.transform.position;
            }
        }
    }
}
