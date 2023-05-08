using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonConfig : EnemyConfig
{
    public Transform MandiblesRoot;
    public Transform LeftArm;
    public Transform RightArm;
    public List<Transform> Segments = new List<Transform>();
    public Transform SegmentParent;
    private float _distanceFrames = 0;
    private GameObject _midPrefab;

    public EnemyBehaviour _behaviour;


    public void SpawnSegments(EnemyBehaviour behaviour, GameObject prefab, GameObject endPrefab, int amount, float distanceFrames)
    {
        _behaviour = behaviour;
        _midPrefab = prefab;
        var p = Instantiate(endPrefab, SegmentParent);
        Segments.Add(p.transform);
        for (int i = 0; i < amount; i++)
        {
            p = Instantiate(prefab, SegmentParent);
            Segments.Insert(0, p.transform);

            foreach (var BulletEmitter in p.GetComponentsInChildren<BulletEmitter>())
            {
                behaviour.emitterContainer.emitters.Add(BulletEmitter);
            }

            p.GetComponentInChildren<EnemySwayParts>().Phase = i * 0.5f;
        }
        _distanceFrames = distanceFrames;
    }
    public void SpawnMoreSegments(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var p = Instantiate(_midPrefab, SegmentParent);
            Segments.Add(p.transform);

            foreach (var renderer in p.GetComponentsInChildren<SpriteRenderer>())
            {
                _behaviour.AddRenderer(renderer);
            }
        }
    }

    public void KillSegments()
    {
        while (Segments.Count > 0)
        {
            Destroy(Segments[Segments.Count - 1].gameObject);
            Segments.RemoveAt(Segments.Count - 1);
        }
        Destroy(LeftArm.gameObject);
        Destroy(RightArm.gameObject);
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        MandiblesRoot.transform.localEulerAngles = _behaviour.spriteRenderer.transform.localEulerAngles;
        _behaviour.spriteRenderer.transform.position = MandiblesRoot.transform.position;

        for (int i = 0; i < Segments.Count; i++)
        {
            var previousTransform = _behaviour.transform.position + Vector3.right;
            if (i > 0)
            {
                previousTransform = Segments[i - 1].transform.position;
            }
            Segments[i].transform.position = previousTransform + Vector3.right * _distanceFrames;
        }
    }
}
