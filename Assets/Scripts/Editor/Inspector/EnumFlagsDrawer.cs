using System;
using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //画出displayName
            position = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName));
            //EditorGUI.EnumFlagsField用来展示枚举类型的组合，就像LayerMask那样
            var e = EditorGUI.EnumFlagsField(position,
                (Enum)Enum.ToObject((attribute as EnumFlagsAttribute).EnumType, property.intValue));
            //把枚举值赋回给property
            property.intValue = Convert.ToInt32(e);
        }
    }
}