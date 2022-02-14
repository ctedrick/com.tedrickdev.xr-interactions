using System.Collections;
using TedrickDev.Utilities;
using UnityEngine;

namespace TedrickDev.XRPoser.Interactions
{
    public abstract class GrabMode : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] protected float PoseDuration = 0.2f;
        [SerializeField] protected float AttachDuration = 0.1f;
        
        public abstract void ApplyPose(GrabZone grabber);

        protected IEnumerator AttachHandRoutine(Transform target, Vector3 positionEnd, Quaternion rotationEnd)
        {
            var time = 0f;
            var startPosition = target.localPosition;
            var startRotation = target.localRotation;
            
            while (time < AttachDuration) {
                target.localPosition = Vector3.Lerp(startPosition, positionEnd, time / AttachDuration);
                target.localRotation = Quaternion.Lerp(startRotation, rotationEnd, time / AttachDuration);
                time += Time.deltaTime;
                yield return null;
            }

            target.localPosition = positionEnd;
            target.localRotation = Quaternion.identity;
        }
        
        protected void SetPose(GrabZone grabber)
        {
            grabber.Hand.SetPose(grabber.Pose, PoseDuration);
            ApplyAttachmentTransform(grabber.ParentTransform, grabber.Hand.AttachTransform);
            transform.SetParent(grabber.Hand.AttachTransform);

            StopAllCoroutines();
            StartCoroutine(AttachHandRoutine(transform, Vector3.zero, Quaternion.identity));
        }
        
        private static void ApplyAttachmentTransform(PoseTransform parentTransformData, Transform attachTransform)
        {
            // https://youtu.be/H-qnAHB1AMw?t=765
            var adjustedPosition = parentTransformData.LocalPosition * -1f;
            var adjustedRotation = Quaternion.Inverse(parentTransformData.LocalRotation);

            adjustedPosition = adjustedPosition.RotatePointAroundPivot(Vector3.zero, adjustedRotation.eulerAngles);

            // Apply offset to hand attach transform
            attachTransform.localPosition = adjustedPosition;
            attachTransform.localRotation = adjustedRotation;
        }
    }
}