using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveObject : MonoBehaviour
{
    public Vector3 speed;
    public float duration;
    public Ease Ease;

    public void Move()
    {
        transform.DOLocalMove(speed, duration).SetEase(Ease);
    }
}
