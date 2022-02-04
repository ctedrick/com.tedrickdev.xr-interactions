﻿using UnityEditor;
using UnityEngine;
using TedrickDev.Utilities;

namespace TedrickDev.XRPoser
{
    public class PoserTool : MonoBehaviour
    {
        [Space]
        [SerializeField] private PoseData poseData;

        private PoserManager manager;

        private GameObject leftHandParent;
        private GameObject rightHandParent;

        private PoserHand leftHand;
        private PoserHand rightHand;

        private void OnValidate()
        {
            if (!manager) manager = FindObjectOfType<PoserManager>();
        }

        public void AdjustHandDistance(float distance)
        {
            if (leftHandParent == null || rightHandParent == null) return;

            var halfDistance = distance * 0.5f;
            
            var leftPosition = leftHandParent.transform.localPosition;
            leftPosition = new Vector3(halfDistance * -1, leftPosition.y, leftPosition.z);
            leftHandParent.transform.localPosition = leftPosition;

            var rightPosition = rightHandParent.transform.localPosition;
            rightPosition = new Vector3(halfDistance, rightPosition.y, rightPosition.z);
            rightHandParent.transform.localPosition = rightPosition;
        }

        public void ApplyDefaultPose(PoserHand poserHand)
        {
            switch (poserHand.Type) {
                case Handedness.Left:
                    poserHand.SetPose(manager.DefaultPose.LeftJoints);
                    break;
                case Handedness.Right:
                    poserHand.SetPose(manager.DefaultPose.RightJoints);
                    break;
            }
        }
        
        public void ApplyPose(PoserHand poserHand, Transform attachTransform)
        {
            switch (poserHand.Type) {
                case Handedness.Left:
                    poserHand.SetPose(poseData.LeftJoints);
                    ApplyAttachmentTransform(poseData.LeftParentTransform, attachTransform);
                    break;
                case Handedness.Right:
                    poserHand.SetPose(poseData.RightJoints);
                    ApplyAttachmentTransform(poseData.RightParentTransform, attachTransform);
                    break;
            }
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
        
        #region Editor Tools
        
        public void ShowPose() => SetEditorPose(poseData);

        public void DefaultPose() => SetEditorPose(manager.DefaultPose);

        public void SavePose(string filePath)
        {
            var so = ScriptableObject.CreateInstance<PoseData>();
            if (leftHand) so.SaveLeftHandData(leftHand.CreatePose(), leftHandParent.transform);
            if (rightHand) so.SaveRightHandData(rightHand.CreatePose(), rightHandParent.transform);

            AssetDatabase.CreateAsset(so, filePath);
        }

        public void ToggleLeftHand()
        {
            if (CheckParentGameObjectExists(ref leftHandParent, nameof(leftHandParent), ref leftHand, manager.LeftPrefab)) {
                leftHandParent.SetActive(!leftHandParent.activeSelf);
            }
        }

        public void ToggleRightHand()
        {
            if (CheckParentGameObjectExists(ref rightHandParent, nameof(rightHandParent), ref rightHand, manager.RightPrefab)) {
                rightHandParent.SetActive(!rightHandParent.activeSelf);
            }
        }

        public void MirrorLeftToRight()
        {
            MirrorHand(leftHandParent.transform, rightHandParent.transform);
            MirrorJoints(leftHand, rightHand);
        }
        
        public void MirrorRightToLeft()
        {
            MirrorHand(rightHandParent.transform, leftHandParent.transform);
            MirrorJoints(rightHand, leftHand);
        }
        
        public void RemoveHands()
        {
            DestroyImmediate(leftHandParent);
            DestroyImmediate(rightHandParent);
            leftHandParent = null;
            rightHandParent = null;
            leftHand = null;
            rightHand = null;
        }
        
        private bool CheckParentGameObjectExists(ref GameObject parentGO, string goName, ref PoserHand poserHand, PoserHand prefab)
        {
            if (!parentGO) {
                parentGO = new GameObject(goName);
                parentGO.transform.localPosition = Vector3.zero;
                parentGO.transform.localRotation = Quaternion.identity;
                parentGO.transform.SetParent(transform);

                poserHand = Instantiate(prefab, parentGO.transform);
                var poserHandTransform = poserHand.transform;
                poserHandTransform.localPosition = Vector3.zero;
                poserHandTransform.localRotation = Quaternion.identity;
                poserHand.gameObject.SetActive(true);

                if (poserHand == leftHand)
                    leftHandParent.transform.position = transform.position + Vector3.left * 0.1f;
                else
                    rightHandParent.transform.position = transform.position + Vector3.right * 0.1f;

                return false;
            }

            return true;
        }
        
        private void SetEditorPose(PoseData data)
        {
            if (leftHand && data.LeftJoints.Length != 0) {
                leftHand.SetPose(data.LeftJoints);
                leftHandParent.transform.localPosition = data.LeftParentTransform.LocalPosition;
                leftHandParent.transform.localRotation = data.LeftParentTransform.LocalRotation;
            }

            if (rightHand && data.RightJoints.Length != 0) {
                rightHand.SetPose(data.RightJoints);
                rightHandParent.transform.localPosition = data.RightParentTransform.LocalPosition;
                rightHandParent.transform.localRotation = data.RightParentTransform.LocalRotation;
            }
        }
        
        private static void MirrorJoints(PoserHand source, PoserHand copy)
        {
            for (var i = 0; i < source.Joints.Count; i++) {
                var rotation = source.Joints[i].localRotation;
                copy.Joints[i].localRotation = rotation;
            }
        }

        private static void MirrorHand(Transform source, Transform copy)
        {
            var localPosition = source.localPosition;
            var newPosition = new Vector3(copy.localPosition.x, localPosition.y, localPosition.z);
            copy.localPosition = newPosition;

            var localRotation = source.localRotation;
            
            // https://forum.unity.com/threads/how-to-mirror-a-euler-angle-or-rotation.90650/
            copy.localRotation = new Quaternion(localRotation.x * -1.0f, localRotation.y, localRotation.z, localRotation.w * -1.0f);
        }
        
        #endregion
    }
}