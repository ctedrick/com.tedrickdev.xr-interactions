using System;
using TedrickDev.InteractionsToolkit.Core;
using UnityEngine;

namespace TedrickDev.InteractionsToolkit.Utility
{
    public class RaycastUtility<T> where T : BaseInteractable
    {
        public event Action<Transform> OnHoverEnter;
        public event Action<Transform> OnHoverExit;
        
        public bool IsHovering{ get; private set; }
        
        private Transform source;
        private LayerMask layerMask;
        
        private float range;
        private float radius;
        private bool isSphere;

        private bool isHovering;
        private Transform currentTarget;
        
        public RaycastUtility(Transform source, LayerMask layerMask, float range)
        {
            this.source = source;
            this.range = range;
            this.layerMask = layerMask;
        }
        
        public RaycastUtility(Transform source, float range, float radius)
        {
            this.source = source;
            this.range = range;
            this.radius = radius;
            isSphere = true;
        }

        public void FixedUpdate()
        {
            var hitInfo = HitInfo();
            if (hitInfo.transform != null) {
                var target = hitInfo.transform;

                if (currentTarget != null && target != currentTarget) OnHoverExit?.Invoke(currentTarget);
                
                if (target != currentTarget) {
                    currentTarget = hitInfo.transform;
                    IsHovering = true;
                    OnHoverEnter?.Invoke(currentTarget);
                }
            }      
            else {
                if (currentTarget != null) {
                    OnHoverExit?.Invoke(currentTarget);
                    currentTarget = null;
                }
            }
        }

        private RaycastHit HitInfo()
        {
            RaycastHit hit;

            if (isSphere) {
                if (Physics.SphereCast(source.position, radius, source.TransformDirection(Vector3.forward), out hit, layerMask)) { }
            }
            else {
                if (Physics.Raycast(source.position, source.TransformDirection(Vector3.forward), out hit, range, layerMask)) { }
            }

            return hit;
        }
    }
}