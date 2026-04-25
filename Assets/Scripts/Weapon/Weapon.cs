using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour, IHittable
{
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private BoxCollider _boxCollider;
    [SerializeField]
    private CapsuleCollider _capsuleCollider;
    private float _baseCapsuleRadius;
    private Dictionary<int, BoxCollider> _colliderByInterval;
    private Dictionary<int, Vector3> _lastColliderPositionsByInterval;
    private Vector3 _baseBoxColliderSize;
    private Vector3 _individualColliderSize;

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

    //[SerializeField]
    //private AnimationCurve _capsuleWidthBySpeed = AnimationCurve.Constant(0, 10f, 1);

    public Vector3 LastPosBase { get; private set; }
    public Vector3 LastPosTip { get; private set; }

    private Vector3 _movementBase;
    public Vector3 MovementBase => _movementBase;

    private Vector3 _movementTip;
    public Vector3 MovementTip => _movementTip;

    private List<Collider> _hitColliders = new();

    [SerializeField]
    private Transform _ikTransformNonCombat;
    [SerializeField]
    private Transform _ikTransformCombat;

    private void Start()
    {
        _baseBoxColliderSize = _boxCollider.size;
        _baseCapsuleRadius = _capsuleCollider.radius;
        _individualColliderSize = _baseBoxColliderSize;
        _individualColliderSize.y = Vector3.Distance(_base.transform.position, _tip.transform.position) / _hitDivisions;

        _colliderByInterval = new();
        _lastColliderPositionsByInterval = new();

        float interval = 1 / (float)_hitDivisions;

        for (int i = 0; i < _hitDivisions; i++)
        {
            float t = interval * i;
            BoxCollider col = transform.AddComponent<BoxCollider>();

            col.center = Vector3.Lerp(_base.transform.localPosition, _tip.transform.localPosition, t);
            col.size = _individualColliderSize;

            _colliderByInterval.Add(i, col);
            _lastColliderPositionsByInterval.Add(i, col.center);
        }
    }

    internal void Init(GameObject ikHandleNonCombat, GameObject ikHandleCombat)
    {
        ikHandleNonCombat.transform.SetParent(_ikTransformNonCombat, false);
        ikHandleCombat.transform.SetParent(_ikTransformCombat, false);
    }

    private void Update()
    {
        transform.position = transform.parent.position;
        rb.centerOfMass = transform.parent.position - transform.position;
        CheckAllCollisions();
        TestCheckColliderSizes();

        _movementBase = _base.transform.position - LastPosBase;
        _movementTip = _tip.transform.position - LastPosTip;

        _hitColliders = new();
    }

    private void FixedUpdate()
    {
        LastPosBase = _base.transform.position;
        LastPosTip = _tip.transform.position;
    }

    private void TestCheckColliderSizes()
    {
        for (int i = 0; i < _hitDivisions; i++)
        {
            CheckColliderSize(i);
            _lastColliderPositionsByInterval[i] = _colliderByInterval[i].bounds.center;
        }


    }

    private void CheckColliderSize(int i)
    {
        Vector3 centerStart = _lastColliderPositionsByInterval[i];
        Vector3 centerEnd = _colliderByInterval[i].bounds.center;
        //Vector3 halfExtents = new(_hitBoxSize.x, Vector3.Distance(LastPosBase, LastPosTip) / _hitDivisions, _hitBoxSize.z);

        Vector3 dir = centerEnd - centerStart;

        Vector3 halfExtents = _individualColliderSize + new Vector3(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z));
        float distance = Vector3.Distance(centerStart, centerEnd);

        Quaternion orientation = dir != Vector3.zero
            ? Quaternion.LookRotation(dir, transform.up)
            : _base.transform.rotation;

        _colliderByInterval[i].size = halfExtents;

        //DebugBoxCast.SimpleDrawBoxCast(centerStart, halfExtents, dir, orientation, distance, Color.red);
        //return Physics.BoxCastAll(centerStart, halfExtents, dir, orientation, distance, _hitLayers);
    }

    private void CheckAllCollisions()
    {
        RayCastHitInfo[] hitObjects = GetHitObjects().ToArray();

        foreach (RayCastHitInfo hitInfo in hitObjects)
        {
            if (!_hitColliders.Contains(hitInfo.rayHit.collider))
            {
                //TODO: order by distance (get lerp pos along initial and target to find distance from sword)
                ApplySkippedForce(hitInfo);
            }
        }
    }

    #region Collision Detection

    #region Raycast Collision Detection

    private IEnumerable<RayCastHitInfo> GetHitObjects()
    {
        List<RaycastHit> hitObjects = new();

        float interval = 1 / (float)_hitDivisions;

        for (int i = 0; i < _hitDivisions; i++)
        {
            float t = interval * i;

            Vector3 centerStart = Vector3.Lerp(LastPosBase, LastPosTip, t);
            Vector3 centerEnd = Vector3.Lerp(_base.transform.position, _tip.transform.position, t);

            foreach (RaycastHit hit in CheckBoxCollision(t, centerStart, centerEnd))
            {
                if (IsNotThisObject(hit)
                && TryGetHittable(hit.collider.gameObject, out IHittable hittable)
                && !hitObjects.Contains(hit))
                {
                    hitObjects.Add(hit);
                    yield return new(hit, centerStart, centerEnd);
                }
            }
        }
    }

    private bool IsNotThisObject(RaycastHit hit)
        => !transform.IsChildOf(hit.collider.transform)
        && !hit.collider.transform.IsChildOf(transform)
        && !hit.collider.transform != transform;

    private RaycastHit[] CheckBoxCollision(float t, Vector3 centerStart, Vector3 centerEnd)
    {
        Vector3 halfExtents = new(_hitBoxSize.x, Vector3.Distance(LastPosBase, LastPosTip) / _hitDivisions, _hitBoxSize.z);

        Vector3 dir = centerEnd - centerStart;
        float distance = Vector3.Distance(centerStart, centerEnd);

        Quaternion orientation = dir != Vector3.zero
            ? Quaternion.LookRotation(dir, transform.up)
            : Quaternion.Lerp(_base.transform.rotation, _tip.transform.rotation, t);

        DebugBoxCast.SimpleDrawBoxCast(centerStart, halfExtents, dir, orientation, distance, Color.red);
        return Physics.BoxCastAll(centerStart, halfExtents, dir, orientation, distance, _hitLayers);
    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (TryGetHittable(collision.gameObject, out IHittable hittable))
        {
            _hitColliders.Add(collision.collider);
            //hittable.Hit();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (TryGetHittable(collision.gameObject, out IHittable hittable))
        {
            if (!_hitColliders.Contains(collision.collider))
                _hitColliders.Add(collision.collider);
            //hittable.Hit();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (TryGetHittable(collision.gameObject, out IHittable hittable))
        {
            _hitColliders.Remove(collision.collider);
        }
    }

    private bool TryGetHittable(GameObject gameObject, out IHittable hittable)
        => gameObject.TryGetComponent(out hittable)
        && _hitLayers.ContainsLayer(gameObject.layer);

    #endregion

    public float forceTest = 1f;
    public float forceTestSelf = 1f;

    private void ApplySkippedForce(RayCastHitInfo hitInfo)
    {
        Debug.Log($"{gameObject.name} Skipped hitting {hitInfo.rayHit.collider.gameObject.name}");

        Vector3 dir = hitInfo.targetPos - hitInfo.initialPos;
        Vector3 force = dir.normalized * dir.magnitude * forceTest;// * Time.fixedDeltaTime;

        hitInfo.rayHit.rigidbody.AddForceAtPosition(force, hitInfo.rayHit.point);

        //TODO: self collision is really bad
        //Maybe revert position to start position if block is strong enough

        Vector3 dirSelf = hitInfo.initialPos - hitInfo.targetPos;
        Vector3 forceSelf = dirSelf.normalized * dirSelf.magnitude * forceTestSelf;// * Time.fixedDeltaTime;

        rb.AddForceAtPosition(forceSelf, hitInfo.rayHit.point);
        _hitColliders.Remove(hitInfo.rayHit.collider);
    }

    public void Hit(Vector3 direction, Vector3 position, float strength)
    {
        //throw new System.NotImplementedException();
    }

    internal void StartAttack(AttackHitInfo hitInfo)
    {
        //_boxCollider.size = _baseBoxColliderSize *= hitInfo.ColliderScale;
        //_capsuleCollider.radius = _baseCapsuleRadius *= hitInfo.ColliderScale;
    }

    internal void EndAttack()
    {
        //_boxCollider.size = _baseBoxColliderSize;
        //_capsuleCollider.radius = _baseCapsuleRadius;
    }
}
