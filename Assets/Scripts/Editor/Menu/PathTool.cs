using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    public class PathTool : Editor
    {
        [MenuItem("Tool/Open PersistentData Folder")]
        public static void OpenPersistentDataFolder() => EditorUtility.RevealInFinder(Application.persistentDataPath);
    }   
}
