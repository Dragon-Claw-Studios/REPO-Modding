// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'
Shader "Projector/MultiplyWithColor"
{
    Properties
    {
        _ShadowTex ("Cookie (grayscale)", 2D) = "gray" {}
        _FalloffTex ("Falloff", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,0,0,1)
        _BoundsExtent ("Bounds Half-Extent (XZ)", Vector) = (7.5,0,7.5,0)
        _OffsetX ("Bounds Offset X", Float) = 0
        _OffsetZ ("Bounds Offset Z", Float) = 0
    }
    Subshader
    {
        Tags { "Queue"="Transparent" }
        Pass
        {
            ZWrite Off
            ColorMask RGB
            Blend DstColor Zero
            Offset -1, -1
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            struct v2f
            {
                float4 uvShadow : TEXCOORD0;
                float4 uvFalloff : TEXCOORD1;
                float3 worldPos : TEXCOORD3;
                UNITY_FOG_COORDS(2)
                float4 pos : SV_POSITION;
            };
            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;
            sampler2D _ShadowTex;
            sampler2D _FalloffTex;
            fixed4 _TintColor;
            float4 _BoundsExtent;
            float _OffsetX;
            float _OffsetZ;

            v2f vert(float4 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.uvShadow = mul(unity_Projector, vertex);
                o.uvFalloff = mul(unity_ProjectorClip, vertex);
                o.worldPos = mul(unity_ObjectToWorld, vertex).xyz;
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 offset = abs(i.worldPos.xz - float2(_OffsetX, _OffsetZ));
                clip(_BoundsExtent.xz - offset);

                fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
                fixed mask = 1.0 - texS.r;
                fixed4 texF = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
                fixed3 multiplyResult = lerp(fixed3(1,1,1), texS.rgb, texF.a);
                fixed3 res = lerp(multiplyResult, _TintColor.rgb, mask * texF.a);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(0,0,0,0));
                return fixed4(res,1);
            }
            ENDCG
        }
    }
}