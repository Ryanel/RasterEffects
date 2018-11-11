// Color Quantization Shader
Shader "Hidden/Quantize"
{
	Properties{
		_NumColors("Number of Colors", Int) = 1
		_MainTex("", 2D) = "white" {}
	}

		SubShader{
			Lighting Off
			ZTest Always
			Cull Off
			ZWrite Off
			Fog { Mode Off }

			Pass {
				CGPROGRAM
				#pragma target 3.0
				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest // Use low precision since we're downsampling anyways
				
				#include "UnityCG.cginc"
				#define MAX_COLORS 256 // Maximum amount of colors on screen.

				uniform int _NumColors; // Number of colors
				uniform fixed4 _Colors[MAX_COLORS]; // Colors
				uniform sampler2D _MainTex; // Screen texture

				fixed4 frag(v2f_img i) : COLOR
				{
					fixed3 targetColor = tex2D(_MainTex, i.uv).rgb; // Get the color we're attempting to quantize

					fixed4 resultColor = fixed4(0,0,0,0); // The color that we're comparing against

					fixed lowestDistance = 99999999; // How close this color is to a palette color

					for (int i = 0; i < _NumColors; i++)
					{
						fixed4 canidateColor = _Colors[i];
						fixed targetDistance = distance(targetColor, canidateColor); // Linear distance

						if (targetDistance < lowestDistance)
						{
							lowestDistance = targetDistance;
							resultColor = canidateColor;
						}
					}

					return resultColor;
				}
				ENDCG
			}
	}

		FallBack "Diffuse"
}
