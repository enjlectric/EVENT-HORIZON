using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenRectTransform : MonoBehaviour
{
    private RectTransform _rectTransform;
    private CanvasScaler _canvas;

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<CanvasScaler>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _rectTransform.transform.position = Vector2.zero;
        _rectTransform.sizeDelta = _canvas.referenceResolution;
    }
}
