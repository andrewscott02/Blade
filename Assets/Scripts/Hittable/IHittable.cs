using UnityEngine;

public interface IHittable
{
    public void Hit(Vector3 direction, Vector3 position/*, AttackInfo*/, float strength);
}