using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgrammableMovement
{
    public float speed;
    public float angularSpeed;
    public float duration;
}

[CreateAssetMenu(menuName = "Game/Enemy/ProgData", fileName = "ProgData")]
public class ProgrammableData : TankData
{
    public List<ProgrammableMovement> MovementList = new List<ProgrammableMovement>();

    public bool loops = false;

    public override void OnStart(EnemyBehaviour behaviour)
    {
        base.OnStart(behaviour);
        behaviour.SwitchSubstate(99);
        behaviour.startSpeed = behaviour.speed.normalized;
    }

    public override void OnUpdate(EnemyBehaviour behaviour)
    {
        if (behaviour.dead)
        {
            return;
        }

        var lastTimer = behaviour.stateTimer - Manager.deltaTime;
        var timer = behaviour.stateTimer;

        behaviour.speed = Vector2.zero;
        foreach (var movement in MovementList)
        {
            if (timer > 0 && timer <= movement.duration)
            {
                var diff = timer - Mathf.Max(lastTimer, 0);
                //behaviour.TiltSprite(diff * movement.angularSpeed, behaviour.transform);
                behaviour.startSpeed = Quaternion.Euler(0, 0, (diff * movement.angularSpeed)) * behaviour.startSpeed;

                behaviour.speed += movement.speed * behaviour.startSpeed;
            }
            if (timer > movement.duration && lastTimer > 0 && lastTimer <= movement.duration)
            {
                var diff = (movement.duration - lastTimer);
                //behaviour.TiltSprite(diff * movement.angularSpeed, behaviour.transform);
                behaviour.startSpeed = Quaternion.Euler(0, 0, (diff * movement.angularSpeed)) * behaviour.startSpeed;

                behaviour.speed += movement.speed * behaviour.startSpeed;
            }
            timer -= movement.duration;
            lastTimer -= movement.duration;
        }

        if (timer > 0)
        {
            if (loops)
            {
                behaviour.stateTimer = timer;
            } else
            {
                var diff = Mathf.Min(Manager.deltaTime, behaviour.stateTimer);
                //behaviour.TiltSprite(diff * MovementList[MovementList.Count - 1].angularSpeed, behaviour.transform);
                behaviour.startSpeed = Quaternion.Euler(0, 0, diff * MovementList[MovementList.Count - 1].angularSpeed) * behaviour.startSpeed;
                behaviour.speed += MovementList[MovementList.Count - 1].speed * behaviour.startSpeed;
            }
        }

        if (behaviour.substateTimer >= shotInterval && shotInterval > 0)
        {
            Shoot(behaviour);
            behaviour.substateTimer -= shotInterval;
        }

        base.OnUpdate(behaviour);
    }
}