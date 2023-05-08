using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckEnemyConfig : EnemyConfig
{
    public Transform backPieceRoot;
    public SpriteRenderer backPieceRenderer;
    public Transform tntThrowerRoot;
    public EnemyBehaviour tntThrowerInstance;
    public SpriteRenderer laserTelegraphRoot;
    public AudioSource laserLoop;
}
