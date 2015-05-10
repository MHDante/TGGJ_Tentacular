Shader "Custom/TileShader"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_LastColor ("LastColor", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_UDLR ("UpDownLeftRight", Vector) = (-1.0,-1.0,-1.0,-1.0)
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
				fixed4 color    : COLOR0;
				half2 texcoord  : TEXCOORD0;
				float4 worldpos : TEXCOORD1;
				fixed4 udlr	    : COLOR1;
				fixed4 lastcol  : COLOR2;

			};
			
			fixed4 _LastColor;
			fixed4 _Color;
			fixed4 _UDLR;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;

				OUT.worldpos = mul(_Object2World, IN.vertex);
				OUT.lastcol = _LastColor;
				OUT.udlr = _UDLR;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;

			bool withinDist(float2 a, float2 b, float dist) {
				return ((a.x - b.x)*(a.x - b.x) + (a.y - b.y)*(a.y - b.y)) < (dist * dist);
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord);// *IN.color;
				
				float2 relCoord = float2(fmod(IN.texcoord.x, (1.0 / 6.0)) / (1.0 / 6.0), IN.texcoord.y);
				//float relcoord = float2(fmod(IN.worldpos.x, 1.0).fmod(IN.worldpos., 1.0))
				bool none = ((IN.udlr.r < -1.0) && (IN.udlr.g < -1.0) && (IN.udlr.b < -1.0) && (IN.udlr.a < -1.0) && withinDist(relCoord, float2(0.5, 0.5), IN.udlr.r + 2));
				//bool none = false;
				bool north = ((IN.udlr.r >= 0) && withinDist(relCoord, float2(0.5, 1), IN.udlr.r));
				bool south = ((IN.udlr.g >= 0) && withinDist(relCoord, float2(0.5, 0), IN.udlr.g));
				bool east = ((IN.udlr.b >= 0) && withinDist(relCoord, float2(1, 0.5), IN.udlr.b));
				bool west = ((IN.udlr.a >= 0) && withinDist(relCoord, float2(0, 0.5), IN.udlr.a));

				if(none
					|| north
					|| south
					|| east
					|| west) {
					c *= IN.color;
				}
				else
				{
					c *= IN.lastcol;
				}
				//fixed4 c = float4(IN.lastcol.r, IN.lastcol.g, IN.lastcol.b, m.a);
				
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
