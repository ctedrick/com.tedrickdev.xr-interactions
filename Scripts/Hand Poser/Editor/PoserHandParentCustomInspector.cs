using UnityEditor;
using UnityEngine;

namespace TedrickDev.XRPoser
{
    [CustomEditor(typeof(PoserHandParent))]
    public class PoserHandParentCustomInspector : Editor
    {
        private PoserHandParent handParent;
        
        private bool isEditing;
        private string editButtonText;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            handParent = target as PoserHandParent;
            if (!handParent) return;

            GUI.color = isEditing ? Color.red : Color.green;
            editButtonText = isEditing ? "Finish Posing Hand" : "Start Posing Hand";

            if (GUILayout.Button(editButtonText, EditorStyles.miniButton)) {
                if (isEditing) {
                    var tool = handParent.transform.GetComponentInParent<PoserTool>();
                    if (tool) {
                        Selection.SetActiveObjectWithContext(tool, null);
                    }
                } 
                
                ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
                isEditing = !isEditing;
            }
        }
    }
}