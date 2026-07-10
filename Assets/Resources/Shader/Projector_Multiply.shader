Shader "Projector/Multiply Atlas Custom" {
    Properties {
        _ShadowTex ("Cookie", 2D) = "gray" {}
        [NoScaleOffset] _FalloffTex ("FallOff", 2D) = "white" {}
        [Toggle(_)]
        _EnableFalloff("Enable Falloff?", Float) = 0.0
        [Toggle(_SKIP_BOUNDS_CHECK)]
        _SkipBoundsCheck("Skip Bounds Check?", Float) = 0.0
        [Toggle(_CLIPPING)]
        _EnableClipping("Enable Clipping?", Float) = 0.0
        [Toggle(_BACKFACE_CULLING)]
        _EnableBackfaceCulling("Enable Backface Culling?", Float) = 0.0
        _CullAngleLimit("Culling Angle Limit (rad)", Range(0, 3.15)) = 1.5708 // 90 degrees
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc("Blend Source Factor", Float) = 5 // SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendDest("Blend Destination Factor", Float) = 10 // OneMinusSrcAlpha
    }
    Subshader {
        Tags {"Queue"="Transparent"}
        Pass {
            ZWrite Off
            ColorMask RGB
            // Original: Blend DstColor Zero
            Blend [_BlendSrc] [_BlendDest]
            Offset -1, -1
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            #pragma shader_feature _SKIP_BOUNDS_CHECK
            #pragma shader_feature _CLIPPING
            #pragma shader_feature _SECOND_DIFFUSEMAP
            #pragma shader_feature _BACKFACE_CULLING
       
            struct VertexInput {
                float4 vertex : POSITION;
            #if defined(_BACKFACE_CULLING)
                fixed3 normal : NORMAL;
            #endif
            };

            struct v2f {
                float4 uvShadow : TEXCOORD0;
                float4 uvFalloff : TEXCOORD1;
            #if defined(_BACKFACE_CULLING)
                float projAngle : TEXCOORD2;
            #endif
                UNITY_FOG_COORDS(2)
                float4 pos : SV_POSITION;
            };
       
            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;

            float4 _ShadowTex_ST;
            sampler2D _ShadowTex;
            sampler2D _FalloffTex;
            float _CullAngleLimit;
       
            inline half angleBetween(half3 vector1, half3 vector2)
            {
                return acos(dot(vector1, vector2) / (length(vector1) * length(vector2)));
            }

            v2f vert (VertexInput v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uvShadow = mul (unity_Projector, v.vertex);
                o.uvFalloff = mul (unity_ProjectorClip, v.vertex);
            #if defined(_BACKFACE_CULLING)
                half3 projNormal = mul(unity_Projector, v.normal);
                o.projAngle = abs(angleBetween(half3(0,0,-1), projNormal));
            #endif
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
            #if defined(_BACKFACE_CULLING)
                if (i.projAngle >= _CullAngleLimit)
                {
                #if defined(_CLIPPING)
                    clip(-1.0);
                #endif
                    return fixed4(0,0,0,0);
                }
            #endif

                float2 texSUV = i.uvShadow.xy / i.uvShadow.w;
                fixed4 texS = tex2D (_ShadowTex, TRANSFORM_TEX(texSUV, _ShadowTex));

            #if defined(_SECOND_DIFFUSEMAP)
                float2 maskUV = 1.0 - abs(texSUV * 2.0 - 1.0);
                maskUV = saturate(maskUV / fwidth(texSUV * 2.0));
                texS.a *= maskUV.x * maskUV.y;
                texS.a = 1.0-texS.a;
                fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
                fixed4 res = lerp(fixed4(1,1,1,0), texS, texF.a);
            #else
                fixed4 res = texS;
            #endif
            #if !defined(_SKIP_BOUNDS_CHECK)
                // Clip pixels outside of the clippling plane: i.uvFalloff.z [-1(near), 1(far)]
                float2 boundsUVs = texSUV;
                if (abs(i.uvFalloff.z) > 1.0 || boundsUVs.x < 0.0 || boundsUVs.y < 0.0 || boundsUVs.x > 1.0 || boundsUVs.y > 1.0)
                {
                #if defined(_CLIPPING)
                    clip(-1.0);
                #endif
                    return fixed4(0,0,0,0);
                }
            #endif
                UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(1,1,1,1));            
                return res;
            }
            ENDCG
        }
    }
}