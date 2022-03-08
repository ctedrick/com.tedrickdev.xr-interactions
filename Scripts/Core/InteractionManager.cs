using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TedrickDev.InteractionsToolkit.Core
{
    public class InteractionManager : MonoBehaviour
    {
        public List<BaseInteractor> Interactors;
        public List<BaseInteractable> Interactables;
        
        private Dictionary<BaseInteractor, List<BaseInteractable>> hoveredTargets;
        private Dictionary<BaseInteractor, List<BaseInteractable>> selectedTargets;

        private void Awake()
        {
            Interactors = new List<BaseInteractor>();
            Interactables = new List<BaseInteractable>();
            
            hoveredTargets = new Dictionary<BaseInteractor, List<BaseInteractable>>();
            selectedTargets = new Dictionary<BaseInteractor, List<BaseInteractable>>();
        }

        public void RegisterInteractor(BaseInteractor interactor) => Interactors.Add(interactor);
        public void RegisterInteractable(BaseInteractable interactable) => Interactables.Add(interactable);

        public void HandleHoverEnter(BaseInteractor interactor, BaseInteractable interactable)
        {
            if (hoveredTargets.ContainsKey(interactor)) {
                if (hoveredTargets[interactor].Contains(interactable)) return;
            }
            
            // Interactor is actively hovering targets
            if (hoveredTargets.TryGetValue(interactor, out var interactables)) {
                
                interactables.Add(interactable);
                hoveredTargets[interactor] = interactables;
            }
            else {
                hoveredTargets.Add(interactor, new List<BaseInteractable> {interactable});
                print($"Added {interactable.name} to {interactor.name} hover list");
            }
            
            interactable.HandleHoverEnter(interactor);
        }
        
        public void HandleHoverExit(BaseInteractor interactor, BaseInteractable interactable)
        {
            if (hoveredTargets.TryGetValue(interactor, out var interactables)) {
                // Already hovering current object
                if (interactables.Remove(interactable)) {
                    print($"Removed {interactable.name} from {interactor.name} hover list");

                    if (interactables.Count == 0) {
                        hoveredTargets.Remove(interactor);
                    }
                    
                    interactable.HandleHoverExit(interactor);
                }
            }
        }

        public void TryToInteract(BaseInteractor interactor)
        {
            // Currently target(s) is selected...So release
            if (selectedTargets.TryGetValue(interactor, out var targets)) {
                if (targets.Count > 0) {
                    var interactable = targets.Last();
                    
                    interactor.NotifySelectExit(interactable);
                    interactable.HandleSelectExit(interactor);
                    
                    if (targets.Remove(targets.Last())) {
                        if (targets.Count == 0) selectedTargets.Remove(interactor);
                    }
                }
            }
            
            // No target selected && Hovering over new target
            else if (hoveredTargets.TryGetValue(interactor, out var hoveredItems)) {
                var interactable = GetLatestHoverTarget(interactor);

                if (!IsValidInteraction(interactor, interactable)) {
                    interactor.HandleInteractionPressedWithNoValidTargets();
                    return;
                }
                
                if (hoveredItems.Remove(interactable)) {
                    print($"{interactable} removed from hover list...");
                    if (hoveredItems.Count == 0) hoveredTargets.Remove(interactor);
                }
                
                selectedTargets.Add(interactor, new List<BaseInteractable>{interactable});
                interactor.NotifySelectEnter(interactable);
                interactable.HandleSelectEnter(interactor);
            }
            
            // No selection or hover targets
            else {
                interactor.HandleInteractionPressedWithNoValidTargets();
            }
        }

        public void ForceDeselect(BaseInteractor interactor)
        {
            if (selectedTargets.TryGetValue(interactor, out var interactables)) {
                foreach (var interactable in interactables) {
                    interactor.NotifySelectExit(interactable);
                }

                selectedTargets.Remove(interactor);
            }
        }

        // Maybe this is distanced based?
        private BaseInteractable GetLatestHoverTarget(BaseInteractor interactor)
        {
            if (hoveredTargets.ContainsKey(interactor)) {
                if (hoveredTargets.Count > 0) {
                    return hoveredTargets[interactor][0];
                }
            }

            return null;
        }
        
        private bool IsValidInteraction(BaseInteractor interactor, BaseInteractable interactable)
        {
            // interactor has selection and trying to grab something else...
            if (selectedTargets.TryGetValue(interactor, out var interactables)) {
                if (interactables.Contains(interactable)) return false;
            }

            // Return false since doesn't matter if interactable is selected by another interactor
            if (!interactable.AllowMultipleSelection) {
                // if interactable is already selected by other interactor
                foreach (var selectedInteractor in selectedTargets) {
                    // Dont look at the current interactor, seeing if interactable is selected by another interactor
                    if (selectedInteractor.Key == interactor) continue;
                
                    // Found another interactor that owns this interactable
                    if (selectedInteractor.Value.Contains(interactable)) return false;
                }
            }
            
            return true;
        }
    }
}