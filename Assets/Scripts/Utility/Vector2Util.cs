using System;
using UnityEngine;

namespace Utility
{
    public static class Vector2Util
    {
        /// 得到两条Vector2向量叉乘结果
        public static float Cross(Vector2 lVector2, Vector2 rVector2) =>
            lVector2.x * rVector2.y - lVector2.y * rVector2.x;
        
        /// 得到两条Vector2向量叉乘结果的正负
        public static int SignCross(Vector2 lVector2, Vector2 rVector2) => 
                Math.Sign(lVector2.x * rVector2.y - lVector2.y * rVector2.x);
    }
}