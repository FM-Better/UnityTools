using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class GeometryUtil
    {
        #region ConvexPolygon相关
        
        /// <summary>
        /// 判断一个点是否在一个凸多边形中
        /// </summary>
        /// <param name="point"> 要判断的点 </param>
        /// <param name="convexPolygon"> 按照顺序(顺/逆)组成凸多边形的的边界点列表 </param>
        /// <returns></returns>
        private static bool IsPointInConvexPolygon(Vector2 point, List<Vector2> convexPolygon)
        {
            var hasCrossed = false; // 是否进行过叉乘
            var firstCrossResult = false; // 第一次的叉乘结果

            for (int i = 0; i < convexPolygon.Count; i++)
            {
                var nextIndex = (i + 1) % convexPolygon.Count; // 末尾的点的下一个点为起点

                var dir2NextVertex = convexPolygon[nextIndex] - convexPolygon[i]; // 指向下一个点的向量
                var dir2Point = point - convexPolygon[i]; // 指向要判断的点的向量
                var crossSign = Vector2Util.SignCross(dir2NextVertex, dir2Point); // 叉乘值的符号

                if (!hasCrossed) // 还未进行叉乘
                {
                    hasCrossed = true;
                    firstCrossResult = crossSign == 1;
                }
                else
                {
                    var nowResult = crossSign == 1;
                    if (firstCrossResult != nowResult) // 若和第一次的结果不一致 则说明不在凸多边形内
                        return false;
                }
            }

            return true; // 否则 在凸多边形中
        }
        
        /// <summary>
        /// 判断一个点是否在一个凸多边形的边界上
        /// </summary>
        /// <param name="point"> 要判断的点 </param>
        /// <param name="convexPolygon"> 按照顺序(顺/逆)组成凸多边形的的边界点列表 </param>
        /// <returns></returns>
        public static bool IsPointOnConvexPolygonBoundary(Vector2 point, List<Vector2> convexPolygon)
        {
            for (int i = 0; i < convexPolygon.Count; i ++)
            {
                var nextIndex = (i + 1) % convexPolygon.Count; // 末尾的点的下一个点为起点
                
                if (IsPointOnLine(point, convexPolygon[i], convexPolygon[nextIndex]))
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 计算一个点距离凸多边形边界上的最近点
        /// </summary>
        /// <param name="point"> 传入的点 </param>
        /// <param name="convexPolygon"> 按照顺序(顺/逆)组成凸多边形的的边界点列表 </param>
        /// <param name="edgeIndex"> 最近点所在的多边形边的索引 </param>
        /// <returns> 计算出的最近点 </returns>
        public static Vector2 CalculateNearestConvexPolygonBoundaryPoint(Vector2 point, List<Vector2> convexPolygon, out int edgeIndex)
        {
            var nearestPoint = Vector2.zero;
            var minDistance = float.MaxValue;
            edgeIndex = 0;

            for (int i = 0; i < convexPolygon.Count; i++)
            {
                var nextIndex = (i + 1) % convexPolygon.Count; // 末尾的点的下一个点为起点
                var start2EndDir = (convexPolygon[nextIndex] - convexPolygon[i]).normalized;
                var start2Point = point - convexPolygon[i];
                var dotResult = Vector2.Dot(start2Point, start2EndDir.normalized);
                var verticalPoint = convexPolygon[i] + dotResult * start2EndDir;
                
                var sqrtDistance = (point - verticalPoint).sqrMagnitude;
                if (sqrtDistance < minDistance)
                {
                    minDistance = sqrtDistance;
                    nearestPoint = verticalPoint;
                    edgeIndex = i;
                }
            }
            
            return nearestPoint;
        }
        
        #endregion
        
        #region Line相关
        
        /// <summary>
        /// 判断一个点是否在一条线段上
        /// </summary>
        /// <param name="point"> 要判断的点 </param>
        /// <param name="startPoint"> 线段起点 </param>
        /// <param name="endPoint"> 线段终点 </param>
        /// <returns> 是否在线段上 </returns>
        public static bool IsPointOnLine(Vector2 point, Vector2 startPoint, Vector2 endPoint)
        {
            var start2Point = point - startPoint;
            var end2Point = point - endPoint;
            var crossResult = Vector2Util.Cross(start2Point, end2Point);
            var dotResult = Vector2.Dot(start2Point, end2Point);
            
            return Mathf.Abs(crossResult) <= 1e-5 && dotResult <= 0f; // 三点共线且反向
        }
        
        /// <summary>
        /// 两条线段是否相交
        /// </summary>
        /// <param name="line1"> 线段1 </param>
        /// <param name="line2"> 线段2 </param>
        /// <returns> 是否相交 </returns>
        public static bool IsLineIntersection(Tuple<Vector2, Vector2> line1, Tuple<Vector2, Vector2> line2) 
        {
            var a = line1.Item1;
            var b = line1.Item2;
            var c = line2.Item1;
            var d = line2.Item2;
            
            var ab = b - a;
            var ad = d - a;
            var ca = a - c;
            var cd = d - c;
            var cb = b - c;
            
            // 快速排斥 (包围盒检测
            if (Mathf.Min(a.x, b.x) > Mathf.Max(c.x, d.x) || Mathf.Max(a.x, b.x) < Mathf.Min(c.x, d.x) || 
                Mathf.Min(a.y, b.y) > Mathf.Max(c.y, d.y) || Mathf.Max(a.y, b.y) < Mathf.Min(c.y, d.y))
                return false;
            
            // 跨立试验
            if (Vector2Util.Cross(-ca, ab) * Vector2Util.Cross(ab, ad) > 0
                && Vector2Util.Cross(ca, cd) * Vector2Util.Cross(cd, cb) > 0)
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 计算两条直线的交点
        /// </summary>
        /// <param name="line1"> 直线1 </param>
        /// <param name="line2"> 直线2 </param>
        /// <returns></returns>
        public static Vector2 CalculateStraightLineIntersectPoint(Tuple<Vector2, Vector2> line1, Tuple<Vector2, Vector2> line2)
        {
            var a = line1.Item1;
            var b = line1.Item2;
            var c = line2.Item1;
            var d = line2.Item2;
            
            var ab = b - a;
            var ca = a - c;
            var cd = d - c;
            
            var v1 = Vector2Util.Cross(ca, cd);
            var v2 = Vector2Util.Cross(cd, ab);
            var ratio = v1 / v2;
            var intersectPoint = a + ab * ratio; 
            
            return intersectPoint;
        }
        
        #endregion

        #region Triangle相关

        /// <summary>
        /// 判断一个点是否在一个三角形内
        /// </summary>
        /// <param name="point"> 要判断的点 </param>
        /// <param name="triangle"> 组成三角形的点的列表 </param>
        /// <returns></returns>
        private static bool IsPointInTriangle(Vector2 point, List<Vector2> triangle)
        {
            var v1 = triangle[1] - triangle[0];
            var v2 = triangle[2] - triangle[1];
            var v3 = triangle[0] - triangle[2];

            var point2V1 = triangle[0] - point;
            var point2V2 = triangle[1] - point;
            var point2V3 = triangle[2] - point;

            return Vector2Util.Cross(v1, point2V1) * Vector2Util.Cross(v2, point2V2) > 0 &&
                   Vector2Util.Cross(v2, point2V2) * Vector2Util.Cross(v3, point2V3) > 0;
        }

        #endregion
    }
}