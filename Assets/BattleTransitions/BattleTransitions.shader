// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/BattleTransitions"
{
	Properties
	{
		_TransitionTex("Transition Texture", 2D) = "white" {}
		_Color("Screen Color", Color) = (1,1,1,1)
		_Cutoff("Cutoff", Range(0, 1)) = 0
		[MaterialToggle] _Distort("Distort", Float) = 0
		_Fade("Fade", Range(0, 1)) = 0
	}

		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always
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
					float2 uv1 : TEXCOORD1;
					float4 vertex : SV_POSITION;
				};


				v2f simplevert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.uv1 = v.uv;

					return o;
				}

				sampler2D _TransitionTex;
				int _Distort;
				float _Fade;

				float _Cutoff;
				fixed4 _Color;

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 transit = tex2D(_TransitionTex, i.uv1);

					fixed2 direction = float2(0,0);
					if(_Distort)
						direction = normalize(float2((transit.r - 0.5) * 2, (transit.g - 0.5) * 2));
					fixed4 col = _Color;
					if (transit.b < _Cutoff) {
						col.a = 1;
					}
					else {
						col.a = 0;
					}
					return col;
				}					
				ENDCG
			}
		}
}
