using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpeed : MonoBehaviour
{
    private Rigidbody2D _rb;

    public float xPosLimit;
    public Enjlectric.ScriptableData.Types.ScriptableDataInt StageProgress;
    public bool triggered = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        if (StageProgress == null)
        {
            return;
        }
        if (transform.position.x <= xPosLimit && !triggered)
        {
            triggered = true;
            _rb.velocity = Vector2.zero;
            transform.position = Vector3.left * 90;
            StageProgress.Value++;
        }
    }

    public void ChangeX(float speed)
    {
        _rb.velocity = new Vector2(speed, _rb.velocity.y);
    }

    public void ChangeXAdd(float speed, float min)
    {
        _rb.velocity = new Vector2(Mathf.Max(min, speed + _rb.velocity.x), _rb.velocity.y);
    }

    public void ChangeY(float speed)
    {
        _rb.velocity = new Vector2(_rb.velocity.x, speed);
    }
}