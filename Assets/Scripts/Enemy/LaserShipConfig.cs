using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShipConfig : EnemyConfig
{
    public EnemyBehaviour TopShip;
    public EnemyBehaviour BottomShip;
    public EnemyBehaviour Barrier;
    public float DistanceLeft = -2;
    public float DistanceTopToBottom = 2;
    public List<Transform> BarrierSprites = new List<Transform>();

    public void OnUpdate(EnemyBehaviour behaviour)
    {
        var cameraRightEdge = MaskManager.GetPositionRelativeToCam(1, 0);
        TopShip.transform.position = cameraRightEdge + Vector3.left * DistanceLeft + Vector3.up * DistanceTopToBottom;
        Barrier.transform.position = cameraRightEdge + Vector3.left * DistanceLeft;
        SineMove(TopShip.transform, 0);
        BottomShip.transform.position = cameraRightEdge + Vector3.left * DistanceLeft + Vector3.down * DistanceTopToBottom;
        SineMove(BottomShip.transform, 2);

        Barrier.transform.localScale = 4 * DistanceTopToBottom * Vector3.up + Vector3.right + Vector3.forward;

        for (int i = 0; i < BarrierSprites.Count; i++)
        {
            var y = Vector3.up * 0.7f * DistanceTopToBottom + (i/(float)(BarrierSprites.Count - 1.0f)) * 1.4f * DistanceTopToBottom * Vector3.down;
            BarrierSprites[i].transform.position = cameraRightEdge + Vector3.left * DistanceLeft + y;
            SineMove(BarrierSprites[i], i * 0.3f);
        }
    }

    public void SineMove(Transform obj, float offset)
    {
        obj.transform.position += Mathf.Sin(Time.time * 4 + offset) * Vector3.up * 0.2f;
    }
}
