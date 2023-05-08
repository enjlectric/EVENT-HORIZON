using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy/DoorData", fileName = "DoorData")]
public class DoorData : EnemyData<EnemyConfig>
{
    public EnemyData DoorFrameData;
    public List<EnemyData> SpawnableEnemies = new List<EnemyData>();

    private EnemyBehaviour _doorFrame;

    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);

        _doorFrame = References.CreateObject<EnemyBehaviour>(DoorFrameData, behaviour.transform.position, Quaternion.identity);
        _doorFrame.spriteRenderer.sortingOrder = -100;
        _doorFrame.ForceInvulnerable(1);
    }

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (behaviour.dead)
        {
            return;
        }

        _doorFrame.ForceInvulnerable(1);

        if (behaviour.state == 0)
        {
            behaviour.transform.position = behaviour.transform.position + Vector3.left * 2 * Manager.deltaTime;
            Vector3 lim = MaskManager.GetPositionRelativeToCam(0.9f, 0);

            if (behaviour.transform.position.x < lim.x)
            {
                behaviour.transform.position = lim;
                behaviour.SwitchState(1);
            }
        } else if (behaviour.state == 1)
        {
            var mainGunCooldown = 1.9f + (behaviour._hp / health);
            var enemyCooldown = 0.9f + 1.3f * (behaviour._hp / health);
            if (behaviour.HasSurpassedState(mainGunCooldown, mainGunCooldown))
            {
                Shoot(behaviour);
            }
            if (behaviour.HasSurpassedState(enemyCooldown, 2 * enemyCooldown))
            {
                var enemy = Random.Range(0, 3);
                References.CreateObject<EnemyBehaviour>(SpawnableEnemies[enemy], _doorFrame.emitterContainer.emitters[0].transform.position, Quaternion.identity);
                Shoot(behaviour);
            }
            if (behaviour.HasSurpassedState(enemyCooldown * 2, 2 * enemyCooldown))
            {
                var enemy = Random.Range(0, 3);
                if (enemy == 2)
                {
                    enemy = 3;
                }
                References.CreateObject<EnemyBehaviour>(SpawnableEnemies[enemy], _doorFrame.emitterContainer.emitters[1].transform.position, Quaternion.identity);
                Shoot(behaviour);
            }
        }
        _doorFrame.transform.position = behaviour.transform.position;

        base.OnUpdate(behaviour);
    }

    public override IEnumerator OnKill(EnemyBehaviour behaviour)
    {
        foreach (var col in _doorFrame.GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        References.DestroyObjectsOfType<BulletBehaviour>();
        Manager.instance.player.ForceInvulnerable(2);
        CoroutineManager.Start(PlayerCutscene());
        _doorFrame.speed = Vector2.zero;
        behaviour.speed = new Vector2(Random.Range(-1.0f, 1.0f), 0.2f);
        yield return base.OnKill(behaviour);
    }

    private IEnumerator PlayerCutscene()
    {
        Manager.LockPlayerInput(true);
        yield return new WaitForSeconds(1);

        float t = 0;
        while (t < 3 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 4)
        {
            t += Time.deltaTime;
            float yAdd = 0;
            if (Mathf.Abs(Manager.instance.player.transform.position.y) > 0.2f)
            {
                yAdd = Manager.instance.player.transform.position.y * -0.35f;
            }
            Manager.instance.player._speed = (Vector2.right + yAdd * Vector2.up) * 250 * (t * t);
            yield return null;
        }
    }
}
