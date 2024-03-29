Shader "Unlit/Scroll"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BGOffset("_BGOffset", Vector) = (1, 1, 1, 0)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					fixed4 color : COLOR;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _BGOffset;

				v2f vert(appdata v)
				{
					v2f o;
					o.color = v.color;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					half4 sub;
					half2 uv = i.uv;
					half tx = _BGOffset.x;
					half ty = _BGOffset.y;
					uv.x = i.uv.x + tx;
					uv.y = i.uv.y + ty;
					fixed4 col = tex2D(_MainTex, uv);

					return col;
				}
				ENDCG
			}
		}
}