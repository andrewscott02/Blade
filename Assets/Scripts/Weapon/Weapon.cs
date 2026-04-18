using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
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
    private int _hitDivisions = 5;

    public Vector3 LastPosBase { get; private set; }
    public Vector3 LastPosTip { get; private set; }

    private Vector3 _movementBase;
    public Vector3 MovementBase => _movementBase;

    private Vector3 _movementTip;
    public Vector3 MovementTip => _movementTip;

    private List<Collider> _hitColliders = new();

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
        RaycastHit[] hitObjects = GetHitObjects().ToArray();

        foreach (RaycastHit hitObject in hitObjects)
        {
            if (!_hitColliders.Contains(hitObject.collider))
                Debug.Log($"{gameObject.name} Skipped hitting {hitObject.collider.gameObject.name}");
        }

        //_hitColliders = new();
    }

    private IEnumerable<RaycastHit> GetHitObjects()
    {
        List<RaycastHit> hitObjects = new();

        float interval = 1 / (float)_hitDivisions;

        for (int i = 0; i < _hitDivisions; i++)
        {
            float t = interval * i;

            foreach (RaycastHit hit in CheckBoxCollision(t))
            {
                if (IsNotThisObject(hit)
                && TryGetHittable(hit.collider.gameObject, out IHittable hittable)
                && !hitObjects.Contains(hit))
                {
                    hitObjects.Add(hit);
                    yield return hit;
                }
            }
        }


        //foreach (RaycastHit hit in TestCheckBoxCollision())
        //{
        //    if (IsNotThisObject(hit)
        //    && TryGetHittable(hit.collider.gameObject, out IHittable hittable)
        //    && !hitObjects.Contains(hit))
        //    {
        //        hitObjects.Add(hit);
        //        yield return hit;
        //    }
        //}
    }

    private bool IsNotThisObject(RaycastHit hit)
        => !transform.IsChildOf(hit.collider.transform)
        && !hit.collider.transform.IsChildOf(transform)
        && !hit.collider.transform != transform;

    private RaycastHit[] CheckBoxCollision(float t)
    {
        Vector3 centerStart = Vector3.Lerp(LastPosBase, LastPosTip, t);
        Vector3 centerEnd = Vector3.Lerp(_base.transform.position, _tip.transform.position, t);

        Vector3 halfExtents = new(_hitBoxSize.x, ((Vector3.Distance(LastPosBase, LastPosTip) / _hitDivisions)), _hitBoxSize.z);
        //halfExtents /= 2;

        Vector3 dir = centerEnd - centerStart;
        float distance = Vector3.Distance(centerStart, centerEnd);

        Quaternion orientation = dir != Vector3.zero
            ? Quaternion.LookRotation(dir, transform.up)
            : Quaternion.Lerp(_base.transform.rotation, _tip.transform.rotation, t);

        DebugBoxCast.SimpleDrawBoxCast(centerStart, halfExtents, dir, orientation, distance, Color.red);
        return Physics.BoxCastAll(centerStart, halfExtents, dir, orientation, distance, _hitLayers);
    }

    //public Vector3 centerEndTest;
    //public float maxDist = 5f;
    //private RaycastHit[] TestCheckBoxCollision()
    //{
    //    Vector3 centerStart = transform.position;
    //    Vector3 centerEnd = transform.position + centerEndTest;

    //    Vector3 halfExtents = new(_hitBoxSize.z, ((Vector3.Distance(LastPosBase, LastPosTip) / _hitDivisions)), _hitBoxSize.z);
    //    //halfExtents /= 2;

    //    Vector3 dir = centerEnd - centerStart;
    //    float distance = maxDist;

    //    Quaternion orientation = dir != Vector3.zero
    //        ? Quaternion.LookRotation(dir, transform.up)
    //        : transform.rotation;

    //    DebugBoxCast.SimpleDrawBoxCast(centerStart, halfExtents, dir, orientation, distance, Color.red);
    //    return Physics.BoxCastAll(centerStart, halfExtents, dir, orientation, distance, _hitLayers);
    //}

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

    private void OnCollisionEnter(Collision collision)
    {
        if (TryGetHittable(collision.gameObject, out IHittable hittable))
        {
            _hitColliders.Add(collision.collider);
            //float test = PosTest(collision.transform.position);

            //hittable.Hit();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (TryGetHittable(collision.gameObject, out IHittable hittable))
        {
            _hitColliders.Remove(collision.collider);
            //float test = PosTest(collision.transform.position);

            //hittable.Hit();
        }
    }

    //private bool CanHitOther(Collider collision, out IHittable hittable)
    //    => collision.gameObject.TryGetComponent(out hittable)
    //    && _hitLayers.ContainsLayer(collision.gameObject.layer);

    private bool TryGetHittable(GameObject gameObject, out IHittable hittable)
        => gameObject.TryGetComponent(out hittable)
        && _hitLayers.ContainsLayer(gameObject.layer);

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

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;

    //    float interval = 1 / (float)_hitDivisions;

    //    for (int i = 0; i < _hitDivisions; i++)
    //    {
    //        float t = interval * i;

    //        DrawGizmoBox(t);
    //    }
    //}

    //private void DrawGizmoBox(float t)
    //{
    //    Vector3 centerStart = Vector3.Lerp(LastPosBase, LastPosTip, t);
    //    Vector3 centerEnd = Vector3.Lerp(_base.transform.position, _tip.transform.position, t);
    //    Vector3 boxCenter = Vector3.Lerp(centerStart, centerEnd, 0.5f);

    //    Vector3 halfExtents = new(_hitBoxSize.z, ((Vector3.Distance(LastPosBase, LastPosTip) / _hitDivisions)), Vector3.Distance(centerStart, centerEnd));
    //    halfExtents /= 2;

    //    Vector3 dir = centerEnd - centerStart;

    //    Quaternion orientation = dir != Vector3.zero
    //        ? Quaternion.LookRotation(dir, transform.up)
    //        : Quaternion.Lerp(_base.transform.rotation, _tip.transform.rotation, t);

    //    Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, orientation*transform.rotation, transform.lossyScale);
    //    //Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, orientation, transform.lossyScale);
    //    Gizmos.matrix = rotationMatrix;


    //    Gizmos.DrawWireCube(boxCenter, halfExtents);
    //}
}
