using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/PlayerData", fileName = "PlayerData")]
public class PlayerData : ShootableData
{
    public float maxVelocity;
    public float steerImpact;
    public float maxSteer;
    public float accelerationFalloff;
    public float steerFalloff;
    public float driftSteerImpact;
    public float driftMinAngle;
    public float driftIdleAngle;
    public float driftMaxAngle;

    public float shootCooldown;

    public BulletData projectile;
    public Sprite tireTracksSprite;
}
