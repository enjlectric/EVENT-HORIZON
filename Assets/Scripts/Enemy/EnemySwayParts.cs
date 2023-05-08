using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwayParts : MonoBehaviour
{
    public float Range = 45;
    public float Speed = 1;
    public float Phase = 0;

    private float timer;
    public bool UpDown;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Speed * Time.deltaTime;
        if (UpDown)
        {
            transform.localPosition = startPos + new Vector3(0, Mathf.Sin(timer * Speed + Phase)) * Range;
        } else
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.Sin(timer * Speed + Phase)) * Range;
        }
    }
}
