using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    public class FolderFilesToEnumWindow : EditorWindow
    {
        [MenuItem("Tool/Folder Files To Enum Window %&C")]
        public static void CreateWindow()
        {
            var windowRect = new Rect(0, 0, 500, 500);
            var window = GetWindowWithRect(typeof(FolderFilesToEnumWindow), windowRect, false,
                "Folder Files To Enum Window");
            window.Show();
        }

        private string _folderPath;
        private string _generatedEnumContent; // 生成的枚举内容

        private void OnGUI()
        {
            GUILayout.Space(10);

            if (string.IsNullOrEmpty(_folderPath))
            {
                EditorGUILayout.LabelField("未选择文件夹，请选择",
                    new GUIStyle() { normal = new GUIStyleState() { textColor = Color.red } });
            }
            else
            {
                EditorGUILayout.LabelField("文件夹路径：", _folderPath);
            }

            if (GUILayout.Button("选择文件夹"))
            {
                _folderPath = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
            }

            if (GUILayout.Button("生成枚举类"))
            {
                var files = Directory.GetFiles(_folderPath, ".", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) ||
                                s.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)).ToArray(); // 获取文件夹下的所有文件(忽略后缀的大小写
                _generatedEnumContent = $"//生成于：{DateTime.Now.ToString()}\n";
                for (int i = 0; i < files.Length; i++)
                {
                    var file = files[i];
                    var fileName = file.Split('\\')[1];
                    var fileNameWithoutExtension = fileName.Substring(0, fileName.LastIndexOf('.'));
                    _generatedEnumContent += $"{fileNameWithoutExtension}";
                    if (i < files.Length - 1) // 不是最后一个文件
                    {
                        _generatedEnumContent += ",\n";
                    }
                }
            }

            if (!string.IsNullOrEmpty(_generatedEnumContent))
            {
                EditorGUILayout.TextArea(_generatedEnumContent);
            }
        }
    }
}
