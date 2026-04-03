using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class GameUtils
{
    public static Dictionary<CharacterStates, GameObject> AsDictionary(this WeaponAttachPoint[] weaponAttachPoints)
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
}