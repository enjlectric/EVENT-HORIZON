using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/BulletPattern", fileName = "BulletPattern")]
public class BulletPatternData : ScriptableObject
{
    public Vector2 axisFlipOnCompletion = Vector2.one;
    [MinMaxSlider(-180, 180, true)]
    public Vector2 rotationOnCompletion = Vector2.zero;

    public AnimationCurve offsetOverLifetime = AnimationCurve.Constant(0, 1, 1);
    public float offsetOverLifetimeMultiplier = 1;

    public enum BulletListType { Sequence, Reverse, Random }

    public ParticleSystem Telegraph;
    public float TelegraphLength = 0;
    public SFX TelegraphSound = SFX.None;
    public bool AlignTelegraph;

    //[AssetList(AssetNamePrefix = "EnemyBullet")]
    public List<BulletData> bullets = new List<BulletData>();
    public BulletListType bulletListSearchType;

    public List<float> bulletSpawnInterval = new List<float>() { 1 / 180 };
    public float spawnInterval;
    public float bulletCount;

    [FoldoutGroup("Offsets")]
    [InlineProperty]
    [LabelText("X")]
    [LabelWidth(20)]
    public ModularCurve xOffsetCurve;
    [FoldoutGroup("Offsets")]
    [InlineProperty]
    [LabelText("Y")]
    [LabelWidth(20)]
    public ModularCurve yOffsetCurve;

    [FoldoutGroup("Speed")]
    [InlineProperty]
    [LabelText("X")]
    [LabelWidth(20)]
    public ModularCurve bulletXDirectionModCurve;
    [FoldoutGroup("Speed")]
    [InlineProperty]
    [LabelText("Y")]
    [LabelWidth(20)]
    public ModularCurve bulletYDirectionModCurve;
    [FoldoutGroup("Speed")]
    [InlineProperty]
    [LabelText("*")]
    [LabelWidth(20)]
    public ModularCurve bulletSpeedModCurve;

    public IEnumerator Evaluate(Transform root, float damageMult = 1)
    {
        int spawnIntervalIndex = 0;
        float curvePoint = 0;
        int bulletSequence = 0;
        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        float startTime = Time.time;
        float totalTime = startTime;

        for (int i = 0; i < bulletCount; i++)
        {
            SpawnBullet(root, curvePoint, bulletSequence, i / bulletCount, damageMult);
            curvePoint += bulletSpawnInterval[spawnIntervalIndex];
            curvePoint %= 1;
            spawnIntervalIndex = (spawnIntervalIndex + 1) % bulletSpawnInterval.Count;
            bulletSequence = (bulletSequence + 1) % bullets.Count;

            totalTime += spawnInterval;

            if (spawnInterval > 0.0001f && totalTime > Time.time)
            {
                yield return wait;
            }
        }

        root.Rotate(0, 0, rotationOnCompletion.Random());
        root.transform.localScale = new Vector3(root.transform.localScale.x * axisFlipOnCompletion.x, root.transform.localScale.y * axisFlipOnCompletion.y, root.transform.localScale.z);
    }

    private void SpawnBullet(Transform root, float time, int bulletSequence, float totalTime, float damageMult = 1)
    {

        float xSpawnOffset = xOffsetCurve.Get(time) * root.transform.localScale.x;
        float ySpawnOffset = yOffsetCurve.Get(time) * root.transform.localScale.y;
        Vector2 spawnDirection = new Vector2(bulletXDirectionModCurve.Get(time), bulletYDirectionModCurve.Get(time)).normalized;
        spawnDirection.Scale(root.transform.localScale);
        float speed = bulletSpeedModCurve.Get(time);

        BulletData data = bullets[bulletSequence];
        switch (bulletListSearchType)
        {
            case BulletListType.Reverse: data = bullets[bullets.Count - 1 - bulletSequence]; break;
            case BulletListType.Random: data = bullets[Random.Range(0, bullets.Count)]; break;
        }

        Quaternion rotation = root.rotation;
        if (spawnDirection != Vector2.zero)
        {
            rotation = root.rotation * Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, spawnDirection));
        }

        var b = References.CreateObject<BulletBehaviour>(data, root.position + root.rotation * new Vector3(xSpawnOffset, ySpawnOffset, 0) * offsetOverLifetimeMultiplier * offsetOverLifetime.Evaluate(totalTime), rotation);
        b.spriteRenderer.transform.localScale = Vector3.one;
        if (data.isLaser)
        {
            b.transform.SetParent(root, true);
        }
        else
        {
            References.ResetParent<BulletBehaviour>(b.gameObject);
        }
        b.speedMod = b.speedMod * speed;
        b.DamageMultiplier = damageMult;
    }
}
