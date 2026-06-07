// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'
Shader "Projector/Multiply" {
    Properties {
        _ShadowTex ("Cookie", 2D) = "gray" {}
        _FalloffTex ("FallOff", 2D) = "white" {}
        _BoundsExtent ("Bounds Half-Extent (XZ)", Vector) = (7.5,0,7.5,0)
        _OffsetX ("Bounds Offset X", Float) = 0
        _OffsetZ ("Bounds Offset Z", Float) = 0
    }
    Subshader {
        Tags {"Queue"="Transparent"}
        Pass {
            ZWrite Off
            ColorMask RGB
            Blend DstColor Zero
            Offset -1, -1
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            struct v2f {
                float4 uvShadow : TEXCOORD0;
                float4 uvFalloff : TEXCOORD1;
                float3 worldPos : TEXCOORD3;
                UNITY_FOG_COORDS(2)
                float4 pos : SV_POSITION;
            };
            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;
            float4 _BoundsExtent;
            float _OffsetX;
            float _OffsetZ;

            v2f vert (float4 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.uvShadow = mul(unity_Projector, vertex);
                o.uvFalloff = mul(unity_ProjectorClip, vertex);
                o.worldPos = mul(unity_ObjectToWorld, vertex).xyz;
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            sampler2D _ShadowTex;
            sampler2D _FalloffTex;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 offset = abs(i.worldPos.xz - float2(_OffsetX, _OffsetZ));
                clip(_BoundsExtent.xz - offset);

                fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
                texS.a = 1.0 - texS.a;
                fixed4 texF = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
                fixed4 res = lerp(fixed4(1,1,1,0), texS, texF.a);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(0,0,0,0));
                return res;
            }
            ENDCG
        }
    }
}