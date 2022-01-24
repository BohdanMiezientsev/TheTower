using System;
using System.Collections;
using System.Collections.Generic;
using MovementControl;
using UnityEngine;
using UnityEngine.Serialization;

public class Grappling : MonoBehaviour
{

    [SerializeField] private LayerMask _whatIsGrappled;
    [SerializeField] private Transform _grappleTip;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _player;
    [SerializeField] private float _maxDistance;
    
    private SpringJoint _joint;
    private LineRenderer _lineRenderer;
    private bool _inAction;
    
    public Vector3 GrapplePoint { get; private set; }
    public bool IsGrappling() => _joint != null;
    

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _inAction = false;
        _lineRenderer.positionCount = 0;
        Destroy(_joint);
    }

    private void Update()
    {
        if (!MovementManager.IsLookUnlocked)
        {
            return;
        }
        
        if (PlayerInputs.Mouse0 && !_inAction)
        {
            StartGrapple();
        }
        else if (!PlayerInputs.Mouse0 && _inAction)
        {
            StopGrapple();
        }

    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartGrapple()
    {
        _inAction = true;
        if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out var hit, _maxDistance, _whatIsGrappled))
        {
            GrapplePoint = hit.point;
            _joint = _player.gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.connectedAnchor = GrapplePoint;

            float distanceFromPoint = Vector3.Distance(_player.position, GrapplePoint);
            
            _joint.maxDistance = distanceFromPoint * 0.8F;
            _joint.minDistance = distanceFromPoint * 0.25F;

            _joint.spring = 4.5F;
            _joint.damper = 7F;
            _joint.massScale = 4.5F;

            _lineRenderer.positionCount = 2;

        }
    }

    private void StopGrapple()
    {
        _inAction = false;
        _lineRenderer.positionCount = 0;
        Destroy(_joint);
    }

    private void DrawRope()
    {
        // dont draw if not grappled
        if (!_joint) return;
        _lineRenderer.SetPosition(0, _grappleTip.position);
        _lineRenderer.SetPosition(1, GrapplePoint);
    }
}
 