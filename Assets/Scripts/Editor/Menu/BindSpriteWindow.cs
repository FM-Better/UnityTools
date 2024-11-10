using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    public class BindSpriteWindow : EditorWindow
    {
        [MenuItem("Tool/Bind Sprite Window %&B")]
        public static void CreateWindow()
        {
            var windowRect = new Rect(0, 0, 500, 500);
            var window = GetWindowWithRect(typeof(BindSpriteWindow), windowRect, false, "Bind Sprite Window");
            window.Show();
        }

        private List<Sprite> _sprites = new List<Sprite>();
        private List<SpriteRenderer> _spriteRenderers = new List<SpriteRenderer>();

        private Vector2 _spriteScrollPos = Vector2.zero;
        private Vector2 _spriteRendererScrollPos = Vector2.zero;

        private int _selectedNum;
        
         private void OnGUI()
        {
            GUILayout.Space(10);

            DrawTittle();
            
            GUILayout.Space(20);
            
            GUI.skin.label.fontSize = 15;
            GUI.color = Color.cyan;
            GUILayout.Label($"当前选择{_selectedNum}个物体");
            GUI.color = Color.white;
            
            GUILayout.Space(10);
            
            DrawSprites();

            GUILayout.Space(10);
            
            DrawSpriteRenderers();
            
            GUILayout.Space(10);
            
            if (Selection.objects.Length <= 0)
            {
                GUI.color = Color.red;
                GUILayout.Button("没有选中物体!");
            }
            else
            {
                DrawSpriteTool();
                
                DrawSpriteRendererTool();
            }

            DrawBindTool();
            
            GUILayout.Space(20);
        }
        
        private void OnSelectionChange() => _selectedNum = Selection.objects.Length;

        #region Sprite

        private void DrawSpriteTool()
        {
            if (CheckSprite())
            {
                if (GUILayout.Button("获取Sprites"))
                {
                    GetSprites();
                }    
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                    
                GUI.color = Color.red;
                GUILayout.Button("有物体不为Sprite资源，无法获取");
                    
                GUI.color = Color.white;
                if(GUILayout.Button("清空Sprite"))
                {
                    _sprites.Clear();
                }
                    
                EditorGUILayout.EndHorizontal();
            }
        }
        
        private void DrawSprites()
        {
            EditorGUILayout.LabelField($"已获取 {_sprites.Count}个 Sprite");

            if (_sprites.Count > 0)
            {
                _spriteScrollPos = EditorGUILayout.BeginScrollView(_spriteScrollPos);
                foreach (var sprite in _sprites)
                {
                    EditorGUILayout.ObjectField(sprite, typeof(Sprite), true);
                }
                EditorGUILayout.EndScrollView();    
            }
        }

        private bool CheckSprite()
        {
            foreach (var selectObj in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(selectObj);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                
                if (!sprite || !AssetDatabase.Contains(selectObj)) // 若不为Sprite 或 不存在Asset中 则失败
                {
                    return false;
                }
            }

            return true;
        }
        
        private void GetSprites()
        {
            _sprites.Clear();
            _sprites.Capacity = Selection.objects.Length;
            
            foreach (var selectObj in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(selectObj);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                _sprites.Add(sprite);
            }
        }
        
        #endregion

        #region SpriteRenderer

        private void DrawSpriteRendererTool()
        {
            if (CheckSpriteRenderer())
            {
                if (GUILayout.Button("获取SpriteRender"))
                {
                    GetSpriteRenderers();
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                    
                GUI.color = Color.red;
                GUILayout.Button("有物体不为SpriteRenderer，无法获取");
                    
                GUI.color = Color.white;
                if(GUILayout.Button("清空SpriteRenderer"))
                {
                    _spriteRenderers.Clear();
                }
                    
                EditorGUILayout.EndHorizontal();
            }
        }
        
        private void DrawSpriteRenderers()
        {
            EditorGUILayout.LabelField($"已获取 {_spriteRenderers.Count}个 SpriteRenderer");
            if (_spriteRenderers.Count > 0)
            {
                _spriteRendererScrollPos = EditorGUILayout.BeginScrollView(_spriteRendererScrollPos);
                foreach (var spriteRenderer in _spriteRenderers)
                {
                    EditorGUILayout.ObjectField(spriteRenderer, typeof(SpriteRenderer), false);
                }
                EditorGUILayout.EndScrollView();    
            }
        }
        
        private bool CheckSpriteRenderer()
        {
            foreach (var selectObj in Selection.objects)
            {
                var spriteRenderer = (selectObj as GameObject)?.GetComponent<SpriteRenderer>();
                var inAsset = AssetDatabase.Contains(selectObj);
                if (spriteRenderer is null || inAsset) // 若不为SpriteRenderer 或 存在Asset中 则失败
                {
                    return false;
                }
            }

            return true;
        }
        
        private void GetSpriteRenderers()
        {
            _spriteRenderers.Clear();
            _spriteRenderers.Capacity = Selection.objects.Length;
            
            foreach (var selectObj in Selection.objects)
            {
                _spriteRenderers.Add((selectObj as GameObject)?.GetComponent<SpriteRenderer>());
            }
        }
        
        #endregion

        private void DrawTittle()
        {
            GUI.color = Color.yellow;
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Bind Sprite Window");
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.color = Color.white;
        }
        
        private void DrawBindTool()
        {
            if (_sprites.Count == 0 || _spriteRenderers.Count == 0)
            {
                GUI.color = Color.red;
                GUILayout.Button("请保证已获取的Sprite和SpriteRenderer都至少有一个");
            }
            else
            {
                GUI.color = Color.cyan;
                if (GUILayout.Button("绑定Sprite到SpriteRenderer上"))
                {
                    var bindCount = 0;
                    for (int i = 0; i < _sprites.Count && i < _spriteRenderers.Count; i++, bindCount ++)
                    {
                        _spriteRenderers[i].sprite = _sprites[i];
                    }

                    EditorUtility.DisplayDialog("绑定完成", $"成功绑定了{bindCount}个Sprite到SpriteRenderer上", "确认");
                }   
            }
        }
    }
}
