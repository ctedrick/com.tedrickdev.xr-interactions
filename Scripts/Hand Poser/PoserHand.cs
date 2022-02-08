using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TedrickDev.XRPoser
{
    public class PoserHand : MonoBehaviour
    {
        [SerializeField] private List<Transform> joints;

        public List<Transform> Joints => joints;
        
        public Handedness Type;

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
        
        public void SetPose(PoseTransform[] pose)
        {
            if (pose == null || pose.Length == 0) return;
            
            StopAllCoroutines();
            
            for (var i = 0; i < joints.Count; i++) {
                if (Application.isPlaying)
                    StartCoroutine(RotationRoutine(joints[i].transform, pose[i].LocalRotation));
                else
                    joints[i].localRotation = pose[i].LocalRotation;
            }
        }

        private static Quaternion GetScrubRotation(Quaternion @from, Quaternion end, float value) => Quaternion.Lerp(@from, end, value);

        private static IEnumerator RotationRoutine(Transform target, Quaternion end)
        {
            var time = 0f;
            var start = target.localRotation;
            while (time < 0.2f) {
                target.localRotation = Quaternion.Lerp(start, end, time / 0.2f);
                time += Time.deltaTime;
                yield return null;
            }

            target.localRotation = end;
        }
    }
}