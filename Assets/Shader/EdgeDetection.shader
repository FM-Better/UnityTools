Shader "Unlit/EdgeDetection"
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
                half2 uv[9] : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half4 _MainTex_TexelSize;
            fixed _EdgeIntensity;
            fixed4 _EdgeColor;
            fixed4 _BackgroundColor;

            half Sobel(v2f i) // 采用Sobel算子
            {
                const half Gx[9] = {-1, -2, -1,
                                    0, 0, 0,
                                    1, 2, 1};
                const half Gy[9] = {-1, 0, 1,
                                    -2, 0, 2,
                                    -1, 0, 1};

                half texColor;
                half edgeX = 0;
                half edgeY = 0;
                for (int it = 0; it < 9; ++ it)
                {
                    texColor = luminance(tex2D(_MainTex, i.uv[it]).rgb);
                    edgeX += texColor * Gx[it];
                    edgeY += texColor * Gy[it];
                }

                return abs(edgeX) + abs(edgeY); // 减去x、y两个方向的卷积的绝对值 边缘的梯度绝对值较大
            }
            
            v2f vert (appdata_img v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                half2 uv = v.texcoord;
                o.uv[0] = uv + _MainTex_TexelSize * half2(-1, -1);
                o.uv[1] = uv + _MainTex_TexelSize * half2(-1, 0);
                o.uv[2] = uv + _MainTex_TexelSize * half2(-1, 1);
                o.uv[3] = uv + _MainTex_TexelSize * half2(0, -1);
                o.uv[4] = uv + _MainTex_TexelSize * half2(0, 0);
                o.uv[5] = uv + _MainTex_TexelSize * half2(0, 1);
                o.uv[6] = uv + _MainTex_TexelSize * half2(1, -1);
                o.uv[7] = uv + _MainTex_TexelSize * half2(1, 0);
                o.uv[8] = uv + _MainTex_TexelSize * half2(1, 1);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half egde = Sobel(i); // 值越大 越可能为边缘点
                fixed4 originColor = tex2D(_MainTex, i.uv[4]);
                fixed4 withEdgeColor = lerp(originColor, _EdgeColor, egde);
                fixed4 onlyEdgeColor = lerp(_BackgroundColor, _EdgeColor, egde);
                return lerp(withEdgeColor, onlyEdgeColor, _EdgeIntensity);
            }
            ENDCG
        }
    }
}
