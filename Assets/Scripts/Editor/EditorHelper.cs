using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    public static class EditorHelper
    {
        public static void DrawTitle(string titleName, Color titleColor, int titleFontSize = 20, float titleHeight = 20f, float titleSpace = 10f, TextAnchor textAnchor = TextAnchor.MiddleCenter)
        {
            EditorGUILayout.Space(titleSpace);

            var _titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = textAnchor,
                normal =
                {
                    textColor = titleColor
                },
                fontSize = titleFontSize
            };
            
            EditorGUILayout.LabelField(titleName, _titleStyle, GUILayout.Height(titleHeight));
        }
    }   
}
