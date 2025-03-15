using UnityEngine;
using Utility.Attribute;

namespace PostEffect
{
    public class BrightnessSaturationAndContrast : PostEffectBase
    {
        [SerializeField] private Shader _effectShader;
        [CustomHeader("参数相关", 0, 1,1, alignment: TextAnchor.MiddleCenter)]
        [Range(0f, 3f)]
        [SerializeField] private float _brightness = 1f;
        [Range(0f, 3f)]
        [SerializeField] private float _saturation = 1f;
        [Range(0f, 3f)]
        [SerializeField] private float _contrast = 1f;
        
        private Material _briSatConMaterial;

        public Material EffectMaterial
        {
            get
            {
                _briSatConMaterial = CheckShaderAndCreateMaterial(_effectShader, _briSatConMaterial);
                return _briSatConMaterial;
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (EffectMaterial)
            {
                _briSatConMaterial.SetFloat("_Brightness", _brightness);
                _briSatConMaterial.SetFloat("_Saturation", _saturation);
                _briSatConMaterial.SetFloat("_Contrast", _contrast);
                Graphics.Blit(source, destination, _briSatConMaterial);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
    }
}
