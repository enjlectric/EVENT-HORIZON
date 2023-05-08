using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : Shootable<EnemyData>
{
    [HideInInspector] public EnemyConfig emitterContainer;

    public SpriteRenderer spriteRenderer;
    private List<SpriteRenderer> additionalRenderers = new List<SpriteRenderer>();

    public bool init = false;

    [HideInInspector]
    public Vector2 speed;

    [HideInInspector]
    public Vector2 startSpeed;

    [HideInInspector]
    public Vector2 speedMultiplier = Vector2.one;

    [HideInInspector] public float stateTimer;
    [HideInInspector] public float substateTimer;
    [HideInInspector] public float animationTimer;
    [HideInInspector] public float lastSubstateTimer;
    [HideInInspector] public float lastStateTimer;

    [HideInInspector] public int state = 0;
    [HideInInspector] public int substate = 0;

    [HideInInspector] public bool dead = false;

    private Tweener _shakeTween;
    private Tweener _colorTween;

    public SpriteRenderer HealthBarRenderer;

    private int _phase = 0;

    // Start is called before the first frame update
    internal override void Start()
    {
        base.Start();
        if (init)
        {
            Initialize();
        }
    }

    private void Update()
    {
        if (init)
        {
            OnUpdate();
        }
    }

    public override void Initialize()
    {
        if (emitterContainer != null)
        {
            Destroy(emitterContainer.gameObject);
            emitterContainer = null;
        }
        speedMultiplier = Vector2.one;
        additionalRenderers.Clear();
        base.Initialize();
        stateTimer = 0;
        substateTimer = 0;
        if (!init)
        {
            transform.localScale = Vector3.one;
            spriteRenderer.transform.localPosition = Vector3.zero;
            spriteRenderer.transform.localEulerAngles = Vector3.zero;
        }
        lastSubstateTimer = 0;
        lastStateTimer = 0;
        animationTimer = 0;
        state = 0;
        substate = 0;
        _phase = 0;
        dead = false;
        _shakeTween?.Complete();
        _colorTween?.Complete();
        data.OnStart(this);
        startSpeed = speed;

        HealthBarRenderer.gameObject.SetActive(data.HealthBarSprite != null);
        if (data.HealthBarSprite != null)
        {
            HealthBarRenderer.material.SetInt("_Arc2", 0);
            HealthBarRenderer.color = References.instance.Palette[2].SetAlpha(0);
            HealthBarRenderer.sprite = data.HealthBarSprite;
        }

        if (data.layer == 10)
        {
            //ForceInvulnerable(4.5f);
            //UIManager.instance.InitializeBossHP(data.phaseTransitions.Count + 1);
        }
    }

    public override void OnUpdate()
    {
        if (dead)
        {
            return;
        }

        lastStateTimer = stateTimer;
        lastSubstateTimer = substateTimer;

        stateTimer += Manager.deltaTime;
        substateTimer += Manager.deltaTime;
        animationTimer += Manager.deltaTime;

        data.OnUpdate(this);

        _speed = speed * speedMultiplier;

        transform.position += (Vector3)_speed * Manager.deltaTime;

        if (HealthBarRenderer.gameObject.activeSelf)
        {
            HealthBarRenderer.transform.rotation = Quaternion.identity;
            HealthBarRenderer.transform.position = transform.position + spriteRenderer.transform.localRotation * new Vector3(data.HealthBarOffset.x, data.HealthBarOffset.y);
        }

        if (_lifetime <= 0)
        {
            return;
        }

        if (_lifetimeStart + _lifetime < Time.time)
        {
            Despawn();
        }
    }

    public void SetFrame(int idx, SpriteRenderer renderer = null)
    {
        if (renderer == null)
        {
            renderer = spriteRenderer;
        }

        if (idx == -1)
        {
            renderer.sprite = null;
            return;
        }
        renderer.sprite = data.sprites[idx];
    }

    public void SwitchState(int newState)
    {
        stateTimer = 0;
        substateTimer = 0;
        state = newState;
        substate = 0;
        animationTimer = 0;
    }

    public bool HasSurpassedState(float seconds)
    {
        return stateTimer >= seconds && lastStateTimer < seconds;
    }

    public bool HasSurpassedSubstate(float seconds)
    {
        return substateTimer >= seconds && lastSubstateTimer < seconds;
    }

    public bool HasSurpassedState(float seconds, float modulo)
    {
        float st = stateTimer + 100 * modulo;
        float lst = lastStateTimer + 100 * modulo;

        return ((st % modulo) >= seconds && (lst % modulo) < seconds) || (seconds == modulo && (lst % modulo) + Manager.deltaTime > modulo && (st % modulo) - Manager.deltaTime < 0);
    }

    public bool HasSurpassedSubstate(float seconds, float modulo)
    {
        float st = substateTimer + 100 * modulo;
        float lst = lastSubstateTimer + 100 * modulo;
        return ((st % modulo) >= seconds && (lst % modulo) < seconds) || (seconds == modulo && (lst % modulo) + Manager.deltaTime > modulo && (st % modulo) - Manager.deltaTime < 0);
    }

    public void SwitchSubstate(int newSubstate)
    {
        substateTimer = 0;
        substate = newSubstate;
    }

    public void TiltSprite(float tiltAmount, Transform t = null)
    {
        if (t == null)
        {
            t = spriteRenderer.transform;
        }
        Vector3 carEulerAngles = t.localEulerAngles;
        carEulerAngles.z = Mathf.SmoothStep(0, tiltAmount, 0.5f);
        t.localEulerAngles = carEulerAngles;
    }

    public void AimSprite(Transform t, Vector3 target)
    {
        Vector3 gunEulerAngles = t.localEulerAngles;
        gunEulerAngles.z = Vector2.SignedAngle(Vector2.up, (target - t.position).normalized);
        t.localEulerAngles = gunEulerAngles;
    }

    public void KeepOnScreen()
    {
        var bounds = MaskManager.instance.bounds;
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, bounds.bounds.min.x, bounds.bounds.max.x);
        pos.y = Mathf.Clamp(pos.y, bounds.bounds.min.y, bounds.bounds.max.y);
        transform.position = pos;
    }

    internal override void Hurt(float damage)
    {
        if (_hp <= 0)
        {
            return;
        }

        if (damage <= 0)
        {
            return;
        }

        if (Time.time <= _invulnerableTime + data.invulnerableTime)
        {
            if (!data.NoInvulnerableSound && Time.time <= _invulnerableTime + data.invulnerableTime - 1)
            {
                SFX.EnemyHitInvulnerable.Play();
            }
            return;
        }
        if (_hp > 0 && _hp - damage <= 0)
        {
            SwitchState(-1);
            speed = Vector2.zero;
            if (data.layer == 10)
            {
                //UIManager.instance.ReduceBossHP(_phase, 0);
            }
            Kill();
            return;
        }

        if (_phase < data.phaseTransitions.Count && _hp - damage <= data.phaseTransitions[_phase])
        {
            data.OnPhaseTransition(this);
            if (data.layer == 10)
            {
                //UIManager.instance.ReduceBossHP(_phase, 0);
                Manager.instance.PointExplosion(transform.position, Mathf.FloorToInt(data.ScoreValue * 0.1f));
            }
            _phase++;
        }

        _hp -= damage;
        _invulnerableTime = Time.time;

        if (data.layer == 10)
        {
            HealthBarRenderer.material.SetInt("_Arc2", 360 - Mathf.FloorToInt(360 * (_hp/data.health)));
            HealthBarRenderer.DOComplete();
            HealthBarRenderer.color = References.instance.Palette[1];
            HealthBarRenderer.DOBlendableColor(References.instance.Palette[2], 0.05f).SetEase(Ease.InQuad);
            HealthBarRenderer.DOBlendableColor(References.instance.Palette[2].SetAlpha(0), 1f).SetEase(Ease.OutQuint);
            //UIManager.instance.ReduceBossHP(_phase, GetPhaseHP());
        }

        //_shakeTween?.Complete();
        //_shakeTween = spriteRenderer.transform.DOShakePosition(0.02f, 0.5f).SetEase(Ease.OutQuad);
        _colorTween?.Complete();

        var color = Color.white;
        if (data.layer == 10)
        {
            color = Color.grey.SetAlpha(0.5f);
        }
        spriteRenderer.color = color;
        _colorTween = spriteRenderer.DOColor(color.SetAlpha(0), 0.05f).SetEase(Ease.InQuad);
        foreach(var renderer in additionalRenderers)
        {
            if (renderer != null)
            {
                renderer.DOComplete();
                renderer.color = color;
                renderer.DOColor(color.SetAlpha(0), 0.05f).SetEase(Ease.InQuad);
            }
        }
        OnHurt();
    }

    private float GetPhaseHP()
    {
        float a = (_phase == data.phaseTransitions.Count ? 0 : data.phaseTransitions[_phase]);
        return (_hp - a) / ((_phase == 0 ? data.health : data.phaseTransitions[_phase - 1]) - a);
    }

    protected override void OnHurt()
    {
        if (data.SpecialHitEffect != SFX.None)
        {
            data.SpecialHitEffect.Play();
        } else
        {
            SFX.EnemyHit.Play();
        }
        data.OnHurt(this);
    }

    public virtual void Despawn()
    {
        if (!dead)
        {
            foreach(var c in GetComponentsInChildren<Collider2D>())
            {
                c.enabled = false;
            }
            _speed = Vector2.zero;
            dead = true;
            emitterContainer.emitters.ForEach(emitter => emitter.CancelAllPatterns());
            CoroutineManager.Start(data.OnDespawn(this));
        }
    }

    public override void Kill()
    {
        if (!dead)
        {
            foreach (var c in GetComponentsInChildren<Collider2D>())
            {
                c.enabled = false;
            }
            emitterContainer.emitters.ForEach(emitter => emitter.CancelAllPatterns());
            dead = true;
            Manager.instance.KillEnemy();
            CoroutineManager.Start(data.OnKill(this));
        }
    }

    public void AddRenderers(List<SpriteRenderer> renderers)
    {
        additionalRenderers.AddRange(renderers);
    }

    public void RemoveRenderers(List<SpriteRenderer> renderers)
    {
        foreach (var renderer in renderers)
        {
            additionalRenderers.Remove(renderer);
        }
    }

    public void AddRenderer(SpriteRenderer renderer)
    {
        additionalRenderers.Add(renderer);
    }

    public void RemoveRenderer(SpriteRenderer renderer)
    {
        additionalRenderers.Remove(renderer);
    }
}
