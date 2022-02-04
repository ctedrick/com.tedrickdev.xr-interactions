using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace TedrickDev.XRPoser.Wrapper
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class XRToolkitPoserWrapper : MonoBehaviour
    {
        private PoserTool tool;
        private XRBaseInteractable interactable;

        protected void Awake()
        {
            tool = GetComponent<PoserTool>();
            interactable = GetComponent<XRBaseInteractable>();
            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.selectExited.AddListener(OnSelectExited);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            var hand = args.interactorObject.transform.GetComponentInChildren<PoserHand>();
            var interactor = args.interactorObject.transform.GetComponent<XRBaseInteractor>();
            tool.ApplyPose(hand, interactor.attachTransform);
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            var hand = args.interactorObject.transform.GetComponentInChildren<PoserHand>();
            tool.ApplyDefaultPose(hand);
        }
    }
}