using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace TedrickDev.Utilities
{
    public class ForceInteract : MonoBehaviour
    {
        private XRBaseInteractor interactor;

        private readonly List<IXRInteractable> targetList = new List<IXRInteractable>();

        private void Awake() => interactor = GetComponent<XRBaseInteractor>();

        public void Grab()
        {
            if (interactor.hasSelection) return;
            
            interactor.GetValidTargets(targetList);
            if (targetList == null || targetList.Count == 0) return;
            
            var interactable = (IXRSelectInteractable) targetList[0];
            interactor.interactionManager.SelectEnter(interactor, interactable);
        }

        public void Release()
        {
            if (!interactor.hasSelection) return;

            interactor.GetValidTargets(targetList);
            if (targetList == null || targetList.Count == 0) return;

            var interactable = (IXRSelectInteractable) targetList[0];
            interactor.interactionManager.SelectCancel(interactor, interactable);
        }
    }
}