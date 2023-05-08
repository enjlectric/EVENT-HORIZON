using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ObjectPool
{
    public abstract MonoBehaviour CreateInstance(PoolableData data, Vector3 position = default, Quaternion rotation = default);
    public abstract bool DestroyInstance(GameObject obj);
    public abstract void OnUpdate();
    public abstract void ResetParent(GameObject obj);
    public abstract void DeactivateAll(bool doKillEffect = false);
    public abstract void DeactivateAll(System.Func<PoolableObject, bool> predicate, bool doKillEffect = false);
}

public class ObjectPool<T> : ObjectPool where T : PoolableObject
{
    private List<T> activeInstances = new List<T>();
    private List<T> inactiveInstances = new List<T>();

    private T _prefab;
    private Transform _parent;

    public ObjectPool(T prefab, Transform parent, int prefill) {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < prefill; i++)
        {
            var instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            inactiveInstances.Add(instance);
        }
    }

    public override void ResetParent(GameObject obj)
    {
        obj.transform.SetParent(_parent);
    }

    private List<T> _toActivate = new List<T>();
    private List<T> _toDeactivate = new List<T>();

    public override void OnUpdate()
    {
        foreach (T instance in _toDeactivate)
        {
            DestroyInstance(instance);
        }

        _toDeactivate.Clear();

        foreach (T instance in activeInstances)
        {
            instance.OnUpdate();
        }

        foreach(T instance in _toActivate)
        {
            instance.OnUpdate();
            activeInstances.Add(instance);
        }

        _toActivate.Clear();

        foreach(T instance in _toDeactivate)
        {
            DestroyInstance(instance);
        }

        _toDeactivate.Clear();
    }

    public override void DeactivateAll(bool doKillEffect = false)
    {
        DeactivateAll((a) => { return true; }, doKillEffect);
    }

    public override void DeactivateAll(System.Func<PoolableObject, bool> predicate, bool doKillEffect = false)
    {
        foreach(var instance in activeInstances)
        {
            if (predicate(instance))
            {

                if (doKillEffect)
                {
                    instance.Kill();
                }
                else
                {
                    _toDeactivate.Add(instance);
                }
            }
        }
    }

    public override MonoBehaviour CreateInstance(PoolableData data, Vector3 position = default, Quaternion rotation = default)
    {
        T instance = null;
        if (inactiveInstances.Count == 0)
        {
            instance = Object.Instantiate(_prefab, _parent);
            //instance.gameObject.name = $"{data.name} Clone {inactiveInstances.Count + activeInstances.Count + 1}";
        } else
        {
            while (instance == null && inactiveInstances.Count > 0)
            {
                instance = inactiveInstances[0];
                inactiveInstances.RemoveAt(0);
            }

            if (instance == null)
            {
                instance = Object.Instantiate(_prefab, _parent);
            }
        }

        instance.gameObject.SetActive(true);
        _toActivate.Add(instance);
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.SetData(data);
        instance.Initialize();

        return instance;
    }

    private void DestroyInstance(T instance)
    {
        if (activeInstances.Contains(instance))
        {
            activeInstances.Remove(instance);
            inactiveInstances.Add(instance);
            instance.transform.SetParent(_parent);
            instance.gameObject.SetActive(false);
        }
    }

    public override bool DestroyInstance(GameObject instance)
    {
        if (activeInstances.FirstOrDefault(i => i.gameObject == instance) is T comp && comp != null)
        {
            _toDeactivate.Add(comp);
            return true;
        }

        if (_toActivate.FirstOrDefault(i => i.gameObject == instance) is T comp2 && comp2 != null)
        {
            _toDeactivate.Add(comp2);
            return true;
        }

        return false;
    }
}
