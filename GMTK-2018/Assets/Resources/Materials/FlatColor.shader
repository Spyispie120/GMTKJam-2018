Shader "Custom/FlatColor" {
    Properties{
        _Color("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex("Texture", 2D) = "white" { }
    }
        SubShader{
           Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
           Blend SrcAlpha OneMinusSrcAlpha
           Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Color;
            sampler2D _MainTex;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _MainTex_ST;

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texcol = tex2D(_MainTex, i.uv);
                if (texcol.r == 0.0 && texcol.g == 0.0 && texcol.b == 0.0) return (1.0, 1.0, 1.0, 0.0);
                return texcol * _Color;
                //return texcol * _Color;
            }

            ENDCG

            }
    }

    Fallback "Diffuse"
}