Shader "Custom/OctopusShader"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_FishTex("FishTex", 2D) = "white" {}
		_FishTexLen("FishTexLen", Int) = 0
		_Percent("Percent", Float) = 0
	}

	SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
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
						fixed4 color : COLOR;
						half2 texcoord  : TEXCOORD0;
						float4 worldpos : TEXCOORD1;
					};

					fixed4 _Color;

					v2f vert(appdata_t IN)
					{
						v2f OUT;
						OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
						OUT.texcoord = IN.texcoord;
						OUT.worldpos = mul(_Object2World, IN.vertex);
						OUT.color = IN.color * _Color;
#ifdef PIXELSNAP_ON
						OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

						return OUT;
					}

					sampler2D _MainTex;
					sampler2D _FishTex;
					int _FishTexLen;
					float _Percent;

					bool withinDist(float2 a, float2 b, float dist) {
						return ((a.x - b.x)*(a.x - b.x) + (a.y - b.y)*(a.y - b.y)) < (dist * dist);
					}

					fixed4 frag(v2f IN) : SV_Target
					{
						fixed4 c = tex2D(_MainTex, IN.texcoord);

						float barwidth = 0.1, barheight = 0.8;
						float barpad = (1 - barheight) / 2.0;
						bool bar = IN.texcoord.x > 1 - barpad - barwidth 
								&& IN.texcoord.x < 1 - barpad
								&& IN.texcoord.y > barpad
								&& IN.texcoord.y < 1 - barpad;


						float seg = 1.0 / _FishTexLen;
						float yy = (IN.texcoord.y - barpad) / barheight;
						int index = floor(yy / seg);
						if (bar)
						{
							float rad = 0.05;
							//half2 point = 
							if (IN.texcoord.y > 1 - barpad - rad)
							{

							}
							half2 mid = half2(1 - barpad - barwidth / 2.0, (index * seg) * barheight + barpad + seg * 0.5);
								bar = bar && withinDist(mid, IN.texcoord, seg * 0.4);
						}


						fixed4 c2 = fixed4(1,1,1,1);
						float winfreq = 1.0, winamp = 1.0;
						if (!bar)
						{
							if (IN.texcoord.y >= _Percent)
							{
								c *= IN.color;
								c.rgb *= c.a;
								return c;
							}
							else
							{
								c.rgb *= fixed4(_Percent, _Percent, _Percent, 1);
								winfreq = 1.0 + _Percent * 3;
								winamp = 1.0 + _Percent * 3;
							}
						}
						else {
							

							float xFishTex = seg * index;
							c2 = tex2D(_FishTex, half2(xFishTex + seg / 2, 0));
							if (c2.a == 0) c2.rgb *= 0.3;
						}
						////
						float sublength = 0.3;
						float time = _Time.x * 5;
						float xcoord = IN.worldpos.x + time;
						float ycoord = IN.worldpos.y + time;
						float decay = max(IN.color.r, max(IN.color.g, IN.color.b));
						//float sublength = 1.0 / lines;
						float inner = fmod(ycoord, sublength);// , sublength);
						float wave = sublength * 0.2 * winamp * ((0.5 + decay) * 2) * sin(xcoord * 10 + time * 10 * winfreq) + (sublength * 0.5);
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


						if (bar) {
							c2.a = 1;
							c *= c2;
						}

						return c;

						
					}
						ENDCG
				}
		}
}