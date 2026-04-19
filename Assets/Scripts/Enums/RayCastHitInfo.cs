using UnityEngine;

public struct RayCastHitInfo
{
    public Vector3 initialPos;
    public Vector3 targetPos;

    public RaycastHit rayHit;

    public RayCastHitInfo(RaycastHit rayHit, Vector3 initialPos, Vector3 targetPos)
    {
        this.rayHit = rayHit;
        this.initialPos = initialPos;
        this.targetPos = targetPos;
    }
}