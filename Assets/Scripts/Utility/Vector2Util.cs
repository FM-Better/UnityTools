using System;
using UnityEngine;

namespace Utility
{
    public static class Vector2Util
    {
        /// <summary>
        /// 获取叉乘值
        /// </summary>
        /// <param name="lVector2"></param>
        /// <param name="rVector2"></param>
        /// <returns></returns>
        public static float Cross(Vector2 lVector2, Vector2 rVector2) =>
            lVector2.x * rVector2.y - lVector2.y * rVector2.x;
        
        /// <summary>
        /// 获取叉乘值的符号
        /// </summary>
        /// <param name="lVector2"></param>
        /// <param name="rVector2"></param>
        /// <returns></returns>
        public static int SignCross(Vector2 lVector2, Vector2 rVector2)
        {
            var result = Cross(lVector2, rVector2);
            if (Mathf.Abs(result) <= 1e-5)
            {
                return 0;
            }

            return result > 0 ? 1 : -1;
        }
    }
}