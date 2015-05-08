Shader "Custom/BackShader"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
			[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
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
					};

					fixed4 _Color;

					v2f vert(appdata_t IN)
					{
						v2f OUT;
						OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
						OUT.texcoord = IN.texcoord;
						OUT.color = IN.color * _Color;
#ifdef PIXELSNAP_ON
						OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

						return OUT;
					}

					sampler2D _MainTex;

					fixed4 frag(v2f IN) : SV_Target
					{
						fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;

						float alph = c.a;
						
						
						float xlen = (1 / 27.0);
						float ylen = (1 / 15.0);
						float xA = fmod(IN.texcoord.x, xlen * 2);
						float yA = fmod(IN.texcoord.y, ylen * 2);
						bool xHalf = xA > xlen;
						bool yHalf = yA > ylen;

						float2 v = float2(floor(IN.texcoord.x/ xlen) * xlen + xlen / 2, floor(IN.texcoord.y/ ylen) * ylen + ylen / 2);

						fixed4 c1 = tex2D(_MainTex, v) * IN.color;

						if (!xHalf && !yHalf) v += float2(0, ylen);
						else if (!xHalf && yHalf) v += float2(xlen, 0);
						else if (xHalf && yHalf) v += float2(0, -ylen);
						else if (xHalf && !yHalf) v += float2(-xlen, 0);

						float speed = 0.5;
						float time = fmod(_Time.y * speed, 4);

						fixed4 c2;

						if (time < 1 || (time > 2 && time < 3))
						{
							c2 = tex2D(_MainTex, v) * IN.color;

							fixed4 c3 = c2 - c1;
							c3 *= fmod(time, 1);

							c += c3;
						}
						else
						{
							c2 = tex2D(_MainTex, v) * IN.color;
							fixed4 c3 = c1 - c2;
							c3 *= fmod(time, 1);

							c = c2 + c3;
							c *= alph;
						}

						//if (alph != 1)
						//{
						//	return c.rgb * alph;
						//}

						
						//float4 xxx = float4(0, xlen, 0, -xlen);
						//float4 yyy = float4(-ylen, 0, ylen, 0);
						//int index = 0 * 1;
						//float2 v = float2(0, 0);
						//if (!xHalf && !yHalf) v = float2(0, ylen);
						//else if (!xHalf && yHalf) v = float2(xlen, 0);
						//else if (xHalf && yHalf) v = float2(0, -ylen);
						//else if (xHalf && !yHalf) v = float2(-xlen, 0);
						//
						//float speed = 0.5;
						//float time = fmod(_Time.y * speed, 4);
						//float2 v2 = float2(0, 0);
						//int i = 1;
						//int ii = 1;
						//for (; i < 4; i++)
						//{
						//	if (i != 0 && time <= (float)i)
						//	{
						//		if (!xHalf && !yHalf) v2 += float2(xxx[i-ii], yyy[i-ii]);
						//		else if (!xHalf && yHalf) v2 += float2(xxx[(i -ii) % 4], yyy[(i -ii) % 4]);
						//		else if (xHalf && yHalf) v2 += float2(xxx[(i -ii) % 4], yyy[(i -ii) % 4]);
						//		else if (xHalf && !yHalf) v2 += float2(xxx[(i -ii) % 4], yyy[(i -ii) % 4]);
						//	}
						//}
						//float2 ncoord = IN.texcoord + v * fmod(_Time.y * speed, 1) +v2;

						//c = tex2D(_MainTex, ncoord) * IN.color;

						//c.rgb *= c.a;
						c.rgb *= alph;
						//alph = 1;
						//c = float4(alph, alph, alph, 1);

						//c = float4(IN.texcoord.y, IN.texcoord.y, IN.texcoord.y, 1);
						return c;
					}
						ENDCG
				}
		}
}