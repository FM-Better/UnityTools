using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorTool
{
    public class RenameTool : EditorWindow
    {
        [MenuItem("Tool/Rename Tool %&R")]
        public static void CreateWindow()
        {
            var windowRect = new Rect(0, 0, 500, 500);
            var window = GetWindowWithRect(typeof(RenameTool), windowRect, false, "Rename Tool");
            window.Show();
        }
        
        private string _prefix = "";
        private bool _isRetain = false;
        private string _name = "";
        private string _suffix = "";
        private int _id = 1;
        private bool _isSingle = false;
        private string _format = "";
        
        private int _selectedNum;
        private string _currentScenePath;

        private void OnEnable()
        {
            _selectedNum = Selection.objects.Length;
        }
        
        private void OnGUI()
        {
            #region Tool Title
            GUILayout.Space(10);
            
            GUI.color = Color.yellow;
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Rename Tool");
            
            GUILayout.Space(20);
            #endregion

            #region Selection Tip
            GUI.color = Color.white;
            GUI.skin.label.fontSize = 20;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label($"当前选择{_selectedNum}个物体");
            
            GUILayout.Space(10);
            #endregion
            
            
            _prefix = EditorGUILayout.TextField("前缀：", _prefix);
            _isRetain = EditorGUILayout.ToggleLeft("是否保留原名", _isRetain);
            if (!_isRetain) // 不保留才需要输入名称
            {
                _name = EditorGUILayout.TextField("名称：", _name);
            }
            _suffix = EditorGUILayout.TextField("后缀：", _suffix);
            
            GUILayout.Space(10);
            
            _isSingle = EditorGUILayout.ToggleLeft("是否单个物体", _isSingle);
            if (!_isSingle) // 多个才需要输入ID相关
            {
                _id = EditorGUILayout.IntField("起始ID：", _id);
                _format = EditorGUILayout.TextField("ID格式化标准：", _format);    
            }
            
            GUILayout.Space(20);
            
            if (Selection.objects.Length <= 0)
            {
                GUI.color = Color.red;
                GUILayout.Button("没有选中物体!");
            }
            else 
            {
                if (GUILayout.Button("重命名"))
                {
                    _currentScenePath = SceneManager.GetActiveScene().path;
                    
                    Rename();
                }
            }
        }
        
        private void OnSelectionChange()
        {
            _selectedNum = Selection.objects.Length;
        }
        
        private void Rename()
        {
            if (!_isRetain && (_name.Length == 0 || _name.Trim().Length == 0))
            {
                Debug.LogError("名称为空");
                return;
            }
        
            foreach (var asset in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                var newName = asset.name;
                if (!_isRetain)
                {
                    newName = _name;
                }
                if (!_isSingle)
                {
                    newName = $"{_name}_{_id.ToString(_format)}";    
                }
        
                var prefix = _prefix.Trim(); // 前缀 去除空白部分
                if (prefix.Length > 0)
                    newName = $"{prefix}_{newName}";
                
                var suffix = _suffix.Trim(); // 后缀 去除空白部分
                if (suffix.Length > 0)
                    newName = $"{newName}_{suffix}";
        
                if (asset is GameObject && !AssetDatabase.Contains(asset)) // 若为GameObject 且 不存在Asset中 则为场景物体
                {
                    asset.name = newName;
                    UnityEditor.SceneManagement.EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), _currentScenePath);
                }
                else if (AssetDatabase.Contains(asset)) // 否则存在Asset中 则为Asset文件夹下资源物体
                {
                    AssetDatabase.RenameAsset(path, newName);
                    AssetDatabase.SaveAssets();
                }
        
                _id++; // id递增
            }
            
            AssetDatabase.Refresh(); // 刷新Project窗口
        }
    }    
}
