Shader "Custom/StarStripeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _StripeDensity ("Stripe Density", Float) = 10.0
        _Speed ("Stripe Speed", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _StripeDensity;
            float _Speed;
            float _StripeTime;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 计算距离中心的距离（中心在 (0.5, 0.5)）
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);

                // 让条纹随时间向内收缩（偏移量随时间增加）
                float phase = dist * _StripeDensity + _Time * _Speed;

                // 生成黑白条纹：小数部分决定黑白
                float stripe = frac(phase);
                fixed4 col = (stripe < 0.5) ? fixed4(0, 0, 0, 1) : fixed4(1, 1, 1, 1);

                return col;
            }
            ENDCG
        }
    }
}
