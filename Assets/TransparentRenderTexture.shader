Shader "Custom/TransparentRenderTexture"{
    Properties {
        _Color ("Color", Color) = (1,1,1,1) // Solid Color
        _Alpha ("Alpha", Range(0, 1)) = 1.0 // 透明度
        _MainTex ("Texture", 2D) = "white" {} // RenderTexture
    }
    SubShader {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color; // Solid Color
            sampler2D _MainTex; // RenderTexture
            float _Alpha; // 透明度

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // RenderTextureからの色
                fixed4 texColor = tex2D(_MainTex, i.uv);
                
                // Solid ColorとRenderTextureの色を合成
                fixed4 finalColor = texColor * _Color;

                // 透明度を設定
                finalColor.a *= _Alpha; // 透明度を操作

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}