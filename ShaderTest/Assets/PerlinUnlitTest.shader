Shader "Custom/Noise/PerlinUnlitTest"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Amplitude("Amplitude", float) = 0.5
		_Frequency("Frequency", float) = 1
		_Persistence("Persistence", float) = 0.5
		_Lacunarity("Lacunarity", float) = 2
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"
				//"DisableBatching"="true"
		}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "PerlinNoise.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			uint _HashSize;

			float _Amplitude;
			float _Frequency;
			float _Persistence;
			float _Lacunarity;
			
			v2f vert (appdata v)
			{
				v2f o;

				float pnoise = 0;
				float w = _Amplitude;
				float s = _Frequency;
				for (int it = 0; it < 6; it++) {
					pnoise += perlinNoise2D(v.vertex.xz*s, 0, _HashSize)*w;
					w *= _Persistence;
					s *= _Lacunarity;
				}
				v.vertex.xyz += v.normal*pnoise;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);
				float pnoise = 0;
				float w = _Amplitude;
				float s = _Frequency*30;
				for (int it = 0; it < 6; it++) {
					pnoise += perlinNoise2D(i.uv*s, 0, _HashSize)*w;
					w *= _Persistence;
					s *= _Lacunarity;
				}
				float val = pnoise;
				fixed4 col = float4(val, val, val, 1)

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
