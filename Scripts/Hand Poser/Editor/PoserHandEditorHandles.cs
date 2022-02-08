using UnityEditor;
using UnityEngine;

namespace TedrickDev.XRPoser
{
    [CustomEditor(typeof(PoserHand))]
    public class PoserHandEditorHandles : Editor
    {
        private PoserHand poserHand;

        private const float Radius = 0.005f;
        private const float ClickRadius = 0.005f;

        private bool isEditing;
        private string editButtonText;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            GUI.color = isEditing ? Color.red : Color.green;
            editButtonText = isEditing ? "Finish Posing Joints" : "Start Posing Joints";
            
            if (GUILayout.Button(editButtonText, EditorStyles.miniButton)) {
                if (isEditing) {
                    var tool = poserHand.transform.GetComponentInParent<PoserTool>();
                    if (tool) {
                        Selection.SetActiveObjectWithContext(tool, null);
                    }
                } 
                
                ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
                isEditing = !isEditing;
            }
        }

        private void OnSceneGUI()
        {
            if (!isEditing) return;
            
            poserHand = target as PoserHand;
            
            if (!poserHand) return;
            var lookRotation = Quaternion.LookRotation(Camera.current.transform.forward);
            if (poserHand && poserHand.Joints != null && poserHand.Joints.Count != 0) {
                foreach (var joint in poserHand.Joints) {
                    Handles.color = new Color(255,255, 255f, 0.05f);

                    Handles.DrawSolidDisc(joint.position, Camera.current.transform.forward, Radius);
                    
                    if (Handles.Button(joint.position, lookRotation, Radius, ClickRadius, Handles.CircleHandleCap)) {
                        Selection.SetActiveObjectWithContext(joint.transform, null);
                    }
                }
            }
        }
    }
}