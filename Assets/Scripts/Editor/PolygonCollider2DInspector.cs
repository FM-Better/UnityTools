using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PolygonCollider2D)), CanEditMultipleObjects]
public class PolygonCollider2DTool : Editor
{
    private Editor _baseEditor;
    private float _radius;

    private void OnEnable()
    {
        EditorCoroutineUtility.StartCoroutineOwnerless(Cor_CreateBaseEditor());
    }

    private IEnumerator Cor_CreateBaseEditor()
    {
        var type = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.PolygonCollider2DEditor", false); // 根据反射获取基类Editor
        yield return new WaitUntil(() => type != null);
        _baseEditor = CreateEditor(target, type);
    }

    public override void OnInspectorGUI()
    {
        if (!_baseEditor) // 未获取到基类Editor 则推出
            return;
        
        _baseEditor.OnInspectorGUI();

        _radius = EditorGUILayout.FloatField("Radius", _radius);
        
        if (GUILayout.Button("Update"))
        {
            foreach (var obj in targets) // 支持多个PolygonCollider2D一起修改
            {
                var collider = ((PolygonCollider2D)obj).GetComponent<PolygonCollider2D>();

                if (collider.TryGetComponent(out SpriteRenderer spriteRenderer)) // 有图片才能获取信息更新
                {
                    var shapeCount = spriteRenderer.sprite.GetPhysicsShapeCount();

                    if (shapeCount <= 0)
                    {
                        Debug.LogError($"PolygonCollider Update: {name}对应的图片没有勾选Generate Physics Shape选项，无法进行对应更新");
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
                    
                    if (_radius > 0f) // 有半径才外扩
                    {
                        var originalPoints = collider.points;
                        var expandedPoints = new Vector2[originalPoints.Length];
                        for (int i = 0; i < originalPoints.Length; i++)
                        {
                            var p1 = originalPoints[i] + (Vector2)spriteRenderer.bounds.center;
                            var p2 = originalPoints[(i + 1) % originalPoints.Length] + (Vector2)spriteRenderer.bounds.center; // 下一个顶点

                            var tangent = p2 - p1;
                            
                            var normal = new Vector2(tangent.y, -tangent.x).normalized; // 计算法线方向
                            expandedPoints[i] = originalPoints[i] + normal * _radius;
                        }

                        collider.points = expandedPoints;    
                    }
                }
                else
                {
                    Debug.LogError($"PolygonCollider Update: {name}没有挂载SpriteRenderer组件，无法进行对应更新");
                }
            }
        }
    }
}
