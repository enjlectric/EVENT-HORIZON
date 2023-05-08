using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FeedbackButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public SFX hoverSound;
    public SFX clickSound;
    public SFX clickConfirmSound;

    public float hoverScale;
    public float hoverAngle;
    public Color hoverColor;
    public Sprite clickSprite;
    public Sprite hoverSprite;
    public RectTransform text;
    public UnityEvent onClick;

    public Image background;
    private Sprite _defaultSprite;
    private Vector3 _startPos;
    private float _startAngle;
    private float _angleSpeed;

    public bool interactable;

    private Tween _rotateTween;
    private Tween _scaleTween;
    private Tween _punchTween;
    private Tween _colorTween;

    private bool _idle = true;
    private bool _pointerState = false;
    private float _speed = 1;

    public float radius = 4;
    public float step = 8;

    private CanvasGroup _cg;

    // Start is called before the first frame update
    void Start()
    {
        _defaultSprite = background.sprite;
        _angleSpeed = Random.Range(-1.5f, 1.5f);
        _startAngle = Random.Range(-180, 180);
        _startPos = background.transform.position;
        _cg = GetComponentInParent<CanvasGroup>();
    }

    void Update()
    {
        if (_cg.alpha == 0)
        {
            return;
        }
        text.anchoredPosition = _pointerState ? Vector3.down * step : Vector3.up * 3;
        _speed = Mathf.Clamp01(_speed + Time.unscaledDeltaTime * (_idle ? 1 : -2));
        _startAngle += _speed * _angleSpeed;
        background.transform.position = _startPos + Quaternion.Euler(0, 0, _startAngle) * Vector2.up * radius;
    }

    private void ResetTweens()
    {
        _rotateTween?.Complete();
        _scaleTween?.Complete();
        _punchTween?.Complete();
        _colorTween?.Complete();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!interactable) { return;}
        ResetTweens();
        _rotateTween = transform.DORotate(new Vector3(0, 0, hoverAngle), 0.25f).SetEase(Ease.OutQuint).SetUpdate(true);
        _scaleTween = transform.DOScale(Vector3.one * hoverScale, 0.15f).SetEase(Ease.OutQuint).SetUpdate(true);
        _punchTween = background.transform.DOPunchScale(Vector3.one * 0.15f, 0.15f).SetEase(Ease.OutQuint).SetUpdate(true);
        _colorTween = background.DOColor(hoverColor, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
        background.sprite = hoverSprite;
        hoverSound.Play();
        _idle = false;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (!interactable) { return; }
        ResetTweens();
        _rotateTween = transform.DORotate(new Vector3(0, 0, 0), 0.25f).SetEase(Ease.OutQuint).SetUpdate(true);
        _scaleTween = transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutQuint).SetUpdate(true);
        _punchTween = background.transform.DOPunchScale(Vector3.one * 0.15f, 0.15f).SetEase(Ease.OutQuint).SetUpdate(true);
        _colorTween = background.DOColor(Color.white, 0.3f).SetEase(Ease.OutQuint).SetUpdate(true);
        hoverSound.Play();
        background.sprite = _defaultSprite;
        _idle = true;
        _pointerState = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable) { return; }
        ResetTweens();
        _punchTween = background.transform.DOPunchScale(Vector3.one * 0.15f, 0.15f).SetEase(Ease.OutQuint).SetUpdate(true);
        background.sprite = clickSprite;
        clickSound.Play();
        _pointerState = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable) { return; }
        ResetTweens();
        if (_pointerState)
        {
            _pointerState = false;
            _punchTween = background.transform.DOPunchScale(Vector3.one * 0.35f, 0.15f).SetEase(Ease.OutQuint).SetUpdate(true);
            background.sprite = hoverSprite;
            clickConfirmSound.Play();
            onClick?.Invoke();
        }
    }
}
