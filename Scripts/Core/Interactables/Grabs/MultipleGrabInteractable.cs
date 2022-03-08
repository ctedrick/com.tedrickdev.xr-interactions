using TedrickDev.InteractionsToolkit.Poser;
using UnityEngine;

namespace TedrickDev.InteractionsToolkit.Core
{
    public class MultipleGrabInteractable : GrabInteractable
    {
        [Header("Poses")]
        [SerializeField] private PoseData primaryPose;
        [SerializeField] private PoseData secondaryPose;
        
        [Header("Animation Settings")]
        [SerializeField] protected float AttachDuration = 0.1f;

        [Header("Grab Zones")]
        [SerializeField] protected Collider PrimaryZone;
        [SerializeField] protected Collider SecondaryZone;

        private PoserHand primaryHand;
        private PoserHand secondaryHand;

        private BaseInteractor secondaryInteractor;

        private GameObject secondaryPoseContainer;
        private Transform secondaryHandParent;
        private Vector3 secondaryOriginalLocalPosition;
        private Quaternion secondaryOriginalLocalRotation;

        protected void Awake()
        {
            AllowMultipleSelection = true;
            
            if (LogMessages()) return;

            secondaryPoseContainer = new GameObject("Secondary Pose Container");
            secondaryPoseContainer.transform.SetParent(transform);
            secondaryPoseContainer.transform.localPosition = Vector3.zero;
            secondaryPoseContainer.transform.localRotation = Quaternion.identity;

            PrimaryZone.isTrigger = true;
            PrimaryZone.enabled = true;
            
            SecondaryZone.isTrigger = true;
            SecondaryZone.enabled = false;
        }

        public override void HandleSelectEnter(BaseInteractor interactor)
        {
            var newHand = interactor.GetComponent<PoserHand>();
            if (newHand) {
                if (primaryHand == null) {
                    primaryHand = newHand;
                    PrimaryZone.enabled = false;
                    SecondaryZone.enabled = true;
                    SetPose(primaryHand, primaryPose, AttachDuration);
                }
                else {
                    secondaryInteractor = interactor;
                    secondaryHand = newHand;
                    secondaryHandParent = newHand.transform.parent;
                    SecondaryZone.enabled = false;
                    
                    SetSecondaryPoseContainer(secondaryHand);
                    secondaryHand.SetPose(secondaryPose);
                    StartCoroutine(AttachHandRoutine(secondaryHand.transform, Vector3.zero, Quaternion.identity, AttachDuration));
                }
            }
        }
        
        public override void HandleSelectExit(BaseInteractor interactor)
        {
            var newHand = interactor.GetComponent<PoserHand>();
            if (newHand) {
                if (newHand == primaryHand) {
                    primaryHand.SetPose(interactor.DefaultPose);
                    primaryHand = null;
                    PrimaryZone.enabled = true;
                    SecondaryZone.enabled = false;
                    transform.SetParent(null);

                    if (secondaryHand) {
                        DetachSecondaryHand(interactor);
                        interactor.InteractionManager.ForceDeselect(secondaryInteractor);
                    }
                }
                else {
                    DetachSecondaryHand(interactor);
                    interactor.InteractionManager.ForceDeselect(secondaryInteractor);
                }
            }
        }
        
        private void SetSecondaryPoseContainer(PoserHand hand)
        {
            if (hand.Type == Handedness.Left) {
                secondaryPoseContainer.transform.localPosition = secondaryPose.LeftParentTransform.LocalPosition;
                secondaryPoseContainer.transform.localRotation = secondaryPose.LeftParentTransform.LocalRotation;
            }
            else {
                secondaryPoseContainer.transform.localPosition = secondaryPose.RightParentTransform.LocalPosition;
                secondaryPoseContainer.transform.localRotation = secondaryPose.RightParentTransform.LocalRotation;
            }
            
            secondaryHand.transform.SetParent(secondaryPoseContainer.transform);
        }

        private void DetachSecondaryHand(BaseInteractor interactor)
        {
            SecondaryZone.enabled = true;

            secondaryHand.SetPose(interactor.DefaultPose);

            var secondaryHandTransform = secondaryHand.transform;
            secondaryHand.transform.SetParent(secondaryHandParent);
            StartCoroutine(AttachHandRoutine(secondaryHandTransform.transform,
                                             secondaryOriginalLocalPosition,
                                             secondaryOriginalLocalRotation, AttachDuration));
            secondaryHand = null;
            secondaryHandParent = null;
            secondaryOriginalLocalPosition = Vector3.zero;
            secondaryOriginalLocalRotation = Quaternion.identity;
        }
        
        private bool LogMessages()
        {
            if (!PrimaryZone) {
                Debug.LogError($"{nameof(PrimaryZone)} is null. {gameObject.name}");
                enabled = false;
                return true;
            }

            if (!SecondaryZone) {
                Debug.LogError($"{nameof(SecondaryZone)} is null. {gameObject.name}");
                enabled = false;
            }

            if (!primaryPose) {
                Debug.LogError($"{nameof(primaryPose)} is null. {gameObject.name}");
                enabled = false;
                return true;
            }

            if (!secondaryPose) {
                Debug.LogError($"{nameof(secondaryPose)} is null. {gameObject.name}");
                enabled = false;
            }

            return false;
        }
    }
}