using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour, IHittable
{
    [SerializeField]
    private LayerMask _hitLayers;

    [SerializeField]
    private GameObject _base;
    [SerializeField]
    private GameObject _tip;
    [SerializeField]
    private float _baseStrength;
    [SerializeField]
    private float _tipStrength;

    [SerializeField]
    private Vector3 _hitBoxSize = new(0.07f, 1.2f, 0.03f);
    [SerializeField]
    private int _hitDivisions = 10;

    public Vector3 LastPosBase { get; private set; }
    public Vector3 LastPosTip { get; private set; }

    private Vector3 _movementBase;
    public Vector3 MovementBase => _movementBase;


    private Vector3 _movementTip;
    public Vector3 MovementTip => _movementTip;

    private void FixedUpdate()
    {
        CheckAllCollisions();

        _movementBase = _base.transform.position - LastPosBase;
        _movementTip = _tip.transform.position - MovementTip;

        LastPosBase = _base.transform.position;
        LastPosTip = _tip.transform.position;
    }

    private void CheckAllCollisions()
    {
        List<RaycastHit> hitObjects = new();

        hitObjects.AddRange(CheckBoxCollision(1));

        for (int i = 0; i < _hitDivisions; i++)
        {
            float t = (1 / _hitDivisions) * i;

            hitObjects.AddRange(CheckBoxCollision(t));
        }
    }

    private RaycastHit[] CheckBoxCollision(float t)
    {
        Vector3 centerStart = Vector3.Lerp(LastPosBase, LastPosTip, t);
        Vector3 centerEnd = Vector3.Lerp(_base.transform.position, _tip.transform.position, t);
        Vector3 boxCenter = Vector3.Lerp(centerStart, centerEnd, 0.5f);

        Vector3 halfExtents = new(_hitBoxSize.z, ((Vector3.Distance(LastPosBase, LastPosTip) / _hitDivisions)), Vector3.Distance(centerStart, centerEnd));
        halfExtents /= 2;

        Vector3 dir = centerEnd - centerStart;
        Quaternion orientation = transform.rotation; //Fix: Doesn't work for rotation

        DrawBoxCast.DrawBoxCastBox(boxCenter, halfExtents, orientation, dir, float.PositiveInfinity, Color.red);
        return Physics.BoxCastAll(boxCenter, halfExtents, dir, orientation, maxDistance: float.PositiveInfinity, _hitLayers);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (CanHitOther(other, out IHittable hittable))
    //    {
    //        float t = PosTest(other.transform.position, _base.transform.position, _tip.transform.position);
    //        float strength = Mathf.Lerp(_baseStrength, _baseStrength, t);
    //        Vector3 direction = Vector3.Lerp(LastPosBase, LastPosTip, t);

    //        hittable.Hit(direction, other.transform.position, strength);
    //    }
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (CanHitOther(collision, out IHittable hittable))
    //    {
    //        float test = PosTest(collision.transform.position);

    //        //hittable.Hit();
    //    }
    //}

    private bool CanHitOther(Collider collision, out IHittable hittable)
        => collision.gameObject.TryGetComponent(out hittable)
        && _hitLayers.ContainsLayer(collision.gameObject.layer);

    private float PosTest(Vector3 pos, Vector3 A, Vector3 B)
    {
        Vector3 AP = pos - A;
        Vector3 AB = B - A;
        float distSqr = AB.sqrMagnitude;
        float d = Vector3.Dot(AP, AB) / distSqr;

        return d;
    }

    public void Hit(Vector3 direction, Vector3 position, float strength)
    {
        //throw new System.NotImplementedException();
    }
}
