using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using LitJson;
#endif

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class SpriteToConvexCollider : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polygonCollider;
    
    // 移除 Awake() 方法，改为通过编辑器菜单手动触发。
    // Remove the Awake() method, change to manually trigger via the editor menu.

    /// <summary>
    /// 根据Sprite的形状生成一个凸多边形碰撞体。
    /// This method gets the sprite's vertices, calculates the convex hull, and updates the PolygonCollider2D.
    /// </summary>
    [ContextMenu("Generate Convex Collider")]
    public void GenerateConvexCollider()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();

        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            Debug.LogError("SpriteRenderer or Sprite is missing!");
            return;
        }

        Vector2[] vertices = null;

        // Check if a CircleCollider2D exists on the GameObject.
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            // If a CircleCollider2D is present, convert it to a polygon.
            // We'll approximate the circle with a 16-sided regular polygon for a good balance of detail and performance.
            int sides = 16;
            vertices = new Vector2[sides];
            float angleStep = 360f / sides;
            for (int i = 0; i < sides; i++)
            {
                float angle = Mathf.Deg2Rad * (angleStep * i);
                float x = circleCollider.radius * Mathf.Cos(angle) + circleCollider.offset.x;
                float y = circleCollider.radius * Mathf.Sin(angle) + circleCollider.offset.y;
                vertices[i] = new Vector2(x, y);
            }
            Debug.Log("Generated a polygon from CircleCollider2D.");
        }
        else
        {
            // If no CircleCollider2D, get the sprite's vertices as a fallback.
            vertices = spriteRenderer.sprite.vertices;

            // Compute the convex hull from the vertices.
            vertices = GetConvexHull(vertices);
            Debug.Log("Generated a convex polygon from Sprite vertices.");
        }

        // Update the PolygonCollider2D with the new path.
        polygonCollider.SetPath(0, vertices);
        
        Debug.Log("PolygonCollider2D successfully updated with a convex shape.");
    }

    /// <summary>
    /// Implements the Monotone Chain algorithm to find the convex hull of a set of points.
    /// This algorithm efficiently finds the smallest convex polygon that contains all points.
    /// </summary>
    /// <param name="points">The input array of Vector2 points.</param>
    /// <returns>An array of Vector2 points representing the vertices of the convex hull.</returns>
    private Vector2[] GetConvexHull(Vector2[] points)
    {
        if (points.Length <= 3)
        {
            // If there are 3 or fewer points, they already form a convex shape.
            return points;
        }

        // Sort the points by x-coordinate, then by y-coordinate.
        System.Array.Sort(points, (a, b) =>
        {
            int comparison = a.x.CompareTo(b.x);
            if (comparison == 0)
            {
                comparison = a.y.CompareTo(b.y);
            }
            return comparison;
        });

        // Build the lower hull.
        var lowerHull = new List<Vector2>();
        foreach (Vector2 p in points)
        {
            while (lowerHull.Count >= 2 && CrossProduct(lowerHull[lowerHull.Count - 2], lowerHull[lowerHull.Count - 1], p) <= 0)
            {
                lowerHull.RemoveAt(lowerHull.Count - 1);
            }
            lowerHull.Add(p);
        }

        // Build the upper hull.
        var upperHull = new List<Vector2>();
        for (int i = points.Length - 1; i >= 0; i--)
        {
            Vector2 p = points[i];
            while (upperHull.Count >= 2 && CrossProduct(upperHull[upperHull.Count - 2], upperHull[upperHull.Count - 1], p) <= 0)
            {
                upperHull.RemoveAt(upperHull.Count - 1);
            }
            upperHull.Add(p);
        }

        // Combine the hulls and remove the last point to avoid duplication.
        lowerHull.RemoveAt(lowerHull.Count - 1);
        upperHull.RemoveAt(upperHull.Count - 1);
        return lowerHull.Concat(upperHull).ToArray();
    }

    /// <summary>
    /// Helper function to calculate the cross product of three points.
    /// A positive value indicates a counter-clockwise turn.
    /// A negative value indicates a clockwise turn.
    /// A zero value indicates the points are collinear.
    /// </summary>
    private float CrossProduct(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpriteToConvexCollider))]
public class SpriteToConvexColliderEditor : Editor
{
    private string serializedPoints;

    public override void OnInspectorGUI()
    {
        // Draw the default inspector for the script.
        DrawDefaultInspector();

        SpriteToConvexCollider myScript = (SpriteToConvexCollider)target;
        PolygonCollider2D polyCollider = myScript.GetComponent<PolygonCollider2D>();

        EditorGUILayout.Space(20);

        EditorGUILayout.LabelField("PolygonCollider2D Points Data", EditorStyles.boldLabel);

        if (GUILayout.Button("Serialize Points to Text"))
        {
            if (polyCollider != null && polyCollider.points != null)
            {
                // Serialize the points array to a JSON string using LitJson.
                JsonData jsonData = new JsonData();
                foreach (var point in polyCollider.points)
                {
                    JsonData pointData = new JsonData();
                    pointData["x"] = point.x;
                    pointData["y"] = point.y;
                    jsonData.Add(pointData);
                }
                serializedPoints = JsonMapper.ToJson(jsonData);
            }
            else
            {
                serializedPoints = "No points found on PolygonCollider2D.";
            }
        }
        
        // Display the serialized points in a multi-line text area.
        EditorGUILayout.LabelField("Serialized Points:");
        serializedPoints = EditorGUILayout.TextArea(serializedPoints, GUILayout.Height(100));

        // Display a button to copy the text to the clipboard.
        if (GUILayout.Button("Copy to Clipboard"))
        {
            EditorGUIUtility.systemCopyBuffer = serializedPoints;
            Debug.Log("Points data copied to clipboard.");
        }

        // NEW: Button to load points from the text area.
        if (GUILayout.Button("Load Points from Text"))
        {
            try
            {
                JsonData jsonData = JsonMapper.ToObject(serializedPoints);
                if (jsonData != null && jsonData.IsArray)
                {
                    List<Vector2> loadedPoints = new List<Vector2>();
                    for (int i = 0; i < jsonData.Count; i++)
                    {
                        float x = (float)(double)jsonData[i]["x"];
                        float y = (float)(double)jsonData[i]["y"];
                        loadedPoints.Add(new Vector2(x, y));
                    }

                    if (loadedPoints.Count > 0)
                    {
                        polyCollider.SetPath(0, loadedPoints.ToArray());
                        Debug.Log("PolygonCollider2D successfully updated from text data.");
                    }
                    else
                    {
                        Debug.LogError("JSON array is empty or invalid.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to deserialize JSON data. Check the format.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error parsing JSON: {e.Message}");
            }
        }
    }
}

#endif
