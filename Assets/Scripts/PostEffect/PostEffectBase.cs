using UnityEngine;
using Utility;

namespace PostEffect
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class PostEffectBase : MonoBehaviour
    {
        /// <summary>
        /// 检查资源是否满足
        /// </summary>
        protected void CheckResources()
        {
            var isSupported = CheckSupport();
            if (!isSupported)
            {
                NotSupported();
            }
        }

        /// <summary>
        /// 检查是否支持
        /// </summary>
        /// <returns></returns>
        protected bool CheckSupport()
        {
            if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Default))
            {
                DebugUtil.LogWarning("Don't support RenderTextureFormat.Default");
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// 不支持屏幕后处理的操作
        /// </summary>
        protected void NotSupported() => enabled = false;

        protected void Awake() => CheckResources();

        /// <summary>
        /// 检查Shader并且创建对应的临时材质
        /// </summary>
        /// <param name="shader"> 指定后处理shader </param>
        /// <param name="material"> 后处理的材质 </param>
        /// <returns> 创建后的后处理材质 </returns>
        protected Material CheckShaderAndCreateMaterial(Shader shader, Material material)
        {
            if (!shader || !shader.isSupported)
                return null;
            
            if (shader.isSupported && material && material.shader == shader)
                return material;
            
            material = new Material(shader);
            material.hideFlags = HideFlags.HideAndDontSave;
            return material;
        }
    }
}
