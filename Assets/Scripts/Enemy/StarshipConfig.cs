using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarshipConfig : EnemyConfig
{
    public SpriteRenderer[] wings = new SpriteRenderer[2];

    public EnemyBehaviour HugeGunModule;

    public List<BulletEmitter> WingEmitters = new List<BulletEmitter>();

    public Vector2 HugeGunFrozenPosition;

    public GameObject BlackHole;

    public void AddWingEmitters(StarshipData data)
    {
        HugeGunModule = References.CreateObject<EnemyBehaviour>(data.HugeGunModuleData, Vector3.right * 500, Quaternion.identity);
        emitters.AddRange(WingEmitters);
    }

    public void RemoveWingEmitters()
    {
        foreach(var we in WingEmitters)
        {
            emitters.Remove(we);
        }
    }

    public void FreezeHugeGun(EnemyBehaviour behaviour)
    {
        var cameraRightEdge = MaskManager.GetPositionRelativeToCam(1, 0);
        HugeGunModule.transform.position = cameraRightEdge +  new Vector3(HugeGunFrozenPosition.x, HugeGunFrozenPosition.y);
    }
}
