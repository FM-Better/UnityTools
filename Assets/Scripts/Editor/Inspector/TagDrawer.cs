using UnityEngine;
using UnityEditor;

namespace EditorTool
{
    [CustomPropertyDrawer(typeof(TagAttribute))]
    public class TagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);

                // 核心：使用 EditorGUI.TagField 绘制系统自带的标签下拉框
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);

                EditorGUI.EndProperty();
            }
            else
            {
                // 如果属性不是 string 类型，显示警告
                EditorGUI.LabelField(position, label.text, "Use [Tag] with strings only.");
            }
        }
    }
}