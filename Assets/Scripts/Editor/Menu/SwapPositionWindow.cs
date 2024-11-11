using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    public class SwapPositionWindow : EditorWindow
    {
        [MenuItem("Tool/Swap Position Window")]
        public static void CreateWindow()
        {
            var windowRect = new Rect(0, 0, 500, 500);
            var window = GetWindowWithRect(typeof(SwapPositionWindow), windowRect, false, "Swap Position Window");
            window.Show();
        }

        private Transform _selectTrans1;
        private Transform _selectTrans2;
        
        private void OnGUI()
        {
            GUILayout.Space(10);

            DrawTittle();
            
            GUILayout.Space(20);

            DrawSelectObj();
            
            GUILayout.Space(10);
            
            DrawSelectTool();
            
            GUILayout.Space(20);
        }
        
        private void DrawTittle()
        {
            GUI.color = Color.yellow;
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Swap Position Window");
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.color = Color.white;
        }

        private void DrawSelectObj()
        {
            _selectTrans1 = EditorGUILayout.ObjectField(_selectTrans1, typeof(Transform), true) as Transform;
            GUILayout.Space(10);
            _selectTrans2 = EditorGUILayout.ObjectField(_selectTrans2, typeof(Transform), true) as Transform;
        }
        
        private void DrawSelectTool()
        {
            if (_selectTrans1 && _selectTrans2)
            {
                if (GUILayout.Button("交换位置"))
                {
                    (_selectTrans1.position, _selectTrans2.position) = (_selectTrans2.position, _selectTrans1.position);
                }    
            }
            else
            {
                GUI.color = Color.red;
                GUILayout.Button("未绑定两个需要交换的物体");
            }
        }
    }
}
