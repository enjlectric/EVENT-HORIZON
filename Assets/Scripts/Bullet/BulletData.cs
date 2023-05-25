using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/BulletData", fileName = "BulletData")]
public class BulletData : ShootableData
{
    public bool isLaser = false;
    public float lifetimeHurtLimitMin = 0;
    public float lifetimeHurtLimit = 1;
    public LayerMask layer;
    [Space]
    public Sprite[] sprites;
    [HideIf("@sprites.Length", 0)]
    [HideIf("@sprites.Length", 1)]
    public float animationSpeed;
    [Space]
    public float speed;
    public Vector2 speedModRandomRange = Vector2.one;
    public Vector2 gravity;
    public bool useSpeedCurve;
    [ShowIf("useSpeedCurve")]
    public ModularCurve speedOverTime;
    public bool useScaleCurve;
    [ShowIf("useScaleCurve")]
    public ModularCurve scaleOverLifetime;
    [Space]
    public float rotationSpeed;
    public bool invertRotation;
    public bool rotationIsDirection;
    public bool useRotationCurve;
    public float absoluteRotationStartAngle;
    public bool rotationCurveIsAbsolute;
    public bool alignToSpeedInstead;
    [ShowIf("useRotationCurve")]
    public ModularCurve rotationOverTime;
    [Space]
    public bool spawnOnDeath;
    [ShowIf("spawnOnDeath")]
    public List<BulletPatternData> onDeathSpawn = new List<BulletPatternData>();
    [ShowIf("spawnOnDeath")]
    public bool spawnWhenKilled;

    public SFX SpawnSound = SFX.None;
    public bool RepeatSpawnSound;
    public bool PlayDeathSoundAlways;
    public SFX DeathSound = SFX.None;

    public float ColliderScale = 1;
    public float LaserWidth = 1;

    public ParticleSystem hitEffect;

    public bool isHoming;
    [ShowIf("isHoming")]
    [Range(0,1)]
    [ShowIf("isHoming")]
    public float homingSnappiness = 1;
    [ShowIf("isHoming")]
    public bool useHomingCurve;
    [ShowIf("useHomingCurve")]
    public ModularCurve homingOverTime;

    public bool NoStartHome = false;

    public virtual void OnStart(BulletBehaviour behaviour)
    {
        if (isHoming)
        {

            if (!NoStartHome)
            {
                Home(behaviour, 0);
            } else
            {

                Home(behaviour);
            }
        }
        behaviour.speed = EvaluateSpeed(behaviour) * behaviour.transform.up;

        behaviour.CircleCollider.enabled = false;
        behaviour.BoxCollider.enabled = false;
        behaviour.spriteRenderer.drawMode = SpriteDrawMode.Simple;
        behaviour.spriteRenderer.size = new Vector2(behaviour.spriteRenderer.size.x, behaviour.spriteRenderer.size.x);
        behaviour.spriteRenderer.transform.localScale = Vector3.one;

        if (isLaser)
        {
            behaviour.spriteRenderer.drawMode = SpriteDrawMode.Sliced;
            behaviour.spriteRenderer.size = new Vector2(behaviour.spriteRenderer.size.x * LaserWidth, 100);
            behaviour.spriteRenderer.transform.localScale = Vector3.one;
            behaviour.BoxCollider.size = Vector2.right * ColliderScale + Vector2.up * 100;
        } else
        {
            behaviour.CircleCollider.radius = ColliderScale * 0.5f;
        }

    }
    public virtual void OnUpdate(BulletBehaviour behaviour)
    {
        UpdateRotation(behaviour);

        if (isHoming && useHomingCurve)
        {
            Home(behaviour);
        }
        if (!behaviour._speedIsForced)
        {
            behaviour.speed = EvaluateSpeed(behaviour) * behaviour.transform.up;
            behaviour.speed = behaviour.speed + gravity * behaviour.LifeTimer;
        }

        if (useScaleCurve)
        {
            behaviour.spriteRenderer.transform.localScale = Vector3.one * scaleOverLifetime.Get(behaviour.LifeTimer/behaviour._lifetime);
        }

        if (behaviour._lifetime <= 0)
        {
            behaviour.CircleCollider.enabled = !isLaser;
            behaviour.BoxCollider.enabled = isLaser;
            return;
        }

        var timer = (behaviour.LifeTimer / behaviour._lifetime);
        behaviour.CircleCollider.enabled = !isLaser && timer < lifetimeHurtLimit && timer > lifetimeHurtLimitMin;
        behaviour.BoxCollider.enabled = isLaser && timer < lifetimeHurtLimit && timer > lifetimeHurtLimitMin;
    }

    internal void UpdateRotation(BulletBehaviour behaviour)
    {
        if (alignToSpeedInstead)
        {
            var speedNormal = behaviour.speed.normalized;
            behaviour.spriteRenderer.transform.eulerAngles = Vector3.forward * ( -90 + Mathf.Rad2Deg * Mathf.Atan2(speedNormal.y, speedNormal.x));
            return;
        }
        float mult = invertRotation ? -1 : 1;
        Transform targetTransform = rotationIsDirection ? behaviour.transform : behaviour.spriteRenderer.transform;

        float newAngle = targetTransform.eulerAngles.z;

        if (useRotationCurve)
        {
            if (rotationCurveIsAbsolute)
            {
                newAngle = absoluteRotationStartAngle + rotationSpeed * mult * rotationOverTime.Get(behaviour.LifeTimer);
            } else
            {
                newAngle += rotationSpeed * mult * rotationOverTime.Get(behaviour.LifeTimer);
            }
        }
        else
        {
            newAngle += rotationSpeed * mult * Manager.deltaTime;
        }

        targetTransform.eulerAngles = Vector3.forward * newAngle;
    }

    internal float EvaluateSpeed(BulletBehaviour behaviour)
    {
        if (useSpeedCurve)
        {
            return speed * behaviour.speedMod * speedOverTime.Get(behaviour.LifeTimer);
        } else
        {
            return speed * behaviour.speedMod;
        }
    }

    private void Home(BulletBehaviour behaviour, float homeAmount = -1)
    {
        if (Manager.instance.player == null)
        {
            return;
        }
        float homing = homingSnappiness;
        if (useHomingCurve)
        {
            homing *= homingOverTime.Get(behaviour.LifeTimer);
        }
        Vector3 carEulerAngles = behaviour.transform.localEulerAngles;
        if (homeAmount == 0)
        {
            carEulerAngles.z = Vector2.SignedAngle(Vector2.up, (Manager.instance.player.transform.position - behaviour.transform.position).normalized);
        } else
        {
            carEulerAngles.z += Mathf.LerpAngle(0, Vector2.SignedAngle(behaviour.transform.up, (Manager.instance.player.transform.position - behaviour.transform.position).normalized), Mathf.Clamp01(homing)) * Manager.deltaTime * 10;
        }
        behaviour.transform.localEulerAngles = carEulerAngles;
    }
}
