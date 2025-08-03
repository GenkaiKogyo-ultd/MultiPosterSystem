using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Wacky612.MultiPosterSystem
{
    [CustomEditor(typeof(MultiPosterSystem))]
    public class MultiPosterSystemEditor : Editor
    {
        private StaticPostersEditorContent _staticPosterEditorContent;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_posterType"),
                                          new GUIContent("動作モード"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_durationSeconds"),
                                          new GUIContent("ポスターの持続時間（秒）"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_transitionSeconds"),
                                          new GUIContent("ポスターの切り替え時間（秒）"));
            
            EditorGUILayout.Space();
            GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
            EditorGUILayout.Space();

            switch ((PosterType) serializedObject.FindProperty("_posterType").enumValueIndex)
            {
                case PosterType.Static:
                    _staticPosterEditorContent.Draw();
                    break;
                case PosterType.Dynamic:
                    DrawDynamicPosterEditorContent();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnEnable()
        {
            var staticPosters = ((Component) target).GetComponentInChildren<StaticPosters>();
            
            _staticPosterEditorContent = new StaticPostersEditorContent(staticPosters);
        }

        private void DrawDynamicPosterEditorContent()
        {
            var dynamicPosters = ((Component) target).GetComponentInChildren<DynamicPosters>();
            var so = new SerializedObject(dynamicPosters);

            EditorGUILayout.PropertyField(so.FindProperty("_posterVideoUrl"),
                                          new GUIContent("ポスターの動画データのURL"));
            EditorGUILayout.PropertyField(so.FindProperty("_groupIdsJsonUrl"),
                                          new GUIContent("グループIDのJSONデータのURL"));
            EditorGUILayout.PropertyField(so.FindProperty("_posterLoadDelaySeconds"),
                                          new GUIContent("ポスターの動画データのロード遅延（秒）"));
            EditorGUILayout.PropertyField(so.FindProperty("_groupIdsLoadDelaySeconds"),
                                          new GUIContent("グループIDのJSONデータのロード遅延（秒）"));

            so.ApplyModifiedProperties();
            so.Update();
        }
    }
}
