using UnityEditor;
using UnityEngine;
using Utility.Attribute;

namespace EditorTool
{
    [CustomPropertyDrawer(typeof(CustomHeaderAttribute))]
    public class CustomHeaderDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            var headerAttribute = (CustomHeaderAttribute)attribute;

            var style = new GUIStyle(EditorStyles.boldLabel)
            {
                normal =
                {
                    textColor = headerAttribute.textColor
                },
                fontSize = headerAttribute.fontSize,
                fontStyle = headerAttribute.fontStyle,
                alignment = headerAttribute.alignment
            };

            var headerRect = EditorGUI.IndentedRect(position);
            headerRect.height = EditorGUIUtility.singleLineHeight * 1.5f; // 设置为1.5倍行高
            headerRect.y += EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.LabelField(headerRect, headerAttribute.headerText, style);
        }

        public override float GetHeight() =>
            EditorGUIUtility.singleLineHeight * 1.5f + EditorGUIUtility.standardVerticalSpacing;
    }
}
