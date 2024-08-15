using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTool
{
    [CustomEditor(typeof(PolygonCollider2D)), CanEditMultipleObjects]
    public class PolygonCollider2DInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("UpdateByPhysicsShape"))
            {
                foreach (var obj in targets) // 支持多个PolygonCollider2D一起修改
                {
                    var collider = ((PolygonCollider2D)obj).GetComponent<PolygonCollider2D>();

                    if (collider.TryGetComponent(out SpriteRenderer spriteRenderer))
                    {
                        var shapeCount = spriteRenderer.sprite.GetPhysicsShapeCount();

                        if (shapeCount <= 0)
                        {
                            Debug.LogError($"PolygonCollider UpdateByPhysicsShape: {name}对应的图片没有勾选Generate Physics Shape选项，无法进行对应更新");
                        }
                        else
                        {
                            collider.pathCount = shapeCount;

                            var path = new List<Vector2>();

                            for (int i = 0; i < collider.pathCount; i++)
                            {
                                path.Clear();
                                spriteRenderer.sprite.GetPhysicsShape(i, path); // 根据sprite的物理形状获取对应边缘点的path
                                collider.SetPath(i, path.ToArray());
                            }    
                        }
                    }
                }
            }
        }
    }
}
