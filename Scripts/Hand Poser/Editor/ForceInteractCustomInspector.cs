using TedrickDev.Utilities;
using UnityEditor;
using UnityEngine;

namespace TedrickDev.XRPoser
{
    [CustomEditor(typeof(ForceInteract))]
    public class ForceInteractCustomInspector : Editor
    {
        private ForceInteract forceInteract;
        
        public override void OnInspectorGUI()
        {
            forceInteract = (ForceInteract) target;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(nameof(forceInteract.Grab), EditorStyles.miniButton)) {
                forceInteract.Grab();
            }
            
            if (GUILayout.Button(nameof(forceInteract.Release), EditorStyles.miniButton)) {
                forceInteract.Release();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}