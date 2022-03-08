using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TedrickDev.InteractionsToolkit.Poser
{
    public class PoserHand : MonoBehaviour
    {
        [Header("Hand Properties")]
        public Handedness Type;
        public Transform AttachTransform;
        [SerializeField] private List<Transform> joints;

        public List<Transform> Joints => joints;

        private void Awake()
        {
            if (joints == null || joints.Count == 0) {
                Debug.LogError($"{nameof(joints)} is null or empty. Check {name}");
                enabled = false;
            }
        }

        public PoseTransform[] CreatePose()
        {
            var jointData = new PoseTransform[joints.Count];
            for (var i = 0; i < joints.Count; i++) {
                var poseData = new PoseTransform {
                    LocalRotation = joints[i].localRotation
                };

                jointData[i] = poseData;
            }

            return jointData;
        }

        public void SetScrubPose(PoseTransform[] poseFrom, PoseTransform[] poseTo, float value)
        {
            if (poseFrom == null || poseFrom.Length == 0) return;
            if (poseTo == null || poseTo.Length == 0) return;

            value = Mathf.Clamp01(value);

            for (var i = 0; i < joints.Count; i++) {
                joints[i].localRotation = GetScrubRotation(poseFrom[i].LocalRotation, poseTo[i].LocalRotation, value);
            }
        }

        public void SetPose(PoseData poseData, float duration = 0.2f)
        {
            SetPose(Type == Handedness.Left ? poseData.LeftJoints : poseData.RightJoints, duration);
        }
        
        public void SetPose(PoseTransform[] pose, float duration = 0.2f)
        {
            if (pose == null || pose.Length == 0) return;
            
            StopAllCoroutines();
            
            for (var i = 0; i < joints.Count; i++) {
                if (Application.isPlaying)
                    StartCoroutine(RotationRoutine(joints[i].transform, pose[i].LocalRotation, duration));
                else
                    joints[i].localRotation = pose[i].LocalRotation;
            }
        }

        private static Quaternion GetScrubRotation(Quaternion from, Quaternion end, float value) => Quaternion.Lerp(from, end, value);

        private static IEnumerator RotationRoutine(Transform target, Quaternion end, float duration)
        {
            var time = 0f;
            var start = target.localRotation;
            while (time < duration) {
                target.localRotation = Quaternion.Lerp(start, end, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            target.localRotation = end;
        }
    }
}