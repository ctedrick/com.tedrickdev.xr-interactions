using UnityEngine;

namespace TedrickDev.XRPoser.Interactions
{
    public class TwoHandGrab : GrabMode
    {
        private bool isPrimaryActive;
        private bool isSecondaryActive;

        private PoserHand primaryHand;
        private PoserHand secondaryHand;
        private GrabZone secondaryGrabZone;
        
        private Transform secondaryHandParent;
        private Vector3 secondaryHandOriginalLocalPosition;
        private Quaternion secondaryHandOriginalLocalRotation;

        private Transform secondaryHandContainer;

        public override void ApplyPose(GrabZone grabber)
        {
            // Must grab primary first before grabbing secondary
            if (!isPrimaryActive && grabber.GrabZoneType == GrabZoneType.Secondary) return;

            // Valid primary grab
            if (!isPrimaryActive && grabber.GrabZoneType == GrabZoneType.Primary) {
                isPrimaryActive = true;

                primaryHand = grabber.Hand;
                SetPose(grabber);
                
            // Swap primary to other hand
            } else if (isPrimaryActive && grabber.Hand != primaryHand && grabber.GrabZoneType == GrabZoneType.Primary) {
                primaryHand.ApplyDefaultPose();
                primaryHand = grabber.Hand;
                SetPose(grabber);
            }
            
            // Primary is Active and Secondary is not...attach hand
            else if (isPrimaryActive && !isSecondaryActive && grabber.GrabZoneType == GrabZoneType.Secondary) {
                isSecondaryActive = true;

                secondaryGrabZone = grabber;
                secondaryHand = grabber.Hand;
                
                secondaryHandParent = grabber.Hand.transform.parent;
                var secondaryHandTransform = secondaryHand.transform;
                secondaryHandOriginalLocalPosition = secondaryHandTransform.localPosition;
                secondaryHandOriginalLocalRotation = secondaryHandTransform.localRotation;
                
                if (!secondaryHandContainer) {
                    secondaryHandContainer = new GameObject("PoserHandParent").transform;
                    secondaryHandContainer.SetParent(grabber.GrabTransform);
                }
                
                secondaryHandContainer.localPosition = grabber.ParentTransform.LocalPosition;
                secondaryHandContainer.localRotation = grabber.ParentTransform.LocalRotation;
                secondaryHand.transform.SetParent(secondaryHandContainer);
                
                grabber.Hand.SetPose(grabber.Pose, PoseDuration);
                StartCoroutine(AttachHandRoutine(grabber.Hand.transform, Vector3.zero, Quaternion.identity));
              
            // Release secondary, reattach to XRController hand
            } else if (isPrimaryActive && isSecondaryActive && grabber.GrabZoneType == GrabZoneType.Secondary) {
                isSecondaryActive = false;

                // Add back to hand with original local pos/rot
                grabber.Hand.transform.SetParent(secondaryHandParent);
                
                grabber.Hand.ApplyDefaultPose();
                StartCoroutine(AttachHandRoutine(grabber.Hand.transform, secondaryHandOriginalLocalPosition, secondaryHandOriginalLocalRotation));
                
            // Release primary with secondary still active
            } else if (isPrimaryActive && grabber.GrabZoneType == GrabZoneType.Primary) {
                isPrimaryActive = false;
                isSecondaryActive = false;
                
                primaryHand.ApplyDefaultPose();
                transform.SetParent(null);

                // Add back to hand with original local pos/rot
                if (!secondaryHand) return;
                secondaryHand.transform.SetParent(secondaryHandParent);
                secondaryHand.ApplyDefaultPose();
                StartCoroutine(AttachHandRoutine(secondaryHand.transform, secondaryHandOriginalLocalPosition, secondaryHandOriginalLocalRotation));
            }
        }

        private void Update()
        {
            if (!isSecondaryActive) return;

            if (secondaryGrabZone.IsOutsideDetachRadius(secondaryHandParent)) {
                isSecondaryActive = false;
                
                secondaryHand.transform.SetParent(secondaryHandParent);
                secondaryHand.ApplyDefaultPose();
                StartCoroutine(AttachHandRoutine(secondaryHand.transform, secondaryHandOriginalLocalPosition, secondaryHandOriginalLocalRotation));
            }
        }
    }
}