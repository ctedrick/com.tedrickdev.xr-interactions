﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TedrickDev.XRPoser.Interactions
{
    public class GrabController : MonoBehaviour
    {
        private enum SelectionType { OneHand, TwoHand }

        [SerializeField] private SelectionType grabType;

        [Space]
        [SerializeField] private bool autoPopulateGrabZones;
        [SerializeField] private List<GrabZone> grabZones;
        
        private GrabMode grabMode;

        private void Awake()
        {
            if (autoPopulateGrabZones) GetComponentsInChildren(grabZones);
            
            if (grabZones.Count == 0) {
                Debug.LogError($"{nameof(grabZones)} is empty. {name}");
                enabled = false;
                return;
            }
            
            foreach (var zone in grabZones) zone.OnInteract += OnInteract;
            
            grabMode = GetComponent<GrabMode>();
        }
        
        // https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/
        #if UNITY_EDITOR
        
        private void OnValidate() => EditorApplication.delayCall += _OnValidate;
        
        private void _OnValidate()
        {
            if (Application.isPlaying) return;
            
            switch (grabType) {
                case SelectionType.OneHand:
                    AddSelectionMode<OneHandGrab>();
                    break;
                case SelectionType.TwoHand:
                    AddSelectionMode<TwoHandGrab>();
                    break;
            }

            grabMode = GetComponent<GrabMode>();
        }
        
        #endif

        private void AddSelectionMode<T>() where T : Component
        {
            if (TryGetComponent(out T _)) return;
            if (TryGetComponent(out GrabMode sm)) DestroyImmediate(sm);
            gameObject.AddComponent<T>();
        }

        private void ApplyPose(GrabZone grabData) => grabMode.ApplyPose(grabData);

        private void OnInteract(GrabZone grabData) => ApplyPose(grabData);
    }
}