using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyData : ShootableData
{
    public List<float> phaseTransitions;
    public int layer = 7;
    public float despawnBeforeEndOfLifetime = 0;
    public ParticleSystem despawnEffect;
    public ParticleSystem deathEffect;
    public int ScoreValue;

    public bool NoInvulnerableSound;

    public SFX SpecialHitEffect;
    public SFX DeathSoundEffect = SFX.ExplosionRegular;

    public List<Sprite> sprites;
    public int deathFrame = -1;
    public Vector2 speed;

    public Sprite HealthBarSprite;
    public Vector2 HealthBarOffset;

    public virtual void OnStart(EnemyBehaviour behaviour)
    {
        behaviour.gameObject.layer = layer;
    }
    public virtual void OnUpdate(EnemyBehaviour behaviour)
    {

    }
    public virtual void OnHurt(EnemyBehaviour behaviour)
    {

    }
    internal virtual IEnumerator OnDespawn(EnemyBehaviour behaviour)
    {
        yield return null;
    }

    public virtual void OnPhaseTransition(EnemyBehaviour behaviour)
    {
        //Manager.DoSlowdown(0.5f, 0.25f);
        //UIManager.instance.DOFlash(0.75f, 0.3f);
        //References.DeactivateAll<BulletBehaviour>(b => b.gameObject.layer == 8);
    }

    public virtual IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        yield return null;
    }
}

[System.Serializable]
public class PatternList
{
    [AssetList(AssetNamePrefix = "BulletPattern_")]
    public List<BulletPatternData> patterns = new List<BulletPatternData>();
}

public class EnemyData<T> : EnemyData where T : EnemyConfig
{
    public T bulletSourcePrefab;

    [SerializeField]
    public List<PatternList> configPatternOverrides = new List<PatternList>();

    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);
        behaviour.emitterContainer = Instantiate(bulletSourcePrefab, behaviour.transform);
        behaviour.speed = speed;
        behaviour.spriteRenderer.sprite = sprites.FirstOrDefault();

        for (int i = 0; i < behaviour.emitterContainer.emitters.Count; i++)
        {
            if (configPatternOverrides.Count > i)
            {
                behaviour.emitterContainer.emitters[i].patterns = configPatternOverrides[i].patterns;
            }
        }
    }
    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if(behaviour.init && !MaskManager.IsInBounds(behaviour.transform.position))
        {
            behaviour.ForceInvulnerable(0.1f);
        }
    }

    protected void SpritesheetAnimation(EnemyBehaviour behaviour, List<int> sequence, float speed, SpriteRenderer renderer = null)
    {
        if (renderer == null)
        {
            renderer = behaviour.spriteRenderer;
        }
        if (sequence.Count == 0)
        {
            return;
        }
        renderer.sprite = sprites[sequence[Mathf.FloorToInt(behaviour.animationTimer * speed) % sequence.Count]];
    }

    internal void Shoot(EnemyBehaviour behaviour, int gunIdx = -1, int patternIdx = -1)
    {
        if (gunIdx == -1)
        {
            foreach(BulletEmitter e in behaviour.emitterContainer.emitters)
            {
                e.EvaluateAllPatterns();
            }
        } else
        {
            if (patternIdx == -1)
            {
                behaviour.emitterContainer.emitters[gunIdx].EvaluateAllPatterns();
            } else
            {
                behaviour.emitterContainer.emitters[gunIdx].EvaluatePattern(patternIdx);
            }
        }
    }

    internal void AbortRoutines(EnemyBehaviour behaviour)
    {
        foreach (BulletEmitter e in behaviour.emitterContainer.emitters)
        {
            e.CancelAllPatterns();
        }
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        DeathSoundEffect.Play();
        if (deathFrame >= 0)
        {
            behaviour.SetFrame(deathFrame);
        }
        GrantScore(behaviour);
        if (deathEffect != null)
        {
            Instantiate(deathEffect, behaviour.transform.position, Quaternion.identity);
            behaviour.spriteRenderer.transform.DOBlendableLocalMoveBy(new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-0.5f, 0.5f), 0), 0.8f).SetEase(Ease.OutQuad);
            behaviour.spriteRenderer.transform.DOBlendableLocalRotateBy(new Vector3(0, 0, Random.Range(-25.0f, 25.0f)), 0.8f).SetEase(Ease.OutQuad);
            
            //behaviour.transform.DOPunchScale(behaviour.transform.localScale, 0.2f);
            for (int i = 1; i < 9; i++)
            {
                behaviour.spriteRenderer.enabled = false;
                yield return new WaitForSeconds(0.04f);
                behaviour.spriteRenderer.enabled = true;
                yield return new WaitForSeconds(0.04f);
            }
        }
        References.DestroyObject(behaviour);
    }

    internal override IEnumerator OnDespawn(EnemyBehaviour behaviour)
    {
        if (despawnEffect != null)
        {
            Instantiate(despawnEffect, behaviour.transform.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(despawnBeforeEndOfLifetime);
        References.DestroyObject(behaviour);
    }

    public void GrantScore(EnemyBehaviour behaviour)
    {
        Manager.instance.PointExplosion(behaviour.transform.position, ScoreValue);
    }
}
