using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : Shootable<BulletData>
{
    public SpriteRenderer spriteRenderer;

    [HideInInspector]
    public Vector2 speed;
    [HideInInspector]
    public int AnimationFrame = 0;
    [HideInInspector]
    public float AnimationTimer = 0;

    [HideInInspector]
    public float speedMod = 1;

    public BoxCollider2D BoxCollider;
    public CircleCollider2D CircleCollider;

    EnemyBehaviour parentEnemyBehaviour;
    bool needsParentCheck = false;

    public float LifeTimer => Time.time - _lifetimeStart;

    internal bool _speedIsForced = false;

    private Rigidbody2D _rb;

    private AudioSource _repeatingSound;

    internal override void Start()
    {
        base.Start();
    }

    public override void Initialize()
    {
        base.Initialize();

        gameObject.layer = (int)Mathf.Log(data.layer.value, 2);
        AnimationFrame = 0;
        AnimationTimer = 0;
        spriteRenderer.transform.localRotation = Quaternion.identity;
        needsParentCheck = data.isLaser;
        speedMod = Random.Range(data.speedModRandomRange.x, data.speedModRandomRange.y);
        _speedIsForced = false;

        if (data.SpawnSound != SFX.None)
        {
            data.SpawnSound.Play();
        } 

        if (data.shootable && data.isSolid)
        {
            if (_rb == null)
            {
                _rb = gameObject.AddComponent<Rigidbody2D>();
                if (_rb != null)
                {
                    _rb.isKinematic = true;
                }
            }
        } else
        {
            if (_rb != null)
            {
                Destroy(_rb);
            }
        }

        SetFrame();
        data.OnStart(this);
    }

    // Update is called once per frame
    public override void OnUpdate()
    {
        if (data.isLaser)
        {
            if (data.SpawnSound != SFX.None && data.RepeatSpawnSound)
            {
                var rps = data.SpawnSound.Play();
                if (rps != null)
                {
                    _repeatingSound = rps;
                }
            }
            if (needsParentCheck)
            {
                parentEnemyBehaviour = GetComponentInParent<EnemyBehaviour>();
            }
            if (parentEnemyBehaviour != null)
            {
                if (parentEnemyBehaviour.dead)
                {
                    Kill();
                }
            }
        }
        AnimationTimer += Manager.deltaTime * data.animationSpeed;
        while (AnimationTimer >= 1)
        {
            AnimationTimer--;
            AnimationFrame++;
            AnimationFrame %= data.sprites.Length;
        }

        data.OnUpdate(this);
        _speed = speed;

        base.OnUpdate();
        SetFrame();
    }

    public void ForceSpeed(Vector2 spd)
    {
        _speedIsForced = true;
        speed = spd;
    }

    public override void Kill()
    {
        if (_repeatingSound)
        {
            if (_repeatingSound.isPlaying)
            {
                _repeatingSound.Stop();
            }
            _repeatingSound = null;
        }
        if (data.spawnOnDeath)
        {
            if ((_hp <= 0 && data.spawnWhenKilled) || LifeTimer >= _lifetime)
            {
                foreach (var spawner in data.onDeathSpawn)
                {
                    CoroutineManager.Start(spawner.Evaluate(transform));
                }
                Instantiate(data.hitEffect, transform.position, transform.rotation);
            }
        }

        if (_hp <= 0 && data.hitEffect != null)
        {
            Instantiate(data.hitEffect, transform.position, transform.rotation);
        }

        if (_hp <= 0 || data.PlayDeathSoundAlways)
        {
            if (data.DeathSound != SFX.None)
            {
                data.DeathSound.Play();
            }
        }

        base.Kill();
    }

    private void SetFrame()
    {
        spriteRenderer.sprite = data.sprites[AnimationFrame];

        //var eulerAngles = spriteRenderer.transform.localEulerAngles;
        //eulerAngles.z = Vector2.SignedAngle(Vector2.up, rb.velocity);
        //spriteRenderer.transform.localEulerAngles = eulerAngles;
    }
}
