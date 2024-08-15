using UnityEngine;

namespace Utility
{
    public static class ColorUtil
    {
        /// <summary>
        /// 返回16进制字符串对应的颜色
        /// </summary>
        /// <param name="hexColor"> 颜色对应的16进制字符串 </param>
        /// <returns></returns>
        public static Color HexToColor(string hexColor)
        {
            ColorUtility.TryParseHtmlString(hexColor, out var color);
            return color;
        }
        
        /// <summary>
        /// 双线性插值 (利用归一化坐标采样贴图对应颜色 x、y：0~1
        /// </summary>
        /// <param name="x"> 对应贴图的横坐标值 </param>
        /// <param name="y"> 对应贴图的纵坐标值 </param>
        /// <param name="dstTexture"> 采样的目标贴图 </param>
        /// <returns></returns>
        private static Color BilinearInterpolation(float x, float y, Texture2D dstTexture)
        {
            var x1 = Mathf.Clamp(Mathf.FloorToInt(x * dstTexture.width), 0, dstTexture.width - 1);
            var x2 = Mathf.Clamp(Mathf.CeilToInt(x * dstTexture.width), 0, dstTexture.width - 1);
            var y1 = Mathf.Clamp(Mathf.FloorToInt(y * dstTexture.height), 0, dstTexture.height - 1);
            var y2 = Mathf.Clamp(Mathf.CeilToInt(y * dstTexture.height), 0, dstTexture.height - 1);

            var c00 = dstTexture.GetPixel(x1, y1);
            var c01 = dstTexture.GetPixel(x1, y2);
            var c10 = dstTexture.GetPixel(x2, y1);
            var c11 = dstTexture.GetPixel(x2, y2);

            var s = x - x1;
            var t = y - y1;

            var top = Color.Lerp(c00, c10, s);
            var bottom = Color.Lerp(c01, c11, s);

            return Color.Lerp(top, bottom, t);
        }
    }
}