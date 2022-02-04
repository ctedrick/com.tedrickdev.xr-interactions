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
                    LocalPosition = joints[i].localPosition,
                    LocalRotation = joints[i].localRotation
                };

                jointData[i] = poseData;
            }

            return jointData;
        }

        public void SetPose(PoseTransform[] pose)
        {
            if (pose == null || pose.Length == 0) return;
            
            StopAllCoroutines();
            
            for (var i = 0; i < joints.Count; i++) {
                if (Application.isPlaying) {
                    StartCoroutine(PositionRoutine(joints[i].transform, pose[i].LocalPosition));
                    StartCoroutine(RotationRoutine(joints[i].transform, pose[i].LocalRotation));
                }
                else {
                    joints[i].localPosition = pose[i].LocalPosition;
                    joints[i].localRotation = pose[i].LocalRotation;
                }
            }
        }

        private static IEnumerator PositionRoutine(Transform target, Vector3 end)
        {
            var time = 0f;
            var start = target.localPosition;
            while (time < 0.2f) {
                target.localPosition = Vector3.Lerp(start, end, time / 0.2f);
                time += Time.deltaTime;
                yield return null;
            }

            target.localPosition = end;
        }
        
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

        private void OnDrawGizmos()
        {
            if (joints == null || joints.Count == 0) return;

            foreach (var joint in joints) {
                Gizmos.matrix = joint.localToWorldMatrix;
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(Vector3.zero, 0.01f);
            }
        }
    }
}