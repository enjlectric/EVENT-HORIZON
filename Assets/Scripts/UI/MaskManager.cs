using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class MaskManager : MonoBehaviour
{
    public static MaskManager instance;

    public BoxCollider2D bounds;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public static bool IsInBoundsV(Vector2 data)
    {
        if (data.y < instance.bounds.transform.position.y - 0.5 * instance.bounds.size.y)
        {
            return false;
        } else if (data.y > instance.bounds.transform.position.y + 0.5 * instance.bounds.size.y)
        {
            return false;
        }

        return true;
    }

    public static Vector3 GetPositionRelativeToCam(float x, float y)
    {
        var ib = instance.bounds.transform;
        return new Vector3(ib.position.x + instance.bounds.size.x * 0.5f * x, ib.position.y + 0.5f * instance.bounds.size.y * y);
    }

    public static bool IsInBounds(Vector2 data)
    {
        return IsInBoundsH(data) && IsInBoundsV(data);
    }

    public static bool IsInBoundsH(Vector2 data)
    {
        if (data.x < instance.bounds.transform.position.x - 0.5 * instance.bounds.size.x)
        {
            return false;
        }
        else if (data.x > instance.bounds.transform.position.x + 0.5 * instance.bounds.size.x)
        {
            return false;
        }
        return true;
    }

    public static Vector2 GetPosition(float position, EnemySpawn.ScreenEdge edge, float depth)
    {
        // This size.x, size.y is backwards for all but center and needs to be fixed for the next game.
        switch(edge)
        {
            default:
            case EnemySpawn.ScreenEdge.Top:
                return (Vector2)instance.bounds.transform.position + instance.bounds.size.y * new Vector2(position * 0.5f, 0.5f) + Vector2.up * (1 + depth);
            case EnemySpawn.ScreenEdge.Left:
                return (Vector2)instance.bounds.transform.position + instance.bounds.size.x * new Vector2(-0.5f, position * 0.5f) + Vector2.left * (1 + depth);
            case EnemySpawn.ScreenEdge.Bottom:
                return (Vector2)instance.bounds.transform.position + instance.bounds.size.y * new Vector2(position * 0.5f, -0.5f) + Vector2.down * (1 + depth);
            case EnemySpawn.ScreenEdge.Right:
                return (Vector2)instance.bounds.transform.position + instance.bounds.size.x * new Vector2(0.5f, position * 0.5f) + Vector2.right * (1 + depth);
            case EnemySpawn.ScreenEdge.Center:
                return (Vector2)instance.bounds.transform.position + instance.bounds.size * new Vector2(position, depth);
        }
    }
}
