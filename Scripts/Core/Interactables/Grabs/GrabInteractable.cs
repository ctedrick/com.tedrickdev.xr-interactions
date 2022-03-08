using System.Collections;
using TedrickDev.InteractionsToolkit.Poser;
using UnityEngine;

namespace TedrickDev.InteractionsToolkit.Core
{
    public class GrabInteractable : BaseInteractable
    {
        protected static void DetachInteractable(Transform interactable) => interactable.SetParent(null);

        protected void SetPose(PoserHand poserHand, PoseData poseData, float poseDuration)
        {
            PoseTransform[] pose;
            PoseTransform parentTransform;
            if (poserHand.Type == Handedness.Left) {
                pose = poseData.LeftJoints;
                parentTransform = poseData.LeftParentTransform;
            }
            else {
                pose = poseData.RightJoints;
                parentTransform = poseData.RightParentTransform;
            }
            
            poserHand.SetPose(pose, poseDuration);
            ApplyAttachmentTransform(parentTransform, poserHand.AttachTransform);
            transform.SetParent(poserHand.AttachTransform);

            StopAllCoroutines();
            StartCoroutine(AttachHandRoutine(transform, Vector3.zero, Quaternion.identity, poseDuration));
        }

        protected static IEnumerator AttachHandRoutine(Transform target, Vector3 positionEnd, Quaternion rotationEnd, float attachDuration)
        {
            var time = 0f;
            var startPosition = target.localPosition;
            var startRotation = target.localRotation;
            
            while (time < attachDuration) {
                target.localPosition = Vector3.Lerp(startPosition, positionEnd, time / attachDuration);
                target.localRotation = Quaternion.Lerp(startRotation, rotationEnd, time / attachDuration);
                time += Time.deltaTime;
                yield return null;
            }

            target.localPosition = positionEnd;
            target.localRotation = Quaternion.identity;
        }
        
        private static void ApplyAttachmentTransform(PoseTransform parentTransformData, Transform attachTransform)
        {
            var adjustedPosition = parentTransformData.LocalPosition * -1f;
            var adjustedRotation = Quaternion.Inverse(parentTransformData.LocalRotation);
            adjustedPosition = Quaternion.Euler(adjustedRotation.eulerAngles) * adjustedPosition;
            
            // Apply offset to hand attach transform
            attachTransform.localPosition = adjustedPosition;
            attachTransform.localRotation = adjustedRotation;
        }
    }
}