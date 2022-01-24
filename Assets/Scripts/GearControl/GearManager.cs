using System;
using System.Collections.Generic;
using MovementControl;
using UnityEngine;

namespace GearControl
{
    public class GearManager : MonoBehaviour
    {
        [SerializeField] private float swaySize;
        [SerializeField] private float swaySmooth;
        
        public Transform swayHolder;

        private void LateUpdate()
        {
            if (!MovementManager.IsLookUnlocked)
            {
                return;
            }

            var mouseDelta = -new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            var localPosition = swayHolder.localPosition;
            
            localPosition += (Vector3) mouseDelta * swaySize;
            localPosition = Vector3.Lerp(localPosition, Vector3.zero, swaySmooth * Time.deltaTime);
            
            swayHolder.localPosition = localPosition;
        }
    }
}