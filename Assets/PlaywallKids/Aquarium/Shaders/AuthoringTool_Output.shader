Shader "Custom/Aquarium/AuthoringTool_Output"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "clear" {}
		_TemplateTex ("Template Texture", 2D) = "black" {}
		_PatternTex ("Pattern Texture", 2D) = "black" {}
		_EffectTex ("Effect Texture", 2D) = "black" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		/* Pass 0 : Cut the canvas texture into the template shapes and attach patterns and effects. */
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			sampler2D _TemplateTex;
			sampler2D _PatternTex;
			sampler2D _EffectTex;

			half4 frag (v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);
				half4 templateCol = tex2D(_TemplateTex, i.uv);
				half4 patternCol = tex2D(_PatternTex, i.uv);
				half4 effectCol = tex2D(_EffectTex, i.uv);

				half4 outCol = half4(0, 0, 0, 0);
				outCol.rgb = templateCol.rgb * (1.0 - col.a) + col.rgb * (templateCol.a * col.a);
				outCol.a = templateCol.a;

				outCol.rgb = outCol.rgb * (1.0 - patternCol.a) + patternCol.rgb * patternCol.a;
				outCol.rgb = outCol.rgb * (1.0 - effectCol.a) + effectCol.rgb * effectCol.a;
				return outCol;
			}
			ENDCG
		}

		/* Pass 1 : Accumulates each template shape into the final texture. */
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

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

			sampler2D _MainTex;

			half4 frag(v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
