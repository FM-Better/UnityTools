using UnityEngine;

namespace Utility
{
    public static class ColorUtil
    {
        /// <summary>
        /// 返回16进制字符串对应的颜色
        /// </summary>
        /// <param name="hexColor"> 颜色对应的16进制字符串 </param>
        /// <returns> 对应的颜色 </returns>
        public static Color HexToColor(string hexColor)
        {
            ColorUtility.TryParseHtmlString(hexColor, out var color);
            return color;
        }

        /// <summary>
        /// 返回颜色对应大写的16进制字符串
        /// </summary>
        /// <param name="color"> 16进制字符串对应的颜色 </param>
        /// <returns> 对应的16进制字符串 </returns>
        public static string ColorToHex(Color color) => $"#{ColorUtility.ToHtmlStringRGB(color)}";

        /// <summary>
        /// 双线性插值 (利用uv坐标采样颜色
        /// </summary>
        /// <param name="u"> 贴图u坐标 </param>
        /// <param name="v"> 贴图v坐标 </param>
        /// <param name="width"> 贴图宽度 </param>
        /// <param name="height"> 贴图高度 </param>
        /// <param name="pixels"> 采样的像素数组 </param>
        /// <returns> 插值得到的颜色 </returns>
        private static Color BilinearInterpolation(float u, float v, int width, int height, Color[] pixels)
        {
            var x1 = Mathf.Clamp(Mathf.FloorToInt(u * width), 0, width - 1);
            var x2 = Mathf.Clamp(Mathf.CeilToInt(u * width), 0, width - 1);
            var y1 = Mathf.Clamp(Mathf.FloorToInt(v * height), 0, height - 1);
            var y2 = Mathf.Clamp(Mathf.CeilToInt(v * height), 0, height - 1);

            var c00 = pixels[y1 * width + x1];
            var c01 = pixels[y2 * width + x1];
            var c10 = pixels[y1 * width + x2];
            var c11 = pixels[y2 * width + x2];

            var s = u - x1;
            var t = v - y1;

            var top = Color.Lerp(c01, c11, s);
            var bottom = Color.Lerp(c00, c10, s);

            return Color.Lerp(top, bottom, t);
        }

        /// <summary>
        /// 创建纯色纹理
        /// </summary>
        /// <param name="width"> 纹理宽度 </param>
        /// <param name="height"> 纹理高度 </param>
        /// <param name="color"> 纹理颜色 </param>
        /// <returns> 创建出的纹理 </returns>
        public static Texture2D CreatePureColorTexture2D(int width, int height, Color color)
        {
            var pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }
            
            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            
            return result;
        }
    }
}