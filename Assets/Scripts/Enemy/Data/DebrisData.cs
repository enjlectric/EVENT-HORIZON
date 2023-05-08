using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/DebrisData", fileName = "DebrisData")]
public class DebrisData : EnemyData<EnemyConfig>
{
    public bool Invincible = false;
    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (Invincible)
        {
            behaviour.ForceInvulnerable(2);
        }
        base.OnUpdate(behaviour);
    }
    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        DeathSoundEffect.Play();
        if (deathFrame >= 0)
        {
            behaviour.SetFrame(deathFrame);
        }
        if (deathEffect != null)
        {
            Instantiate(deathEffect, behaviour.transform.position, Quaternion.identity);
            behaviour.spriteRenderer.transform.DOBlendableLocalMoveBy(new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-0.5f, 0.5f), 0), 0.8f).SetEase(Ease.OutQuad);
            behaviour.spriteRenderer.transform.DOBlendableLocalRotateBy(new Vector3(0, 0, Random.Range(-25.0f, 25.0f)), 0.8f).SetEase(Ease.OutQuad);

            //behaviour.transform.DOPunchScale(behaviour.transform.localScale, 0.2f);
            for (int i = 1; i < 9; i++)
            {
                behaviour.spriteRenderer.enabled = false;
                yield return new WaitForSeconds(0.04f);
                behaviour.spriteRenderer.enabled = true;
                yield return new WaitForSeconds(0.04f);
            }
        }
    }

    public IEnumerator KillForReal(EnemyBehaviour behaviour)
    {
        DeathSoundEffect.Play();
        GrantScore(behaviour);
        if (deathEffect != null)
        {
            Instantiate(deathEffect, behaviour.transform.position, Quaternion.identity);
            behaviour.spriteRenderer.transform.DOBlendableLocalMoveBy(new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-0.5f, 0.5f), 0), 0.8f).SetEase(Ease.OutQuad);
            behaviour.spriteRenderer.transform.DOBlendableLocalRotateBy(new Vector3(0, 0, Random.Range(-25.0f, 25.0f)), 0.8f).SetEase(Ease.OutQuad);

            //behaviour.transform.DOPunchScale(behaviour.transform.localScale, 0.2f);
            for (int i = 1; i < 9; i++)
            {
                behaviour.spriteRenderer.enabled = false;
                yield return new WaitForSeconds(0.04f);
                behaviour.spriteRenderer.enabled = true;
                yield return new WaitForSeconds(0.04f);
            }
        }
        References.DestroyObject(behaviour);
    }
}
