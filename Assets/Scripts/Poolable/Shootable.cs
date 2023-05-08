using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shootable : PoolableObject
{
    public float DamageMultiplier = 1;
    internal abstract void ProcessContact(Shootable other, bool isReaction = false);
    internal abstract void Hurt(float damage);
}

public class Shootable<T> : Shootable where T : ShootableData
{
    [SerializeField]
    public T data;

    internal float _hp;
    internal float _invulnerableTime = 0;
    internal float _lifetimeStart;
    internal float _lifetime;

    private List<int> _friendlyLayers = new List<int>()
    {
        0,0,0,0,0,0,9,8,7,6,8
    };


    internal Vector2 _speed;

    internal virtual void Start()
    {
        //Initialize();
    }

    public override void Initialize()
    {
        _lifetime = data.lifetime.Random();
        _hp = data.health;
        _lifetimeStart = Time.time;
        _invulnerableTime = 0;
    }

    public override PoolableData GetData()
    {
        return data;
    }

    public override void SetData(PoolableData d)
    {
        data = (T)d;
    }

    public override void OnUpdate()
    {
        transform.position += (Vector3)_speed * Manager.deltaTime;

        if (_lifetime <= 0)
        {
            return;
        }

        if (_lifetimeStart + _lifetime < Time.time)
        {
            Kill();
        }
    }

    public void ForceInvulnerable(float seconds, bool maxCurrent = false)
    {
        if (maxCurrent)
        {
            _invulnerableTime = Mathf.Max(Time.time + seconds, _invulnerableTime);
        } else
        {
            _invulnerableTime = Time.time + seconds;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != 6)
        {
            if (other.GetComponent<Shootable>() is Shootable shootable)
            {
                ProcessContact(shootable);

                shootable.ProcessContact(this, true);
            }
        } else
        {
            if (other.GetComponentInParent<Shootable>() is Shootable shootable)
            {
                ProcessContact(shootable);

                shootable.ProcessContact(this, true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer != 6)
        {
            if (other.GetComponent<Shootable>() is Shootable shootable)
            {
                ProcessContact(shootable);

                shootable.ProcessContact(this, true);
            }
        }
        else
        {
            if (other.GetComponentInParent<Shootable>() is Shootable shootable)
            {
                ProcessContact(shootable);

                shootable.ProcessContact(this, true);
            }
        }

    }

    internal override void ProcessContact(Shootable other, bool isReaction = false)
    {

        if (other.gameObject.layer == gameObject.layer)
        {
            return;
        }

        if (other is EnemyBehaviour bh && bh.dead)
        {
            return;
        }

        if ((_friendlyLayers.Count > gameObject.layer && other.gameObject.layer == _friendlyLayers[gameObject.layer]) || (other.gameObject.layer == 10 && gameObject.layer == 8))
        {
            return;
        }

        ShootableData otherData = (ShootableData) other.GetData();

        if (!isReaction && data.takesContactDamage && otherData.isSolid)
        {
            Hurt(data.ownContactDamage + otherData.damage * other.DamageMultiplier);
        }

        if (otherData.shootable)
        {
            other.Hurt(data.damage * DamageMultiplier);
        }
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
            return;
        }

        if (_hp > 0 && _hp - damage <= 0)
        {
            _hp -= damage;
            Kill();
            return;
        }

        _hp -= damage;
        _invulnerableTime = Time.time;
        OnHurt();
    }

    protected virtual void OnHurt()
    {

    }

    public override void Kill()
    {
        References.DestroyObject(this);
    }
}
