using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TedrickDev.HandPoser.Interactions
{
    public class InteractionController : MonoBehaviour
    {
        private enum SelectionType { OneHand, SimpleTwoHand }

        [SerializeField] private SelectionType grabType;

        [Space]
        [SerializeField] private bool autoPopulateGrabZones = true;
        [SerializeField] private List<InteractionZone> grabZones;
        
        private InteractionMode interactionMode;

        private void Awake()
        {
            if (autoPopulateGrabZones) GetComponentsInChildren(grabZones);
            
            if (grabZones.Count == 0) {
                Debug.LogError($"{nameof(grabZones)} is empty. {name}");
                enabled = false;
                return;
            }
            
            foreach (var zone in grabZones) zone.OnInteract += OnInteract;
            
            interactionMode = GetComponent<InteractionMode>();
        }
        
        // https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/
        #if UNITY_EDITOR
        
        private void OnValidate() => EditorApplication.delayCall += _OnValidate;
        
        private void _OnValidate()
        {
            if (Application.isPlaying) return;
            
            switch (grabType) {
                case SelectionType.OneHand:
                    AddSelectionMode<OneHandInteraction>();
                    break;
                case SelectionType.SimpleTwoHand:
                    AddSelectionMode<SimpleTwoHandInteraction>();
                    break;
            }

            interactionMode = GetComponent<InteractionMode>();
        }
        
        #endif

        private void AddSelectionMode<T>() where T : Component
        {
            if (TryGetComponent(out T _)) return;
            if (TryGetComponent(out InteractionMode sm)) DestroyImmediate(sm);
            gameObject.AddComponent<T>();
        }

        private void ApplyPose(InteractionZone interactionData) => interactionMode.ApplyPose(interactionData);

        private void OnInteract(InteractionZone interactionData) => ApplyPose(interactionData);
    }
}