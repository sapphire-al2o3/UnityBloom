Shader "Custom/Bloom"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        CGINCLUDE


        #include "UnityCG.cginc"
        sampler2D _MainTex;
        float4 _MainTex_TexelSize;
        float _Threshold;
        float _Intensity;
        half2 _Offset;
        sampler2D _BlurTex;

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

        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = v.uv;
            return o;
        }
        ENDCG

        // 0: ダウンサンプリングと抽出パス
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            fixed4 frag(v2f i) : SV_Target
            {
                half2 o = _MainTex_TexelSize.xy;
                fixed4 col = tex2D(_MainTex, i.uv + o);
                col += tex2D(_MainTex, i.uv + half2(o.x, -o.y));
                col += tex2D(_MainTex, i.uv + half2(-o.x, o.y));
                col += tex2D(_MainTex, i.uv + -o);
                col *= 0.25;
                half y = dot(col.rgb, half3(0.299, 0.587, 0.144));
                half c = max(0.0, y - _Threshold);
                c /= max(1.0 - y, 0.0001);
                return col * c;
            }

            ENDCG
        }

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            fixed4 frag(v2f i) : SV_Target
            {
                half2 o = _MainTex_TexelSize.xy;
                fixed4 col = tex2D(_MainTex, i.uv + o);
                col += tex2D(_MainTex, i.uv + half2(o.x, -o.y));
                col += tex2D(_MainTex, i.uv + half2(-o.x, o.y));
                col += tex2D(_MainTex, i.uv + -o);
                col *= 0.25;
                return col;
            }

            ENDCG
        }

        // 1: Blur
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            fixed4 frag(v2f i) : SV_Target
            {
                half2 s = i.uv;
                s -= _Offset * 5.0;
                fixed4 col = tex2D(_MainTex, s);
                s += _Offset;
                col += tex2D(_MainTex, s);
                s += _Offset;
                col += tex2D(_MainTex, s) * 2.0;
                s += _Offset;
                col += tex2D(_MainTex, s) * 2.0;
                s += _Offset;
                col += tex2D(_MainTex, s) * 3.0;
                s += _Offset;
                col += tex2D(_MainTex, s);
                s += _Offset;
                col += tex2D(_MainTex, s);

                return col / 21.0;
            }

            ENDCG
        }

        // 合成
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb += tex2D(_BlurTex, i.uv).rgb * _Intensity;
                //return tex2D(_BlurTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
