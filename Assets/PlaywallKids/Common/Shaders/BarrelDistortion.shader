Shader "Custom/BarrelDistortion" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Strength ("Strength", Range(0.0, 1.0)) = 0.5
		_FOV ("FOV", Range (0.0, 180.0)) = 60.0
		_AspectRatio ("Aspect Ratio", Range (0.1, 3.0)) = 1.777
		_CylindricalRatio ("Cylindrical Ratio", Range (0.0, 4.0)) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass {
			Cull Back
			Lighting Off
		
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			uniform sampler2D _MainTex;
	    
			uniform float _Strength;
			uniform float _FOV;
			uniform float _AspectRatio;
			uniform float _CylindricalRatio;

			struct v2f 
			{
				float4 pos : POSITION;
				float3 uv : TEXCOORD0;
				float2 dot : TEXCOORD1;
			};

			v2f vert(appdata_base v) 
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);

				float3 uv = v.texcoord.xyz;
				float height = tan(_FOV * 3.14159264 / 360.0);
			
				float scaledHeight = _Strength * height;
				float cylAspectRatio = _AspectRatio * _CylindricalRatio;
				float aspectDiagSq = _AspectRatio * _AspectRatio + 1.0;
				float diagSq = scaledHeight * scaledHeight * aspectDiagSq;
				float2 signedUV = (2.0 * uv + float2(-1.0, -1.0));

				float z = 0.5 * sqrt(diagSq + 1.0) + 0.5;
				float ny = (z - 1.0) / (cylAspectRatio * cylAspectRatio + 1.0);
				
				o.dot = sqrt(ny) * float2(cylAspectRatio, 1.0) * signedUV;
				o.uv = float3(0.5, 0.5, 1.0) * z + float3(-0.5, -0.5, 0.0);
				o.uv.xy += uv.xy;

				return o;
			}

			float4 frag(v2f i) :COLOR 
			{
				float3 uv = dot(i.dot, i.dot) * float3(-0.5, -0.5, -1.0) + i.uv;
				float4 outColor = tex2Dproj(_MainTex, float4(uv, uv.z));
				return outColor;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
