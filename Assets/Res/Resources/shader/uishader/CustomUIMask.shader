Shader "Custom/CustomUIMask" {
	Properties {
	 	_MainTex ("Base (RGB)", 2D) = "white" {}
	 	_Color ("Tint", Color) = (1,1,1,1)
	 	_MaskTex ("Culling Mask", 2D) = "white" {}
	 	_Cutoff("Alpha cutoff", Range(0, 1)) = 0.1
	}
	SubShader {
		Tags {"Queue" = "Transparent"}
		Lighting off
		ZWrite off
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest GEqual[_Cutoff]
		
		Pass{
			SetTexture[_MaskTex]{combine texture}
			SetTexture[_MainTex]
			{
				combine texture, texture-previous				
			}
			SetTexture[_MainTex]
			{
				constantColor [_Color]
				combine previous * constant, previous * constant
			}
		}
	}
}
