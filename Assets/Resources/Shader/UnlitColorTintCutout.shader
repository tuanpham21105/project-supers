Shader "Custom/UnlitColorTintCutout"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture (tùy chọn)", 2D) = "white" {}

        [Enum(Opaque,0,Cutout,1,Transparent,2)] _Mode ("Rendering Mode", Float) = 0
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

        // Các property ẩn — được C# ShaderGUI tự động điều chỉnh theo _Mode
        [HideInInspector] _SrcBlend ("__src", Float) = 1
        [HideInInspector] _DstBlend ("__dst", Float) = 0
        [HideInInspector] _ZWrite ("__zw", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local _ALPHABLEND_ON
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                #if _ALPHATEST_ON
                    // Cutout — pixel dưới ngưỡng bị loại bỏ hoàn toàn (răng cưa, không mờ dần)
                    clip(col.a - _Cutoff);
                #endif

                return col;
            }
            ENDCG
        }
    }

    CustomEditor "UnlitColorTintCutoutGUI"
}
