using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayerController : Shootable<PlayerData>
{
    public Transform movementRoot;
    public Transform spriteRoot;
    public SpriteRenderer carSprite;
    public BoxCollider2D bounds;
    public List<BulletEmitter> emitters;
    public List<BulletEmitter> laserEmitters;

    public GameObject focus;

    private Vector2 _movementInput;
    private Vector2 _externalInput;
    private float _currentVelocity;
    private float _currentSteer;
    private float _targetCarAngle;

    private bool _isShooting;
    private float _lastShotTime;

    private bool _isFocusing = false;

    public Enjlectric.ScriptableData.Types.ScriptableDataInt Health;
    public Enjlectric.ScriptableData.Types.ScriptableDataInt MaxHealth;
    public Enjlectric.ScriptableData.Types.ScriptableDataFloat LimitMeter;
    public Enjlectric.ScriptableData.Types.ScriptableDataFloat LimitMeterDamage;
    public Enjlectric.ScriptableData.Types.ScriptableDataInt WhiteFlashValue;

    public ParticleSystem HitEffect;
    public ParticleSystem DeathEffect;

    public Enjlectric.ScriptableData.Types.ScriptableDataBool IsPaused;

    private Rigidbody2D _rb;

    private bool _limitMeterNeedsCooldown;

    private float _flashTimer;

    private TireTracksData _tracksData;

    public bool InputOverride = false;

    // Start is called before the first frame update
    internal override void Start()
    {
        base.Start();

        _rb = GetComponentInChildren<Rigidbody2D>();
        _speed = Vector2.up * 0.01f;
        _tracksData = ScriptableObject.CreateInstance<TireTracksData>();
        _tracksData.sprite = data.tireTracksSprite;
        Initialize();

        LimitMeterDamage.OnValueChanged.AddListener(FlooshPlayer);
    }

    private void OnDestroy()
    {
        LimitMeterDamage.OnValueChanged.RemoveListener(FlooshPlayer);
    }

    private void FlooshPlayer()
    {
        carSprite.DOBlendableColor(Color.black, 0.05f).SetEase(Ease.OutBack).OnComplete(() => carSprite.DOBlendableColor(Color.black.SetAlpha(0), 0.2f).SetEase(Ease.OutQuad));
    }

    private void FlashPlayer()
    {
        carSprite.DOBlendableColor(Color.white, 0.05f).SetEase(Ease.OutBack).OnComplete(() => carSprite.DOBlendableColor(Color.white.SetAlpha(0), 0.2f).SetEase(Ease.OutQuad));
    }

    public Color Red;

    private void FleshPlayer()
    {
        carSprite.DOBlendableColor(Red, 0.05f).SetEase(Ease.OutBack).OnComplete(() => carSprite.DOBlendableColor(Red.SetAlpha(0), 0.2f).SetEase(Ease.OutQuad));
    }

    public override void Initialize()
    {
        base.Initialize();
        _hp = Health.Value;
        //UIManager.instance.SetHP(_hp);
    }

    // Update is called once per frame
    internal void Update()
    {
        if (!_isFocusing)
        {
            if (LimitMeterDamage.Value > 0f)
            {
                ForceInvulnerable(0.5f - data.invulnerableTime, true);
                ShootLasers();
                FlashPlayer();
                LimitMeterDamage.Value = 0;
            }
            if (_limitMeterNeedsCooldown)
            {
                LimitMeter.Value -= Time.deltaTime * 0.6f;

                if (LimitMeter.Value <= 0)
                {
                    LimitMeter.Value = 0;
                    _limitMeterNeedsCooldown = false;
                }
                _flashTimer += Time.deltaTime;
                if (_flashTimer > 0.3f)
                {
                    FleshPlayer();
                    _flashTimer = 0;
                }
            }
            spriteRoot.transform.localPosition = Vector3.zero;
        }
        else
        {
            LimitMeter.Value += Time.deltaTime * 0.45f;
            _flashTimer += Time.deltaTime * LimitMeter.Value * LimitMeter.Value;
            if (_flashTimer > 0.15f)
            {
                FleshPlayer();
                _flashTimer = 0;
            }
            spriteRoot.transform.localPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 0.1f * LimitMeter.Value;
            if (LimitMeter.Value >= 1.1f)
            {
                if (SceneManager.GetActiveScene().buildIndex == 1)
                {
                    if (_hp > 1)
                    {
                        Hurt(1);
                    }
                }
                else
                {
                    Hurt(1);
                }
                _isFocusing = false;
                focus.SetActive(_isFocusing);
                _limitMeterNeedsCooldown = true;
            }
        }

        if (Manager.InputIsLocked())
        {
            ExternalInputFalloff();
            _speed = _externalInput;

            movementRoot.position += (Vector3)_speed * Time.deltaTime;

            if (_lifetime <= 0)
            {
                return;
            }

            if (_lifetimeStart + _lifetime < Time.time)
            {
                Kill();
            }
            return;
        }

        _hp = Mathf.Clamp(_hp, 0, MaxHealth.Value);

        Health.Value = (int)_hp;

        _currentVelocity = (_currentVelocity + _movementInput.y * data.maxVelocity * Manager.deltaTime) * data.accelerationFalloff;

        float angleMultiplier = 2;

        _currentSteer = (_currentSteer + _movementInput.x * data.steerImpact * Manager.deltaTime) * data.steerFalloff;
        ExternalInputFalloff();
        _speed = _externalInput + new Vector2(_currentSteer, _currentVelocity) * (!_isFocusing ? 1 : (0.45f - 0.4f * (LimitMeter.Value * LimitMeter.Value)));

        Vector3 carEulerAngles = carSprite.transform.localEulerAngles;
        carEulerAngles.z = Mathf.SmoothStep(_targetCarAngle, _currentVelocity * angleMultiplier, 0.5f);
        carSprite.transform.localEulerAngles = carEulerAngles;
        carSprite.transform.localScale = Vector3.one + (Vector3.up - Vector3.right) * _currentSteer * -0.015f;

        if (_isShooting && Time.time >= _lastShotTime + data.shootCooldown && !_isFocusing && !_limitMeterNeedsCooldown)
        {
            _lastShotTime = Time.time;
            Shoot();
        }

        movementRoot.position += (Vector3)_speed * Time.deltaTime;

        if (_lifetime <= 0)
        {
            return;
        }

        if (_lifetimeStart + _lifetime < Time.time)
        {
            Kill();
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = _speed * Time.fixedDeltaTime;
    }

    public override void OnUpdate()
    {
    }

    private void ExternalInputFalloff()
    {
        _externalInput *= (1 - 2 * Manager.deltaTime);
        if (_externalInput.magnitude <= 0.2f)
        {
            _externalInput = Vector2.zero;
        }
    }

    public void SetVelocity(Vector2 velocity)
    {
        _externalInput = velocity;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Vector3 closestExit = new Vector3(1000, 1000, 1000);
        foreach (var contact in collision.contacts)
        {
            if (contact.point.sqrMagnitude < closestExit.sqrMagnitude)
            {
                closestExit = contact.point;
            }
        }

        if ((closestExit - movementRoot.transform.position).magnitude > 0.75f)
        {
            Hurt(1);
        }

        _speed = closestExit - movementRoot.transform.position;
    }

    private void LateUpdate()
    {
        if (Manager.InputIsLocked())
        {
            return;
        }

        Vector3 pos = movementRoot.position;

        if (_invulnerableTime + data.invulnerableTime > Time.time && Time.time > data.invulnerableTime)
        {
            carSprite.enabled = Mathf.FloorToInt((_invulnerableTime + data.invulnerableTime - Time.time) * 50) % 2 == 0;
        }
        else
        {
            carSprite.enabled = true;
        }

        pos.x = Mathf.Clamp(pos.x, bounds.bounds.min.x, bounds.bounds.max.x);
        pos.y = Mathf.Clamp(pos.y, bounds.bounds.min.y, bounds.bounds.max.y);
        movementRoot.position = pos;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        _isShooting = context.ReadValue<float>() > 0.5f;
    }

    public void HurtEffect()
    {
        SFX.ExplosionSmall.Play();
        if (_hp > 1)
        {
            SFX.PlayerHurt.Play();
        }
        else
        {
            SFX.PlayerHurtLastHP.Play();
        }
        SFX.ExplosionPlayerHurt.Play();
        WhiteFlashValue.Value = 6;
        Instantiate(HitEffect, transform);
        Manager.ShakeCamera(0.2f, 0.25f);
        FlashPlayer();
    }

    protected override void OnHurt()
    {
        Manager.instance.thisLevelHits++;
        HurtEffect();
        //Manager.DoSlowdown(4f, 0.01f);
        //UIManager.instance.DOFlash(0.1f, 0.3f);
        //UIManager.instance.ShakeHPNeedle(_hp <= 1);
        //Camera.main.DOShakePosition(0.5f, 0.5f).SetEase(Ease.OutElastic);
        //UIManager.instance.SetHP(_hp);
        Manager.instance.ReduceScoreMultiplierFromDamage();
    }

    public override void Kill()
    {
        Health.Value = 0;
        //UIManager.instance.SetHP(_hp);
        //UIManager.instance.ShakeHPNeedle(false);
        gameObject.SetActive(false);
        WhiteFlashValue.Value = 8;
        Instantiate(DeathEffect, transform.position, Quaternion.identity);
        Manager.instance.EndRun();
    }

    public void OnDrift(InputAction.CallbackContext context)
    {
        if (InputOverride)
        {
            return;
        }
        _isFocusing = context.ReadValue<float>() > 0.5f;

        if (!_isFocusing && LimitMeter.Value > 0)
        {
            _limitMeterNeedsCooldown = true;
        }

        if (_limitMeterNeedsCooldown)
        {
            _isFocusing = false;
        }

        focus.SetActive(_isFocusing);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (InputOverride)
        {
            return;
        }
        if (context.ReadValue<float>() > 0.5f)
        {
            IsPaused.Value = true;
        }
    }

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (InputOverride)
        {
            return;
        }
        if (context.ReadValue<float>() > 0.5f)
        {
            //IsPaused.Value = false;
        }
    }

    public void AddHP()
    {
        _hp = _hp + 1;
        SFX.PlayerGainHealth.Play();
    }

    public void ShootLasers()
    {
        var val = Mathf.Clamp(Mathf.FloorToInt(LimitMeterDamage.Value * 4), 0, 3);

        switch (val)
        {
            case 3:
                SFX.PlayerLaserStrongest.Play();
                break;

            case 2:
                SFX.PlayerLaserStrong.Play();
                break;

            case 1:
                SFX.PlayerLaserMedium.Play();
                break;

            case 0:
                SFX.PlayerLaserWeak.Play();
                break;
        }

        laserEmitters[val].EvaluateAllPatterns();
    }

    public void Shoot()
    {
        SFX.PlayerShoot.Play();

        foreach (var emitter in emitters)
        {
            emitter.EvaluateAllPatterns();
        }
    }
}