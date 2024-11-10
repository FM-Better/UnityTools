using UnityEditor;

namespace EditorTool
{
    public class AssetPreprocessor : AssetPostprocessor
    {
        #region 图片文件

        // private void OnPreprocessTexture()
        // {
        //     if (assetPath.StartsWith("Assets/"))
        //     {
        //         var textureImporter = assetImporter as TextureImporter;
        //         if (textureImporter)
        //         {
        //             Debug.Log(assetPath);
        //             
        //             textureImporter.textureType = TextureImporterType.Sprite;
        //             textureImporter.spriteImportMode = SpriteImportMode.Single;
        //             textureImporter.spritePixelsPerUnit = 100f;
        //             textureImporter.spritePivot = new Vector2(0.5f, 0.5f);
        //
        //             textureImporter.alphaIsTransparency = true;
        //             textureImporter.mipmapEnabled = false;
        //             textureImporter.isReadable = false;
        //             textureImporter.wrapMode = TextureWrapMode.Clamp;
        //             textureImporter.filterMode = FilterMode.Bilinear;
        //             textureImporter.textureCompression = TextureImporterCompression.Compressed; // 使用压缩
        //             textureImporter.compressionQuality = 100;
        //             
        //             textureImporter.SaveAndReimport();
        //         }
        //     }
        // }

        #endregion
    }
}
