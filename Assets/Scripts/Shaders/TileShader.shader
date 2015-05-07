Shader "Sprites/Custom/TileShader"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldpos : TEXCOORD1;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;

				OUT.worldpos = mul(_Object2World, IN.vertex);

				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;

				//float lines = 4;
				//float actualSize = 1.0 / 6.0;
				//float xcoord = fmod(IN.texcoord.x, actualSize);
				//if (fmod(xcoord, actualSize / lines) < (actualSize / lines) / 5)
				//{
				//	c *= 0.8;
				//}
				float sublength = 0.3;
				float time = _Time.x*5;
				float xcoord = IN.worldpos.x + time;
				float ycoord = IN.worldpos.y + time;
				float decay = max(IN.color.r, max(IN.color.g, IN.color.b));
				//float sublength = 1.0 / lines;
				float inner = fmod(ycoord, sublength);// , sublength);
				float wave = sublength * 0.2 * ((0.5 + decay) * 2) * sin(xcoord*10+time *10) + (sublength * 0.5);
				bool check = inner < wave;

				if (fmod(ycoord, sublength * 2) > sublength)
				{
					check = !check;
				}

				//if (check)
				//{
				//	c.rgb *= 0.7;
				//}
				//
				//if (check)
				//{
				//	float mult = 0.2;
				//	int i = 0;
				//	for (; i < 3; i++)
				//	{
				//		//swizzledexing
				//		c.rgb[i] = (1.0 - c.rgb[i]) * mult + c.rgb[i];
				//	}
				//}
				//else
				//{
				//	//c = float4(1, 0, 0, 1);
				//}


				////
				float a = 1.0, b = 1.0;
				if (check)
				{
					a = 0.0;
					b = -1.0;
				}
				float mult = 0.15;
				int i = 0;
				for (; i < 3; i++)
				{
					//swizzledexing
					c.rgb[i] = (1.0 * a - c.rgb[i] * b) * mult * b + c.rgb[i];
				}
				////


				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
