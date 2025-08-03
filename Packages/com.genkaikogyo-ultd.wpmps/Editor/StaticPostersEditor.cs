using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Wacky612.MultiPosterSystem
{
    [CustomEditor(typeof(StaticPosters))]
    public class StaticPostersEditor : Editor
    {
        private StaticPostersEditorContent _staticPosterEditorContent;

        public override void OnInspectorGUI()
        {
            _staticPosterEditorContent.Draw();
        }
        
        private void OnEnable()
        {
            _staticPosterEditorContent = new StaticPostersEditorContent((StaticPosters) target);
        }
    }

    public class StaticPostersEditorContent
    {
        private StaticPosters   _staticPosters;
        private ReorderableList _posterReorderableList;
        private List<Poster>    _posterList;
        
        public StaticPostersEditorContent(StaticPosters staticPosters)
        {
            _staticPosters = staticPosters;
            GeneratePosterList();
            GeneratePosterReorderableList();
        }

        public void Draw()
        {
            _posterReorderableList.DoLayoutList();
        }

        private void GeneratePosterList()
        {
            _posterList = new List<Poster>();

            for (int i = 0; i < _staticPosters.transform.childCount; i++)
            {
                var poster = _staticPosters.transform.GetChild(i).GetComponent<Poster>();

                if (poster != null) _posterList.Add(poster);
            }
        }

        private void GeneratePosterReorderableList()
        {
            _posterReorderableList = new ReorderableList(_posterList, typeof(Poster),
                                                        true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "ポスター一覧");
                },
                onAddCallback = (list) =>
                {
                    var poster = new GameObject("Poster", typeof(Poster)).GetComponent<Poster>();
                    poster.transform.SetParent(_staticPosters.transform);
                    poster.transform.localPosition = Vector3.zero;
                    poster.transform.localRotation = Quaternion.identity;
                    
                    _posterList.Add(poster);
                },
                onRemoveCallback = (list) =>
                {
                    var poster = _posterList[list.index];
                    _posterList.RemoveAt(list.index);
                    GameObject.DestroyImmediate(poster.gameObject);
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedObject so = new SerializedObject(_posterList[index]);
                    SerializedProperty texture = so.FindProperty("_texture");
                    SerializedProperty groupId = so.FindProperty("_groupId");

                    Rect textureRect = rect;
                    Rect groupIdRect = rect;

                    textureRect.height  = EditorGUIUtility.singleLineHeight;
                    groupIdRect.height  = EditorGUIUtility.singleLineHeight;
                    groupIdRect.y      += (EditorGUIUtility.singleLineHeight +
                                           EditorGUIUtility.standardVerticalSpacing);

                    EditorGUI.PropertyField(textureRect, texture, new GUIContent("ポスターのテクスチャ"));
                    EditorGUI.PropertyField(groupIdRect, groupId, new GUIContent("グループID"));

                    so.ApplyModifiedProperties();
                    so.Update();
                },
                onReorderCallback = (ReorderableList list) =>
                {
                    for (int i = 0; i < list.list.Count; i++)
                    {
                        ((Poster) list.list[i]).transform.SetSiblingIndex(i);
                    }
                },
                elementHeight = (EditorGUIUtility.singleLineHeight +
                                 EditorGUIUtility.standardVerticalSpacing) * 2
            };
        }
    }
}
