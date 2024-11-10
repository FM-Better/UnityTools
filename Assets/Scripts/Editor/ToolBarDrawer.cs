using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace EditorTool
{
    [InitializeOnLoad]
    public class ToolBarDrawer
    {
        private static bool _isTestMode;
        private static GUIStyle _labelStyle;
        private const string TestModeKey = "IsTestMode";
        
        static ToolBarDrawer()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolBarGUI);
            _isTestMode = EditorPrefs.GetBool(TestModeKey, false);
            _labelStyle = new GUIStyle()
            {
                normal =
                {
                    textColor = Color.cyan
                }
            };
        }

        private static void OnToolBarGUI()
        {
            GUILayout.FlexibleSpace();

            GUILayout.Label("测试模式:", _labelStyle);
            EditorGUI.BeginChangeCheck();
            _isTestMode = GUILayout.Toggle(_isTestMode, "");
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(TestModeKey, _isTestMode);
            }
        }
    }
}
