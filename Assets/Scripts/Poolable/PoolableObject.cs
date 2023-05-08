using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolableObject : MonoBehaviour
{
    public abstract PoolableData GetData();
    public abstract void SetData(PoolableData data);
    public abstract void Initialize();

    public abstract void OnUpdate();
    public abstract void Kill();
}

public abstract class PoolableObject<T> : PoolableObject where T : PoolableData
{
    public T data;

    internal Vector2 _speed;

    public override PoolableData GetData()
    {
        return data;
    }

    public override void OnUpdate()
    {
        transform.position += (Vector3) _speed * Manager.deltaTime;
    }

    public override void SetData(PoolableData d)
    {
        data = (T)d;
    }

    public override void Kill()
    {
        References.DestroyObject(this);
    }
}
