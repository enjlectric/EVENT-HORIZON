using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpritesheetAnimation : MonoBehaviour
{
    public bool Once = false;
    public List<Sprite> sprites = new List<Sprite>();

    public float AnimationSpeed = 1;

    private SpriteRenderer _renderer;

    private float _timer;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;

        _renderer.sprite = sprites[Mathf.FloorToInt(AnimationSpeed * _timer) % sprites.Count];
        if (Once && Mathf.FloorToInt(AnimationSpeed * _timer) % sprites.Count == 0 && _timer * AnimationSpeed > 1)
        {
            gameObject.SetActive(false);
        }
    }
}
