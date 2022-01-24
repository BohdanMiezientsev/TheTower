using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGrappling : MonoBehaviour
{
    [SerializeField] private Grappling _grappling;

    [SerializeField] private Quaternion _desiredRotation;
    [SerializeField] private float _rotationSpeed = 5f;
    
    void Update()
    {
        if (!_grappling.IsGrappling())
        {
            _desiredRotation = transform.parent.rotation;
        }
        else
        {
            _desiredRotation = Quaternion.LookRotation(_grappling.GrapplePoint - transform.position);
        }
        
        transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRotation, Time.deltaTime * _rotationSpeed);
        
    }
}
