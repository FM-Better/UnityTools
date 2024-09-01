using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    public class Texture2DSettingTool : EditorWindow
    {
        [MenuItem("Tool/Texture2D Setting Tool %&S")]
        public static void CreateWindow()
        {
            var windowRect = new Rect(0, 0, 500, 500);
            var window = GetWindowWithRect(typeof(Texture2DSettingTool), windowRect, false, "Texture2D Setting Tool");
            window.Show();
        }

        private TextureImporterType _textureImporterType;
        
        private int _selectedNum;
        private bool _selectTexture2D;

        private void OnEnable()
        {
            CheckSelectionObjects();
        }
        
        private void OnGUI()
        {
            #region Tool Title
            GUILayout.Space(10);
            
            GUI.color = Color.yellow;
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Texture2D Setting Tool");
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.color = Color.white;
            
            GUILayout.Space(20);
            #endregion
            
            GUI.skin.label.fontSize = 20;
            GUILayout.Label($"当前选择{_selectedNum}个物体");
            
            GUILayout.Space(10);

            _textureImporterType = (TextureImporterType)EditorGUILayout.EnumPopup("图片类型：", _textureImporterType);
            
            GUILayout.Space(10);
            
            if (Selection.objects.Length <= 0)
            {
                GUI.color = Color.red;
                GUILayout.Button("没有选中物体!");
            }
            else if (!_selectTexture2D)
            {
                GUI.color = Color.red;
                GUILayout.Button("选中的物体中有文件不为图片格式,请取消选择!");
            }
            else 
            {
                if (GUILayout.Button("设置图片"))
                {
                    SetSprite();
                }
            }
        }

        private void OnSelectionChange()
        {
            CheckSelectionObjects();
        }

        private void CheckSelectionObjects()
        {
            foreach (var asset in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                if (!IsTexture2D(path))
                {
                    _selectTexture2D = false;
                    return;
                }
            }

            _selectTexture2D = true;
            _selectedNum = Selection.objects.Length;
        }
        
        private bool IsTexture2D(string assetPath)
        {
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (assetType == typeof(Texture2D))
            {
                return true;
            }

            return false;
        }
        
        private void SetSprite()
        {
            foreach (var asset in Selection.objects)
            {
                var assetPath = AssetDatabase.GetAssetPath(asset);
        
                if (AssetDatabase.Contains(asset)) // 为Asset文件夹下资源物体
                {
                    var importer = AssetImporter.GetAtPath(assetPath);
                    var textureImporter = importer as TextureImporter;

                    if (textureImporter)
                    {
                        textureImporter.textureType = _textureImporterType;
                        textureImporter.alphaIsTransparency = true;
                        textureImporter.mipmapEnabled = false;
                        textureImporter.wrapMode = TextureWrapMode.Clamp;
                        textureImporter.filterMode = FilterMode.Bilinear;
                        textureImporter.textureCompression = TextureImporterCompression.Compressed;
                        textureImporter.compressionQuality = 100;
                        
                        AssetDatabase.ImportAsset(assetPath);
                    }
                    AssetDatabase.SaveAssets();
                }
            }
            
            AssetDatabase.Refresh(); // 刷新
        }
    }    
}
