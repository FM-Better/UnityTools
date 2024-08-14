using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utility;

namespace EditorTool
{
    [CustomEditor(typeof(SetRenderOrder))]
    public class SetRenderOrderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        
            if (GUILayout.Button("ApplyRenderOrder"))
            {
                var obj = (SetRenderOrder)target;
                obj.ApplyRenderOrder();
            }
        }
    }

    [CustomPropertyDrawer(typeof(SortingLayerMask))]
    public class SortingLayerMaskPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var layerID = property.FindPropertyRelative("SortingLayerID");
            var layerName = property.FindPropertyRelative("SortingLayerName");
        
            var layerNames = new List<string>();
            foreach (var layer in SortingLayer.layers)
            {
                layerNames.Add(layer.name);
            }
        
            var selectedLayerIndex = Mathf.Max(0, System.Array.IndexOf(layerNames.ToArray(), layerName.stringValue));

            selectedLayerIndex = EditorGUILayout.Popup(label.text, selectedLayerIndex, layerNames.ToArray());
            layerID.intValue = SortingLayer.NameToID(layerNames[selectedLayerIndex]);
            layerName.stringValue = layerNames[selectedLayerIndex];

            EditorGUI.EndProperty();
        }
    }
}
