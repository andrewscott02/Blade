using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static CharacterAnimatorEventsListener;
using static UnityEditor.Experimental.GraphView.GraphView;

public static class GameUtils
{
    public static Dictionary<CharacterStates, GameObject> AsDictionary(this AttachPointInfo[] weaponAttachPoints)
        => weaponAttachPoints.ToDictionary(kvp => kvp.State, kvp => kvp.AttachPoint);

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static bool TryInvoke(this CharacterAnimatorEventDelegate callFunc)
    {
        if (callFunc?.GetInvocationList()?.Length > 0)
        {
            callFunc.Invoke();
            return true;
        }

        return false;
    }

    public static bool TryInvoke(this GuardInfoEventDelegate callFunc, AttackGuardChangeInfo changeInfo)
    {
        if (callFunc?.GetInvocationList()?.Length > 0)
        {
            callFunc.Invoke(changeInfo);
            return true;
        }
        return false;
    }

    public static bool TryInvoke(this HitInfoEventDelegate callFunc, AttackHitInfo changeInfo)
    {
        if (callFunc?.GetInvocationList()?.Length > 0)
        {
            callFunc.Invoke(changeInfo);
            return true;
        }
        return false;
    }

    public static bool ContainsLayer(this LayerMask mask, int layer)
        => (mask & (1 << layer)) != 0;
}