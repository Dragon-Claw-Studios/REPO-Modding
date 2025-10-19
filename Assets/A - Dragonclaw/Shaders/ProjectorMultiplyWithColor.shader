// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/MultiplyWithColor"
{
    Properties
    {
        _ShadowTex ("Cookie (grayscale)", 2D) = "gray" {}
        _FalloffTex ("Falloff", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (1,0,0,1) // default red
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
                UNITY_FOG_COORDS(2)
                float4 pos : SV_POSITION;
            };

            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;

            sampler2D _ShadowTex;
            sampler2D _FalloffTex;
            fixed4 _TintColor;

            v2f vert(float4 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.uvShadow = mul(unity_Projector, vertex);
                o.uvFalloff = mul(unity_ProjectorClip, vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

fixed4 frag(v2f i) : SV_Target
{
    // Sample cookie (grayscale mask)
    fixed4 texS = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
    fixed mask = 1.0 - texS.r;

    // Sample falloff
    fixed4 texF = tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));

    // Apply tint as a multiplier to the original multiply color
    fixed3 multiplyResult = lerp(fixed3(1,1,1), texS.rgb, texF.a); // original multiply

    // Apply tint only where cookie mask is active
    fixed3 res = lerp(multiplyResult, _TintColor.rgb, mask * texF.a);

				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(1,1,1,1));
    return fixed4(res,1);
}

            ENDCG
        }
    }
}
