using System;
using UnityEngine;
using Utility.Attribute;

namespace PostEffect
{
    public class EdgeDetection : PostEffectBase
    {
        [SerializeField] private Shader _effectShader;
        [CustomHeader("参数相关", 0, 1, 1, alignment: TextAnchor.MiddleCenter)]
        [Range(0f, 1f)]
        [SerializeField] private float _edgeIntensity;
        [SerializeField] private Color _edgeColor = Color.black;
        [SerializeField] private Color _bgColor = Color.white;
        
        private Material _edgeMaterial;

        public Material EffectMaterial
        {
            get
            {
                _edgeMaterial = CheckShaderAndCreateMaterial(_effectShader, _edgeMaterial);
                return _edgeMaterial;
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (EffectMaterial)
            {
                _edgeMaterial.SetFloat("_EdgeIntensity", _edgeIntensity);
                _edgeMaterial.SetColor("_EdgeColor", _edgeColor);
                _edgeMaterial.SetColor("_BackgroundColor", _bgColor);
                Graphics.Blit(source, destination, _edgeMaterial);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
    }
}
