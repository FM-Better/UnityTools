using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class GeometryUtil
    {
        /// <summary>
        /// 判断一个点是否在一个凸多边形中
        /// </summary>
        /// <param name="point"> 要判断的点 </param>
        /// <param name="convexPolygon"> 按照顺序(顺/逆)组成凸多边形的的边界点列表 </param>
        /// <returns></returns>
        private static bool IsPointInConvexPolygon(Vector2 point, List<Vector2> convexPolygon)
        {
            var nowState = 0; // 0表示未进行叉乘 -1表示叉乘结果为负 1表示叉乘结果为正
                
            for (int i = 0; i < convexPolygon.Count; i++)
            {
                var nextIndex = i + 1;
                if (nextIndex >= convexPolygon.Count) // 末尾的点的下一个点为起点
                    nextIndex = 0;

                var dir2NextVertex = convexPolygon[nextIndex] - convexPolygon[i]; // 指向下一个点的向量
                var dir2Point = point - convexPolygon[i]; // 指向要判断的点的向量
                var crossSign = Vector2Util.SignCross(dir2NextVertex, dir2Point); // 两条向量叉乘结果的正负
                    
                if (nowState == 0) // 还未进行叉乘
                {
                    nowState = crossSign;
                }
                else
                {
                    if (crossSign != nowState) // 若和第一次的结果不一致则说明不在凸多边形内
                        return false;
                }
            }

            return true; // 否则 在凸多边形中
        }
        
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
    }
}