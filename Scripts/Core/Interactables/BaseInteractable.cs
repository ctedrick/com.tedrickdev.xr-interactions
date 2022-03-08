using UnityEngine;

namespace TedrickDev.InteractionsToolkit.Core
{
    public abstract class BaseInteractable : MonoBehaviour
    {
        public bool AllowMultipleSelection{ get; protected set; }
        
        protected virtual void Start()
        {
            var interactionManager = FindObjectOfType<InteractionManager>();
            if (interactionManager) interactionManager.RegisterInteractable(this);
        }

        public void HandleHoverEnter(BaseInteractor interactor)
        {
            print($"{interactor.name} is hovering on {name}");
        }

        public void HandleHoverExit(BaseInteractor interactor)
        {
            print($"{interactor.name} is no longer hovering on {name}");
        }

        public virtual void HandleSelectEnter(BaseInteractor interactor)
        {
            print($"{interactor.name} select entered {name}");
        }

        public virtual void HandleSelectExit(BaseInteractor interactor)
        {
            print($"{interactor.name} select exited {name}");
        }
    }
}