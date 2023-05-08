using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References : MonoBehaviour
{
    public static References instance;

    public BulletBehaviour bulletPrefab;
    public TireTracks tireTracksPrefab;
    public EnemyBehaviour enemyPrefab;
    public Pickup pickupPrefab;

    public List<Color> Palette = new List<Color>();

    private Dictionary<System.Type, ObjectPool> _pools = new Dictionary<System.Type, ObjectPool>();

    private void Awake()
    {
        if (instance == null)
        {
            _pools.Add(typeof(BulletBehaviour), new ObjectPool<BulletBehaviour>(bulletPrefab, transform, 400));
            _pools.Add(typeof(EnemyBehaviour), new ObjectPool<EnemyBehaviour>(enemyPrefab, transform, 15));
            instance = this;
        } else
        {
            Destroy(this);
        }
    }

    public void Update()
    {
        foreach(var pool in _pools.Values)
        {
            pool.OnUpdate();
        }
    }

    public static void DeactivateAll()
    {
        foreach(var pool in instance._pools.Values)
        {
            pool.DeactivateAll();
        }
    }

    public static void DeactivateAll<T>(bool doKillEffect = false)
    {
        DeactivateAll<T>((a) => {return true;}, doKillEffect);
    }

    public static void DeactivateAll<T>(System.Func<PoolableObject, bool> predicate, bool doKillEffect = false)
    {
        if (instance._pools.ContainsKey(typeof(T)))
        {
            instance._pools[typeof(T)].DeactivateAll(predicate, doKillEffect);
        }
    }

    public static void ResetParent<T>(GameObject child) where T : PoolableObject
    {
        if (instance._pools.ContainsKey(typeof(T)))
        {
            instance._pools[typeof(T)].ResetParent(child);
        }
    }

    public static T CreateObject<T>(PoolableData data, Vector3 position = default, Quaternion rotation = default) where T : PoolableObject
    {
        if (instance._pools.ContainsKey(typeof(T)))
        {
            return instance._pools[typeof(T)].CreateInstance(data, position, rotation) as T;
        }

        return null;
    }

    public static void DestroyObjectsOfType<T>()
    {
        if (instance._pools.ContainsKey(typeof(T)))
        {
            instance._pools[typeof(T)].DeactivateAll();
        }
    }

    public static void DestroyObject(GameObject obj)
    {
        foreach(var pool in instance._pools.Values)
        {
            if (pool.DestroyInstance(obj))
            {
                return;
            }
        }

        Destroy(obj.gameObject);
    }

    public static void DestroyObject(MonoBehaviour obj)
    {
        if (instance._pools.ContainsKey(obj.GetType()))
        {
            if (instance._pools[obj.GetType()].DestroyInstance(obj.gameObject))
            {
                return;
            }
        }

        Destroy(obj.gameObject);
    }
}
