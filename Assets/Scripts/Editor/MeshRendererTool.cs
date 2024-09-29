using EditorToolKit;
using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    [CustomEditor(typeof(MeshRenderer))]
    public class MeshRendererTool : Editor
    {
        private MeshRenderer _meshRenderer;

        private string[] _sortingLayerNameArray;
        private int _sortingLayerID;
        private int _sortingOrder;

        private void OnEnable()
        {
            serializedObject.Update();

            _sortingLayerNameArray = new string[SortingLayer.layers.Length];

            var layers = SortingLayer.layers;

            for (int i = 0; i < layers.Length; i++)
            {
                _sortingLayerNameArray[i] = layers[i].name;
            }

            _meshRenderer = (MeshRenderer)target;

            _sortingLayerID = _meshRenderer.sortingLayerID;
            _sortingOrder = _meshRenderer.sortingOrder;

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            EditorHelper.DrawTitle("Additional Settings", Color.cyan, titleFontSize: 15,
                textAnchor: TextAnchor.MiddleLeft);

            DrawSortingAbout();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSortingAbout()
        {
            EditorGUI.BeginChangeCheck();
            _sortingLayerID = EditorGUILayout.Popup("Sorting Layer", _sortingLayerID, _sortingLayerNameArray);
            if (EditorGUI.EndChangeCheck())
            {
                _meshRenderer.sortingLayerID = _sortingLayerID;
            }

            EditorGUI.BeginChangeCheck();
            _sortingOrder = EditorGUILayout.IntField("Order In Layer", _sortingOrder);
            if (EditorGUI.EndChangeCheck())
            {
                _meshRenderer.sortingOrder = _sortingOrder;
            }
        }
    }
}
