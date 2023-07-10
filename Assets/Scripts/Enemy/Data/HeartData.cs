using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "Game/Enemy/HeartData", fileName = "HeartData")]
public class HeartData : EnemyData<EnemyConfig>
{
    public BulletData BloodBullet;
    public BulletData RedBloodCell;
    public BulletData WhiteBloodCell;

    private SpriteRenderer _configSprite;
    public Enjlectric.ScriptableData.Types.ScriptableDataInt BackgroundEvent;

    private ChangeSpeed _changeSpeed;
    private float basicX;

    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);
        _changeSpeed = FindObjectOfType<ChangeSpeed>();
        foreach (var ren in behaviour.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            if (ren != behaviour.spriteRenderer && ren != behaviour.HealthBarRenderer)
            {
                behaviour.AddRenderer(ren);
                _configSprite = ren;
            }
        }
    }

    private void SpawnCells(BulletData prefab, EnemyBehaviour behaviour, float interval, bool reverse)
    {
        var min = -0.95f;
        var maxMult = 1;
        var iver = 0.1f;
        if (reverse)
        {
            min = 0.95f;
            maxMult = -1;
            iver = -0.1f;
        }

        var x = 1;
        for (float i = min; i * maxMult <= 0.96f; i += iver)
        {
            if (behaviour.HasSurpassedState(x * interval, interval * 20))
            {
                var position = MaskManager.GetPositionRelativeToCam(1.15f, i);
                var bullet = References.CreateObject<BulletBehaviour>(prefab, position, Quaternion.Euler(0, 0, 90));
                bullet.transform.SetParent(_changeSpeed.transform);
            }
            x++;
        }
    }

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        base.OnUpdate(behaviour);

        if (behaviour.state == 0)
        {
            behaviour.ForceInvulnerable(0.5f);
            if (MaskManager.IsInBounds(behaviour.transform.position + Vector3.right * 3.3f))
            {
                basicX = behaviour.transform.position.x;
                behaviour.SwitchState(1);
            }
        }
        else if (behaviour.state == 1)
        {
            SpawnCells(RedBloodCell, behaviour, 0.25f, false);
            if (behaviour.substate >= 1)
            {
                SpawnCells(RedBloodCell, behaviour, 0.326f, true);
            }
            if (behaviour.substate >= 2)
            {
                SpawnCells(WhiteBloodCell, behaviour, 1.35f, false);
            }
            if (behaviour.substate >= 3)
            {
                if (behaviour.HasSurpassedSubstate(7, 7))
                {
                    var angle = (Mathf.Floor(behaviour.substateTimer / 4.0f) % 5) * 15;
                    behaviour.emitterContainer.emitters[1].transform.localEulerAngles = new Vector3(0, 0, 60 + angle);
                    Shoot(behaviour, 1);
                }
            }

            if (behaviour.substateTimer > 0.25f)
            {
                if (behaviour.transform.position.x > basicX)
                {
                    _changeSpeed.ChangeXAdd(-Manager.deltaTime * 2f, -2);
                }
                else
                {
                    _changeSpeed.ChangeX(0);
                }
            }
        }
    }

    public override void OnPhaseTransition(EnemyBehaviour behaviour)
    {
        base.OnPhaseTransition(behaviour);

        SFX.ExplosionMiniboss.Play();
        behaviour.SwitchSubstate(behaviour.substate += 1);
        _changeSpeed.ChangeX(8);
    }

    public override void OnHurt(EnemyBehaviour behaviour)
    {
        base.OnHurt(behaviour);

        _configSprite.transform.DOComplete();
        _configSprite.transform.DOPunchScale(0.05f * Vector2.up, 0.15f).SetEase(Ease.OutBack);
        for (int i = 1; i <= 2; i++)
        {
            var offset = Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector3.up * Random.Range(0.1f, 1);
            var bullet = References.CreateObject<BulletBehaviour>(BloodBullet, behaviour.emitterContainer.emitters[0].transform.position + offset, Quaternion.Euler(0, 0, 45 * Random.Range(-1, 1.0f)));
            bullet.speedMod = Random.Range(0.6f, 1.3f);
            bullet.transform.SetParent(_changeSpeed.transform);
        }
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        AudioManager.ChangeMusic(null, 8);
        BackgroundEvent.Value++;
        References.DestroyObjectsOfType<BulletBehaviour>();
        return base.OnKill(behaviour);
    }
}