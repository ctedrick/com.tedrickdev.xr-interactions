using System;
using TedrickDev.HandPoser.Poser;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TedrickDev.HandPoser.Interactions
{
    public enum GrabZoneType {Primary, Secondary }

    public class InteractionZone : MonoBehaviour
    {
        public event Action<InteractionZone> OnInteract;

        [SerializeField] private InputActionReference leftInputAction;
        [SerializeField] private InputActionReference rightInputAction;

        [SerializeField] private Collider grabCollider;
        
        [SerializeField] public PoserTool PoserTool;

        [Space] [SerializeField] private GrabZoneType grabZoneType;

        [SerializeField] private float detachRadius = 0.2f;

        [SerializeField] private bool showGizmos;
        
        public PoserHand Hand{ get; private set; }
        public PoseTransform[] Pose{ get; private set; }
        public GrabZoneType GrabZoneType => grabZoneType;
        public PoseTransform ParentTransform{ get; private set; }
        public Transform GrabTransform => transform;

        private PoserHand leftHoverHand;
        private PoserHand rightHoverHand;

        private PoseData poseData;

        private void Awake()
        {
            if (!grabCollider) {
                Debug.LogError($"{nameof(grabCollider)} is null. {gameObject}");
                enabled = false;
                return;
            }

            if (leftInputAction is {asset: { }}) leftInputAction.asset.Enable();
            if (rightInputAction is {asset: { }}) rightInputAction.asset.Enable();

            if (leftInputAction != null) leftInputAction.action.performed += context => OnLeftSelectPress();
            if (rightInputAction != null) rightInputAction.action.performed += context => OnRightSelectPress();

            grabCollider.isTrigger = true;
            poseData = PoserTool.PoseData;
        }

        public bool IsOutsideDetachRadius(Transform xrController)
        {
            return Vector3.Distance(grabCollider.bounds.center, xrController.position) > detachRadius;
        }

        private void OnLeftSelectPress()
        {
            if (!leftHoverHand) return;

            Hand = leftHoverHand;
            Pose = poseData.LeftJoints;
            ParentTransform = poseData.LeftParentTransform;

            OnInteract?.Invoke(this);
        }

        private void OnRightSelectPress()
        {
            if (!rightHoverHand) return;
            
            Hand = rightHoverHand;
            Pose = poseData.RightJoints;
            ParentTransform = poseData.RightParentTransform;
            
            OnInteract?.Invoke(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            var hand = other.gameObject.GetComponentInChildren<PoserHand>();
            if (!hand) return;
            
            if (hand.Type == Handedness.Left) leftHoverHand = hand;
            else rightHoverHand = hand;
        }

        private void OnTriggerExit(Collider other)
        {
            var hand = other.gameObject.GetComponentInChildren<PoserHand>();
            if (!hand) return;

            if (hand.Type == Handedness.Left) leftHoverHand = null;
            else rightHoverHand = null;
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos || !grabCollider) return;
            if (GrabZoneType == GrabZoneType.Primary) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(grabCollider.bounds.center, detachRadius);
        }
    }
}