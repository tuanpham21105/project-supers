Shader "Custom/Billboard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        [Toggle] _LockYAxis ("Lock Y-Axis (chỉ xoay quanh trục Y)", Float) = 0
        [Toggle] _Cutout ("Alpha Cutoff (thay vì Blend)", Float) = 0
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

        [Toggle] _ConstantScreenSize ("Giữ kích thước cố định trên màn hình", Float) = 0
        _FixedSize ("Fixed Size (world units tại distance = 1)", Range(0.001, 1)) = 0.05
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off
        ZWrite Off
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _LOCKYAXIS_ON
            #pragma shader_feature _CUTOUT_ON
            #pragma shader_feature _CONSTANTSCREENSIZE_ON
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
            float _FixedSize;

            v2f vert(appdata v)
            {
                v2f o;

                // Vị trí gốc object trong world space (pivot của object)
                float3 worldPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;

                // Vector từ object tới camera
                float3 camRight = UNITY_MATRIX_V[0].xyz; // trục X của camera trong world space
                float3 camUp    = UNITY_MATRIX_V[1].xyz; // trục Y của camera trong world space

                #if _LOCKYAXIS_ON
                    // Chỉ xoay quanh trục Y — object luôn đứng thẳng (dùng cho billboard nhân vật/cây cối)
                    camRight = normalize(float3(camRight.x, 0, camRight.z));
                    camUp = float3(0, 1, 0);
                #endif

                // Lấy scale gốc của object để giữ đúng kích thước
                float3 scale = float3(
                    length(unity_ObjectToWorld._m00_m10_m20),
                    length(unity_ObjectToWorld._m01_m11_m21),
                    length(unity_ObjectToWorld._m02_m12_m22)
                );

                #if _CONSTANTSCREENSIZE_ON
                    // Bù trừ theo khoảng cách tới camera để giữ kích thước cố định trên màn hình
                    float distToCam = distance(_WorldSpaceCameraPos, worldPos);
                    float screenScale = distToCam * _FixedSize;
                    scale = scale * screenScale;
                #endif

                // Vị trí vertex cục bộ (dùng x,y của mesh làm offset theo mặt phẳng camera)
                float3 vertexOffset = camRight * v.vertex.x * scale.x
                                     + camUp    * v.vertex.y * scale.y;

                float3 finalWorldPos = worldPos + vertexOffset;

                o.vertex = mul(UNITY_MATRIX_VP, float4(finalWorldPos, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                #if _CUTOUT_ON
                    clip(col.a - _Cutoff);
                #endif

                return col;
            }
            ENDCG
        }
    }
}
