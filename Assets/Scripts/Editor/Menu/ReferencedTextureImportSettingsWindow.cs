using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    public sealed class ReferencedTextureImportSettingsWindow : EditorWindow
    {
        private const string WindowTitle = "引用图片导入设置扫描";
        private const float PathColumnMinWidth = 360f;

        // 这些资源类型可能直接或间接引用图片，用它们作为依赖扫描入口。
        private static readonly HashSet<string> ReferenceSourceExtensions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".anim",
                ".asset",
                ".controller",
                ".cubemap",
                ".flare",
                ".guiskin",
                ".mat",
                ".mask",
                ".overrideController",
                ".playable",
                ".prefab",
                ".renderTexture",
                ".spriteatlas",
                ".spriteatlasv2",
                ".terrainlayer",
                ".unity",
                ".uss",
                ".uxml"
            };

        // 图片本身不作为扫描入口，避免把自依赖误判为“被引用”。
        private static readonly HashSet<string> TextureExtensions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".bmp",
                ".exr",
                ".gif",
                ".hdr",
                ".iff",
                ".jpeg",
                ".jpg",
                ".pict",
                ".png",
                ".psb",
                ".psd",
                ".tga",
                ".tif",
                ".tiff"
            };

        private readonly List<TextureIssue> issues = new List<TextureIssue>();
        private Vector2 scrollPosition;
        private string searchText = string.Empty;
        private bool locateReadWrite = true;
        private bool locateMipmap = true;
        private bool includeEditorAssets = true;
        private string lastScanInfo = "尚未扫描";

        [MenuItem("Tool/扫描引用图片导入设置")]
        public static void Open()
        {
            ReferencedTextureImportSettingsWindow window = GetWindow<ReferencedTextureImportSettingsWindow>();
            window.titleContent = new GUIContent(WindowTitle);
            window.minSize = new Vector2(900f, 480f);
            window.Show();
        }

        private void OnGUI()
        {
            DrawToolbar();
            EditorGUILayout.Space(6f);
            DrawSummary();
            EditorGUILayout.Space(6f);
            DrawIssueList();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.HelpBox("扫描当前项目中被其它资源引用到的图片，并定位 Read/Write 或 Mipmap 开启的导入设置。", MessageType.Info);

            using (new EditorGUILayout.HorizontalScope())
            {
                locateReadWrite = EditorGUILayout.ToggleLeft("定位 Read/Write", locateReadWrite, GUILayout.Width(130f));
                locateMipmap = EditorGUILayout.ToggleLeft("定位 Mipmap", locateMipmap, GUILayout.Width(120f));
                includeEditorAssets =
                    EditorGUILayout.ToggleLeft("包含 Editor 目录", includeEditorAssets, GUILayout.Width(130f));

                GUILayout.FlexibleSpace();

                GUI.enabled = locateReadWrite || locateMipmap;
                if (GUILayout.Button("扫描", GUILayout.Width(110f), GUILayout.Height(24f)))
                {
                    ScanProject();
                }

                GUI.enabled = true;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                searchText = EditorGUILayout.TextField("过滤", searchText);

                List<TextureIssue> visibleIssues = GetVisibleIssues();
                GUI.enabled = visibleIssues.Count > 0;
                if (GUILayout.Button("一键关闭当前列表", GUILayout.Width(150f), GUILayout.Height(22f)))
                {
                    ApplySettingsToIssues(visibleIssues, true, true);
                }

                if (GUILayout.Button("关闭 Read/Write", GUILayout.Width(130f), GUILayout.Height(22f)))
                {
                    ApplySettingsToIssues(visibleIssues, true, false);
                }

                if (GUILayout.Button("关闭 Mipmap", GUILayout.Width(120f), GUILayout.Height(22f)))
                {
                    ApplySettingsToIssues(visibleIssues, false, true);
                }

                GUI.enabled = true;
            }
        }

        private void DrawSummary()
        {
            int readableCount = issues.Count(issue => issue.IsReadable);
            int mipmapCount = issues.Count(issue => issue.MipmapEnabled);
            EditorGUILayout.LabelField(lastScanInfo, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                $"当前列表：{issues.Count} 张，Read/Write 开启：{readableCount} 张，Mipmap 开启：{mipmapCount} 张");
        }

        private void DrawIssueList()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("图片路径", EditorStyles.boldLabel, GUILayout.MinWidth(PathColumnMinWidth));
                GUILayout.Label("Read/Write", EditorStyles.boldLabel, GUILayout.Width(85f));
                GUILayout.Label("Mipmap", EditorStyles.boldLabel, GUILayout.Width(70f));
                GUILayout.Label("引用数", EditorStyles.boldLabel, GUILayout.Width(55f));
                GUILayout.Label("首个引用资源", EditorStyles.boldLabel, GUILayout.MinWidth(220f));
                GUILayout.Label("操作", EditorStyles.boldLabel, GUILayout.Width(190f));
            }

            List<TextureIssue> visibleIssues = GetVisibleIssues();
            if (visibleIssues.Count == 0)
            {
                EditorGUILayout.HelpBox(issues.Count == 0 ? "没有可显示的异常图片。点击“扫描”开始检查。" : "当前过滤条件下没有匹配项。",
                    MessageType.None);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (TextureIssue issue in visibleIssues)
            {
                DrawIssueRow(issue);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawIssueRow(TextureIssue issue)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(issue.AssetPath, EditorStyles.label, GUILayout.MinWidth(PathColumnMinWidth)))
                {
                    PingAsset(issue.AssetPath);
                }

                GUILayout.Label(issue.IsReadable ? "开启" : "-", GUILayout.Width(85f));
                GUILayout.Label(issue.MipmapEnabled ? "开启" : "-", GUILayout.Width(70f));
                GUILayout.Label(issue.ReferenceCount.ToString(), GUILayout.Width(55f));

                string firstReference = issue.FirstReferencePath;
                if (GUILayout.Button(firstReference, EditorStyles.label, GUILayout.MinWidth(220f)))
                {
                    PingAsset(firstReference);
                }

                using (new EditorGUILayout.HorizontalScope(GUILayout.Width(190f)))
                {
                    if (GUILayout.Button("定位", GUILayout.Width(48f)))
                    {
                        PingAsset(issue.AssetPath);
                    }

                    GUI.enabled = issue.IsReadable;
                    if (GUILayout.Button("关 R/W", GUILayout.Width(64f)))
                    {
                        ApplySettings(issue, true, false);
                    }

                    GUI.enabled = issue.MipmapEnabled;
                    if (GUILayout.Button("关 Mip", GUILayout.Width(64f)))
                    {
                        ApplySettings(issue, false, true);
                    }

                    GUI.enabled = true;
                }
            }
        }

        private List<TextureIssue> GetVisibleIssues()
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return issues.ToList();
            }

            return issues
                .Where(issue => issue.AssetPath.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                                || issue.FirstReferencePath.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >=
                                0)
                .ToList();
        }

        private void ScanProject()
        {
            issues.Clear();
            Dictionary<string, TextureIssue> referencedTextures = new Dictionary<string, TextureIssue>();
            string[] sourcePaths = AssetDatabase.GetAllAssetPaths()
                .Where(IsReferenceSourceAsset)
                .ToArray();

            try
            {
                for (int i = 0; i < sourcePaths.Length; i++)
                {
                    string sourcePath = sourcePaths[i];
                    if (EditorUtility.DisplayCancelableProgressBar(WindowTitle, sourcePath,
                            (float)i / sourcePaths.Length))
                    {
                        lastScanInfo = "扫描已取消";
                        return;
                    }

                    string[] dependencies = AssetDatabase.GetDependencies(sourcePath, true);
                    foreach (string dependencyPath in dependencies)
                    {
                        if (dependencyPath == sourcePath || !IsProjectAsset(dependencyPath))
                        {
                            continue;
                        }

                        TextureImporter textureImporter = AssetImporter.GetAtPath(dependencyPath) as TextureImporter;
                        if (textureImporter == null)
                        {
                            continue;
                        }

                        TextureIssue issue;
                        if (!referencedTextures.TryGetValue(dependencyPath, out issue))
                        {
                            issue = new TextureIssue(dependencyPath);
                            referencedTextures.Add(dependencyPath, issue);
                        }

                        issue.AddReference(sourcePath);
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            foreach (TextureIssue issue in referencedTextures.Values)
            {
                RefreshIssueImportSettings(issue);
                if (ShouldShowIssue(issue))
                {
                    issues.Add(issue);
                }
            }

            issues.Sort((left, right) =>
                string.Compare(left.AssetPath, right.AssetPath, StringComparison.OrdinalIgnoreCase));
            lastScanInfo = $"扫描完成：共检查 {sourcePaths.Length} 个引用资源，发现 {referencedTextures.Count} 张被引用图片。";
            Repaint();
        }

        private bool IsReferenceSourceAsset(string assetPath)
        {
            if (!IsProjectAsset(assetPath) || AssetDatabase.IsValidFolder(assetPath))
            {
                return false;
            }

            if (!includeEditorAssets && IsEditorAsset(assetPath))
            {
                return false;
            }

            string extension = Path.GetExtension(assetPath);
            return ReferenceSourceExtensions.Contains(extension) && !TextureExtensions.Contains(extension);
        }

        private static bool IsProjectAsset(string assetPath)
        {
            return assetPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsEditorAsset(string assetPath)
        {
            return assetPath.IndexOf("/Editor/", StringComparison.OrdinalIgnoreCase) >= 0
                   || assetPath.StartsWith("Assets/Editor/", StringComparison.OrdinalIgnoreCase);
        }

        private bool ShouldShowIssue(TextureIssue issue)
        {
            return (locateReadWrite && issue.IsReadable) || (locateMipmap && issue.MipmapEnabled);
        }

        private static void RefreshIssueImportSettings(TextureIssue issue)
        {
            TextureImporter importer = AssetImporter.GetAtPath(issue.AssetPath) as TextureImporter;
            if (importer == null)
            {
                issue.IsReadable = false;
                issue.MipmapEnabled = false;
                return;
            }

            issue.IsReadable = importer.isReadable;
            issue.MipmapEnabled = importer.mipmapEnabled;
        }

        private void ApplySettingsToIssues(List<TextureIssue> targets, bool disableReadWrite, bool disableMipmap)
        {
            if (targets.Count == 0)
            {
                return;
            }

            if (!EditorUtility.DisplayDialog(WindowTitle, $"确认要批量修改当前列表中的 {targets.Count} 张图片导入设置吗？", "确认修改", "取消"))
            {
                return;
            }

            int changedCount = 0;

            AssetDatabase.StartAssetEditing();
            try
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    TextureIssue issue = targets[i];
                    EditorUtility.DisplayProgressBar("修改图片导入设置", issue.AssetPath, (float)i / targets.Count);
                    if (ApplySettingsInternal(issue, disableReadWrite, disableMipmap))
                    {
                        changedCount++;
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                EditorUtility.ClearProgressBar();
            }

            RemoveFixedIssues();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[{WindowTitle}] 已修改 {changedCount} 张图片导入设置。");
        }

        private void ApplySettings(TextureIssue issue, bool disableReadWrite, bool disableMipmap)
        {
            if (ApplySettingsInternal(issue, disableReadWrite, disableMipmap))
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                RemoveFixedIssues();
                Debug.Log($"[{WindowTitle}] 已修改：{issue.AssetPath}");
            }
        }

        private static bool ApplySettingsInternal(TextureIssue issue, bool disableReadWrite, bool disableMipmap)
        {
            TextureImporter importer = AssetImporter.GetAtPath(issue.AssetPath) as TextureImporter;
            if (importer == null)
            {
                return false;
            }

            bool changed = false;
            if (disableReadWrite && importer.isReadable)
            {
                importer.isReadable = false;
                changed = true;
            }

            if (disableMipmap && importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
                changed = true;
            }

            if (!changed)
            {
                RefreshIssueImportSettings(issue);
                return false;
            }

            importer.SaveAndReimport();
            RefreshIssueImportSettings(issue);
            return true;
        }

        private void RemoveFixedIssues()
        {
            issues.RemoveAll(issue =>
            {
                RefreshIssueImportSettings(issue);
                return !ShouldShowIssue(issue);
            });
            Repaint();
        }

        private static void PingAsset(string assetPath)
        {
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            if (asset == null)
            {
                return;
            }

            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        private sealed class TextureIssue
        {
            private readonly HashSet<string> referencePaths = new HashSet<string>();

            public TextureIssue(string assetPath)
            {
                AssetPath = assetPath;
            }

            public string AssetPath { get; private set; }
            public bool IsReadable { get; set; }
            public bool MipmapEnabled { get; set; }

            public int ReferenceCount
            {
                get { return referencePaths.Count; }
            }

            public string FirstReferencePath { get; private set; }

            public void AddReference(string referencePath)
            {
                if (!referencePaths.Add(referencePath))
                {
                    return;
                }

                if (string.IsNullOrEmpty(FirstReferencePath))
                {
                    FirstReferencePath = referencePath;
                }
            }
        }
    }
}
