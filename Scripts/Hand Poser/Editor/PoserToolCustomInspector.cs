using TedrickDev.Utilities;
using UnityEditor;
using UnityEngine;

namespace TedrickDev.XRPoser
{
    [CustomEditor(typeof(PoserTool))]
    public class PoserToolCustomInspector : Editor
    {
        private PoserTool poserTool;

        private bool setDistance;
        private float distance;
        private Vector2 minMaxValues;
        private float scrubValue;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            poserTool = (PoserTool) target;

            DrawPlaybackSection();
            DrawEditSection();
            DrawSaveSection();
        }

        private void DrawPlaybackSection()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Playback", EditorStyles.boldLabel);
            DrawUILine(Color.grey, 1, 5);

            if (poserTool.PoseData) {
                scrubValue = EditorGUILayout.Slider("Scrub", scrubValue, 0f, 1f);
                poserTool.ScrubPose(scrubValue);
            }
            
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Show Pose", EditorStyles.miniButton)) {
                ResetScrubValue(1);
                poserTool.ShowPose();
            }

            if (GUILayout.Button("Default Pose", EditorStyles.miniButton)) {
                ResetScrubValue(0);
                poserTool.DefaultPose();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawEditSection()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Edit", EditorStyles.boldLabel);
            DrawUILine(Color.grey, 1, 5);

            #region Top
            
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Toggle Left Hand", EditorStyles.miniButton)) {
                ResetScrubValue(1);
                poserTool.ToggleLeftHand();
            }

            if (GUILayout.Button("Toggle Right Hand", EditorStyles.miniButton)) {
                ResetScrubValue(1);
                poserTool.ToggleRightHand();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Select Left Hand", EditorStyles.miniButton)) {
                poserTool.SelectLeftHand();
            }

            if (GUILayout.Button("Select Right Hand", EditorStyles.miniButton)) {
                poserTool.SelectRightHand();
            }

            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Mirror >>>", EditorStyles.miniButton)) {
                ResetScrubValue(1);
                poserTool.MirrorLeftToRight();
            }

            if (GUILayout.Button("<<<< Mirror", EditorStyles.miniButton)) {
                ResetScrubValue(1);
                poserTool.MirrorRightToLeft();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Remove Hands", EditorStyles.miniButton)) {
                ResetScrubValue(1);
                poserTool.RemoveHands();
            }
            
            #endregion

            #region Set Distance Subsection

            EditorGUILayout.Space();

            setDistance = EditorGUILayout.Toggle("Set Distance", setDistance);

            if (setDistance) {
                minMaxValues = EditorGUILayout.Vector2Field("Min/Max", minMaxValues);

                distance = EditorGUILayout.Slider("Horizontal Distance", distance, minMaxValues.x, minMaxValues.y);
                poserTool.AdjustHandDistance(distance);
            }

            #endregion
        }

        private void DrawSaveSection()
        {
            EditorGUILayout.Space();

            DrawUILine(Color.grey, 1, 5);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            if (GUILayout.Button("Save New Pose", EditorStyles.miniButton)) {
                ResetScrubValue(1);
                
                var path = EditorUtility.SaveFilePanel("Save as asset", "Assets/", "Pose", "asset");
                if (path.Length != 0) { poserTool.SavePose(path.ConvertToProjectRelativePath()); }
            }
        }

        private void ResetScrubValue(float value)
        {
            if (!poserTool.PoseData) return;
            
            scrubValue = value;
            poserTool.ScrubPose(value);
        }

        private static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            var r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding * 0.5f;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }
}