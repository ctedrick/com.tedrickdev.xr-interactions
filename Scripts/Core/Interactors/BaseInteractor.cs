using TedrickDev.InteractionsToolkit.Poser;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TedrickDev.InteractionsToolkit.Core
{
    public abstract class BaseInteractor : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference primary;
        
        [Header("Manager")]
        public InteractionManager InteractionManager;

        [Header("Properties")]
        [SerializeField] private PoseData defaultPose;
        [SerializeField] private PoserHand poserHand;

        public PoseData DefaultPose => defaultPose;
        
        private bool isButtonPressed;
        private bool isButtonDown;

        protected virtual void Awake()
        {
            if (primary) {
                primary.action.Enable();
                primary.action.performed += context => {
                    isButtonPressed = true;
                    TryToInteract();
                };
                primary.action.canceled += context => {
                    isButtonPressed = false;
                    TryToInteract();
                };
            }
        }

        private void Start()
        {
            if (!InteractionManager) {
                Debug.LogError($"{nameof(InteractionManager)} is null. Add reference on {gameObject.name}");
                return;
            }
            
            InteractionManager.RegisterInteractor(this);    
            
            if (defaultPose && poserHand) poserHand.SetPose(defaultPose);
        }

        protected void HandleHoverEnter(BaseInteractable interactable)
        {
            InteractionManager.HandleHoverEnter(this, interactable);
        }

        protected void HandleHoverExit(BaseInteractable interactable)
        {
            InteractionManager.HandleHoverExit(this, interactable);
        }

        private void TryToInteract()
        {
            if (isButtonPressed && !isButtonDown) {
                isButtonDown = true;
                InteractionManager.TryToInteract(this);
            }
            else if (!isButtonPressed && isButtonDown) isButtonDown = false;
        }

        /// <summary>
        /// Called when interact button is pressed but no valid targets exist
        /// </summary>
        public virtual void HandleInteractionPressedWithNoValidTargets()
        {
            // trigger pose, event, etc...
            print($"{name} notified of idle selection");
        }

        public virtual void NotifySelectEnter(BaseInteractable interactable)
        {
            print($"{name} notified of selection enter");
        }
        
        public virtual void NotifySelectExit(BaseInteractable interactable)
        {
            print($"{name} notified of selection exit");
        }
    }
}