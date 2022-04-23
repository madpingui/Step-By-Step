// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/AlphaDependingDistance"
{
	Properties
	{
		 _Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_Radius("Radius", Range(0.001, 500)) = 10
	}
		SubShader
		{
			Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }

			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				
			#include "UnityLightingCommon.cginc" // for _LightColor0

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 worldPos : TEXCOORD1;
					fixed4 diff : COLOR0; // diffuse lighting color
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Color;
				v2f vert(appdata_base v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					half3 worldNormal = UnityObjectToWorldNormal(v.normal);
					half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
					o.diff = nl * _LightColor0;
					return o;
				}

				float _Radius;

				fixed4 frag(v2f i) : SV_Target {
				
					float dist = distance(i.worldPos, _WorldSpaceCameraPos) -5;
				fixed4 col = _Color * (_Color * (1 - saturate(dist / _Radius)));
					
					col *= i.diff +0.5f;
					col.a = 1;
					return col;
				}

				ENDCG
			}
		}
}