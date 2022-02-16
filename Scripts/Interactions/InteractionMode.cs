using System.Collections;
using TedrickDev.HandPoser.Poser;
using UnityEngine;

namespace TedrickDev.HandPoser.Interactions
{
    public abstract class InteractionMode : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] protected float PoseDuration = 0.2f;
        [SerializeField] protected float AttachDuration = 0.1f;
        
        public abstract void ApplyPose(InteractionZone grabber);

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
        
        protected void SetPose(InteractionZone grabber)
        {
            grabber.Hand.SetPose(grabber.Pose, PoseDuration);
            ApplyAttachmentTransform(grabber.ParentTransform, grabber.Hand.AttachTransform);
            transform.SetParent(grabber.Hand.AttachTransform);

            StopAllCoroutines();
            StartCoroutine(AttachHandRoutine(transform, Vector3.zero, Quaternion.identity));
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