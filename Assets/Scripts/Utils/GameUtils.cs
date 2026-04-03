using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class GameUtils
{
    public static Dictionary<CharacterStates, GameObject> AsDictionary(this WeaponAttachPoint[] weaponAttachPoints)
        => weaponAttachPoints.ToDictionary(kvp => kvp.State, kvp => kvp.AttachPoint);
}