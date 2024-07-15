using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    public class PathTool : Editor
    {
        [MenuItem("Tool/Path/打开PersistentDataPath路径")]
        public static void OpenpersistentDataPath()
        {
            Application.OpenURL(Application.persistentDataPath);
        }
    }   
}
