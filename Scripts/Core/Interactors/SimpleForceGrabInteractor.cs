using TedrickDev.InteractionsToolkit.Utility;
using UnityEngine;

namespace TedrickDev.InteractionsToolkit.Core
{
    public class SimpleForceGrabInteractor : BaseInteractor
    {
        [Header("Raycast Settings")]
        [SerializeField] private Transform source;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float range = 5f;
        
        private RaycastUtility<BaseInteractable> raycast;

        protected override void Awake()
        {
            base.Awake();

            if (!source) {
                Debug.LogError($"{nameof(source)} is null. Check {gameObject.name}");
                return;
            }

            raycast = new RaycastUtility<BaseInteractable>(source, layerMask, range);
        }

        private void OnEnable()
        {
            raycast.OnHoverEnter += RaycastOnOnHoverEnter;
            raycast.OnHoverExit += RaycastOnOnHoverExit;
        }
        
        private void OnDisable()
        {
            raycast.OnHoverEnter -= RaycastOnOnHoverEnter;
            raycast.OnHoverExit -= RaycastOnOnHoverExit;
        }

        private void RaycastOnOnHoverExit(BaseInteractable interactable) => HandleHoverExit(interactable);

        private void RaycastOnOnHoverEnter(BaseInteractable interactable) => HandleHoverEnter(interactable);
        
        private void FixedUpdate() => raycast.FixedUpdate();

        private void OnDrawGizmos()
        {
            if (!source) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(source.position, source.forward * range);
        }
    }
}