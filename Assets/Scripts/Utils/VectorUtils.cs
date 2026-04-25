using UnityEngine;

public static class VectorUtils
{
    public static Vector3 MultiplyVectors(Vector3 v1, Vector3 v2)
        => new(v1.x * v2.x,
            v1.y * v2.y,
            v1.z * v2.z);
}