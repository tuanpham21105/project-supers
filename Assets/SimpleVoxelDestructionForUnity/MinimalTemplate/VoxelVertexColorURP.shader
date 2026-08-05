Shader "Voxel/VertexColorLit"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _Ambient ("Ambient", Range(0,1)) = 0.25
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            fixed4 _BaseColor;
            float _Ambient;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                fixed4 color  : COLOR;
            };

            struct v2f
            {
                float4 pos         : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                fixed4 vcol        : COLOR;
                UNITY_LIGHTING_COORDS(2,3)
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.vcol = v.color * _BaseColor;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 N = normalize(i.worldNormal);
                float3 L = normalize(_WorldSpaceLightPos0.xyz);

                float ndotl = saturate(dot(N, L));

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                fixed3 light = _Ambient + (_LightColor0.rgb * ndotl) * atten;
                fixed3 col = i.vcol.rgb * light;

                return fixed4(col, 1);
            }
            ENDCG
        }
    }
}
