using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZParallax : MonoBehaviour
{
    private Camera _camera;
    private float _parallaxMultiplier;

    private Vector3 _startPosition;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _parallaxMultiplier = transform.localPosition.z * 0.1f;
        _startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _startPosition + _parallaxMultiplier * _camera.transform.position;
    }
}
