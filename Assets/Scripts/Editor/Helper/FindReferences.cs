using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Utility;

namespace EditorTool
{
    public class FindReferences : MonoBehaviour
    {
        [MenuItem("Assets/Find References", false, 10)]
        private static void Find()
        {
            EditorSettings.serializationMode = SerializationMode.ForceText;
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (Directory.Exists(path) || string.IsNullOrEmpty(path))
                return;

            var guid = AssetDatabase.AssetPathToGUID(path);
            var withoutExtensions = new List<string>()
                { ".prefab", ".unity", ".mat", ".asset", ".controller", ".anim" };
            var files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            var nowIndex = 0;
            var matchCount = 0;
            EditorApplication.update = delegate()
            {
                var file = files[nowIndex];

                var isCancel =
                    EditorUtility.DisplayCancelableProgressBar("匹配资源中", file,
                        nowIndex / (float)files.Length);

                if (Regex.IsMatch(File.ReadAllText(file), guid))
                {
                    matchCount++;
                    Debug.Log(file, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));
                }

                nowIndex++;
                if (isCancel || nowIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    nowIndex = 0;
                    DebugUtil.Log($"匹配结束, 共找到 {matchCount} 个引用", Color.cyan);
                }
            };
        }

        private static string GetRelativeAssetsPath(string path) =>
            $"Assets{Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/')}";
    }
}
