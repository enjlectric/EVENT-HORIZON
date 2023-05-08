using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CentipedeConfig : EnemyConfig
{
    public List<Transform> Segments = new List<Transform>();
    public Transform SegmentParent;

    public Transform MandiblesRoot;

    public List<EnemySwayParts> SwayParts;

    private SpriteRenderer spriteRenderer;

    private List<Vector3> _pastPositions;
    private int _firstIndex = 0;
    private int _distanceFrames = 0;

    private EnemyBehaviour _behaviour;

    private GameObject _midPrefab;

    public void SpawnSegments(EnemyBehaviour behaviour, GameObject prefab, GameObject endPrefab, int amount, int distanceFrames)
    {
        _behaviour = behaviour;
        _midPrefab = prefab;
        spriteRenderer = behaviour.spriteRenderer;
        var p = Instantiate(endPrefab, SegmentParent);
        Segments.Add(p.transform);
        for (int i = 0; i < amount; i++)
        {
            p = Instantiate(prefab, SegmentParent);
            Segments.Insert(0, p.transform);

            foreach(var BulletEmitter in p.GetComponentsInChildren<BulletEmitter>())
            {
                behaviour.emitterContainer.emitters.Add(BulletEmitter);
            }
        }
        _pastPositions = new List<Vector3>();

        for (int i = 0; i < distanceFrames * (amount + 2); i++)
        {
            _pastPositions.Add(transform.position);
        }

        _distanceFrames = distanceFrames;

        SwayParts = GetComponentsInChildren<EnemySwayParts>().ToList();
    }
    public void SpawnMoreSegments(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var p = Instantiate(_midPrefab, SegmentParent);
            Segments.Insert(0, p.transform);

            foreach (var BulletEmitter in p.GetComponentsInChildren<BulletEmitter>())
            {
                _behaviour.emitterContainer.emitters.Add(BulletEmitter);
            }
            foreach (var renderer in p.GetComponentsInChildren<SpriteRenderer>())
            {
                _behaviour.AddRenderer(renderer);
            }
            foreach (var renderer in p.GetComponentsInChildren<EnemySwayParts>())
            {
                SwayParts.Add(renderer);
            }
        }
        for (int i = 0; i < _distanceFrames * (amount); i++)
        {
            _pastPositions.Insert(_firstIndex, transform.position);
            _firstIndex++;
        }
    }

    public IEnumerator KillSegments(ParticleSystem effect)
    {
        while (Segments.Count > 0)
        {
            SFX.ExplosionSmall.Play();
            Instantiate(effect, Segments[Segments.Count - 1].transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.06f);
            Destroy(Segments[Segments.Count - 1].gameObject);
            Segments.RemoveAt(Segments.Count - 1);
        }
        yield return new WaitForSeconds(0.06f);
        Destroy(MandiblesRoot.gameObject);
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        if (_pastPositions.Count == 0)
        {
            return;
        }
        _firstIndex = (_firstIndex + 1) % _pastPositions.Count;
        _pastPositions[_firstIndex] = transform.position;
        MandiblesRoot.transform.localEulerAngles = spriteRenderer.transform.localEulerAngles;
        for (int i = 0; i < Segments.Count; i++)
        {
            if (Segments[i] == null)
            {
                continue;
            }
            var idx = (_firstIndex - (i * _distanceFrames) + _pastPositions.Count - 1);
            if (i == Segments.Count - 1)
            {
                idx = Mathf.FloorToInt(idx + _distanceFrames * 0.5f);
            }
            idx = idx % _pastPositions.Count;
            
            Segments[i].position = _pastPositions[idx];
            var eu = Segments[i].localEulerAngles;
            eu.z = Vector2.SignedAngle(Vector2.left, _pastPositions[(idx + 1) % _pastPositions.Count] - _pastPositions[idx]);
            Segments[i].localEulerAngles = eu;
        }
    }
}
