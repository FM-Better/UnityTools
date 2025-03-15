Shader "Unlit/BrightnessSaturationAndContrast"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "CalculateUtil.cginc"

            struct v2f
            {
                half2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half _Brightness;
            half _Saturation;
            half _Contrast;

            v2f vert (appdata_img v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 originColor = tex2D(_MainTex, i.uv);
                fixed3 finalColor = originColor.rgb * _Brightness; // 亮度：线性增大

                fixed curLuminance = luminance(originColor);
                fixed3 luminanceColor = fixed3(curLuminance, curLuminance, curLuminance);
                finalColor = lerp(luminanceColor, finalColor, _Saturation); // 饱和度：灰度值越少越饱和

                fixed3 avgColor = fixed3(0.5f, 0.5f, 0.5f);
                finalColor = lerp(avgColor, finalColor, _Contrast); // 对比度：亮的更亮，暗的更暗 中间色作为参考
                
                return fixed4(finalColor, originColor.a);
            }
            ENDCG
        }
    }
}
