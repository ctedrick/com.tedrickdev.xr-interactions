using TedrickDev.InteractionsToolkit.Poser;
using UnityEngine;

namespace TedrickDev.InteractionsToolkit.Core
{
    [RequireComponent(typeof(PoserTool))]
    public class SingleGrabInteractable : GrabInteractable
    {
        [Header("Pose")]
        [SerializeField] private PoseData pose;
        
        [Header("Animation Settings")]
        [SerializeField] protected float AttachDuration = 0.2f;
  
        private PoserHand activeHand;

        public override void HandleSelectEnter(BaseInteractor interactor)
        {
            if (interactor.TryGetComponent(out PoserHand newHand)) {
                if (activeHand == null) {
                    activeHand = newHand;
                    SetPose(activeHand, pose, AttachDuration);
                }
            }
        }

        public override void HandleSelectExit(BaseInteractor interactor)
        {
            activeHand.SetPose(interactor.DefaultPose);
            DetachInteractable(transform);

            activeHand = null;
        }
    }
}