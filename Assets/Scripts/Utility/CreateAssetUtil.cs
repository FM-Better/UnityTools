using System.IO;
using UnityEditor;
using UnityEngine;

namespace Utility
{
    public static class CreateAssetUtil
    {
#if UNITY_EDITOR
        
        /// <summary>
        /// 创建Texture2D
        /// </summary>
        /// <param name="texturePixels"> 对应的像素 </param>
        /// <param name="textureWidth"> 宽 </param>
        /// <param name="textureHeight"> 高 </param>
        /// <param name="savePath"> 要保存的路径 </param>
        private static void CreateTexture2D(Color[] texturePixels, int textureWidth, int textureHeight, string savePath)
        {
            var dstTexture2D = new Texture2D(textureWidth, textureHeight); // 创建贴图
            dstTexture2D.SetPixels(texturePixels); // 设置像素
            dstTexture2D.Apply(); // 应用
                    
            var textureBytes = dstTexture2D.EncodeToPNG(); // 将Texture2D保存为PNG文件
            if (savePath.Length != 0)
            {
                File.WriteAllBytes(savePath, textureBytes); // 保存创建的贴图到路径
                AssetDatabase.Refresh();

                var texturePath = savePath.Substring(savePath.IndexOf("Assets")); // 获取Aseets下的相对路径 否则获取失败
                var importer = AssetImporter.GetAtPath(texturePath);
                var textureImporter = importer as TextureImporter;

                if (textureImporter) //获取Texture2D 进行设置
                {
                    textureImporter.textureType = TextureImporterType.Default; // 设置为Default
                    textureImporter.alphaIsTransparency = true; // 将 Alpha 通道视为透明度
                    // textureImporter.isReadable = true; // 设置为可读写
                    textureImporter.mipmapEnabled = false; // 禁用mipmap
                    textureImporter.wrapMode = TextureWrapMode.Clamp;
                    textureImporter.filterMode = FilterMode.Bilinear;
                    textureImporter.textureCompression = TextureImporterCompression.Compressed; // 使用压缩
                    textureImporter.compressionQuality = 100;

                    AssetDatabase.ImportAsset(texturePath);
                }
            }
                
            Object.DestroyImmediate(dstTexture2D); // 删除创建的贴图 释放内存
        }
        
#endif
    }
}