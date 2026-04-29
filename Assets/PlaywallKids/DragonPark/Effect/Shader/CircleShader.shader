// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CircleShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "black" {}
		_Strength("Strength of shadow", range(0.0,1.0)) = 0.5
	}
	SubShader
	{
		// No culling or depth
		Cull Back ZWrite On ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha

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
			fixed _Strength;
			fixed4 frag (v2f i) : SV_Target
			{
				fixed w = 0.5 - i.uv.x;
				fixed h = 0.5 - i.uv.y;

				if( w * w  + h * h < 0.5 * 0.5 )
					return float4(0,0,0,_Strength);
				clip(-1);
				return float4(0,0,0,0.0);

			}
			ENDCG
		}
	}
}
