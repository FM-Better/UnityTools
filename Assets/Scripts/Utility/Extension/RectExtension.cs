using System.Runtime.CompilerServices;
using UnityEngine;

namespace Utility
{
    public static class RectExtension
    {
        #region 并集
        
        /// <summary>
        /// 获取两个矩形的并集区域
        /// </summary>
        /// <param name="rect"> 当前矩形 </param>
        /// <param name="otherRect"> 另一个矩形 </param>
        /// <returns> 并集 </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetUnionRegion(this Rect rect, Rect otherRect)
        {
            var minX = Mathf.Min(rect.xMin, otherRect.xMin);
            var minY = Mathf.Min(rect.yMin, otherRect.yMin);
            var maxX = Mathf.Max(rect.xMax, otherRect.xMax);
            var maxY = Mathf.Max(rect.yMax, otherRect.yMax);

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
        
        /// <summary>
        /// 扩展当前矩形使其能够完全包含指定的另一个矩形
        /// </summary>
        /// <param name="rect"> 当前矩形 </param>
        /// <param name="otherRect"> 需要被包含的矩形 </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Encapsulate(this ref Rect rect, Rect otherRect)
        {
            var minX = Mathf.Min(rect.xMin, otherRect.xMin);
            var minY = Mathf.Min(rect.yMin, otherRect.yMin);
            var maxX = Mathf.Max(rect.xMax, otherRect.xMax);
            var maxY = Mathf.Max(rect.yMax, otherRect.yMax);

            rect.Set(minX, minY, maxX - minX, maxY - minY);
        }
        
        #endregion

        #region 交集
        
        /// <summary>
        /// 获取两个矩形的重叠面积 没有则返回0
        /// </summary>
        /// <param name="rect"> 当前矩形 </param>
        /// <param name="otherRect"> 另一个矩形 </param>
        /// <returns> 重叠面积 </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetOverlapArea(this Rect rect, Rect otherRect)
        {
            if (rect.xMin > otherRect.xMax || rect.xMax < otherRect.xMin ||
                rect.yMin > otherRect.yMax || rect.yMax < otherRect.yMin) // 没有重叠区域
                return 0;

            var overlapMin = Vector2.Max(rect.min, otherRect.min); // 重叠的min
            var overlapMax = Vector2.Min(rect.max, otherRect.max); // 重叠的max 
            var overlapRect = Rect.MinMaxRect(overlapMin.x, overlapMin.y, overlapMax.x, overlapMax.y);

            return overlapRect.width * overlapRect.height; // 返回重叠面积
        }

        /// <summary>
        /// 判断两个矩形是否重叠
        /// </summary>
        /// <param name="rect"> 当前矩形 </param>
        /// <param name="otherRect"> 另一个矩形 </param>
        /// <returns> 是否重叠 </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOverlap(this ref Rect rect, Rect otherRect) =>
            rect.xMin <= otherRect.xMax && rect.xMax >= otherRect.xMin && 
            rect.yMin <= otherRect.yMax && rect.yMax >= otherRect.yMin;

        #endregion
    }
}
