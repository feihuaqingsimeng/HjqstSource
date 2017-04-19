Shader "Custom/Particles/Sine_Add" {
	Properties {
		_MainTex ("Base layer (RGB)", 2D) = "white" {}
		_DetailTex ("2nd layer (RGB)", 2D) = "white" {}
		_ScrollX ("Base layer Scroll speed X", Float) = 1.0
		_ScrollY ("Base layer Scroll speed Y", Float) = 0.0
		_Scroll2X ("2nd layer Scroll speed X", Float) = 1.0
		_Scroll2Y ("2nd layer Scroll speed Y", Float) = 0.0
		_Color("Color", Color) = (1,1,1,1)
		_MMultiplier ("Layer Multiplier", Float) = 2.0
		_RealTime("Real Time",Float)=0.0
		//----------------------add------------------------------
		_Position ("Position", Vector) = (0,0,0,0)
		_Scale ("Scale", Vector) = (1,1,1,1)
		//----------------------add------------------------------	
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
		Blend SrcAlpha One
		Cull Off 
		Lighting Off 
		ZWrite Off 
		Fog { Color (0,0,0,0) }
		
		
	CGINCLUDE   
	#include "UnityCG.cginc"
	sampler2D _MainTex;
	sampler2D _DetailTex;

	float4 _MainTex_ST;
	float4 _DetailTex_ST;
	
	float _ScrollX;
	float _ScrollY;
	float _Scroll2X;
	float _Scroll2Y;
	float _MMultiplier;
	float4 _Color;
	float _RealTime;
	//----------------------add------------------------------
	float4 _Position;
	float4 _Scale;
	//----------------------add------------------------------
	
	struct v2f {
		float4 pos : SV_POSITION;
		float4 uv : TEXCOORD0;
		fixed4 color : TEXCOORD1;
	};

	
	v2f vert (appdata_full v)
	{
		v2f o;
		//----------------------add------------------------------
		float4 objV = mul(UNITY_MATRIX_MV, v.vertex);
		objV.xyz -= _Position;
		objV.xyz = float3(_Scale.x * objV.x, _Scale.y * objV.y, _Scale.z * objV.z);
		objV.xyz += _Position;
		o.pos = mul(UNITY_MATRIX_P, objV);				
		//----------------------add------------------------------
		//o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		//o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex) + frac(float2(_ScrollX, _ScrollY) * _Time);
		//o.uv.zw = TRANSFORM_TEX(v.texcoord.xy,_DetailTex) + frac(float2(_Scroll2X, _Scroll2Y) * _Time);
		o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex) + frac(float2(_ScrollX, _ScrollY) * _RealTime);
		o.uv.zw = TRANSFORM_TEX(v.texcoord.xy,_DetailTex) + frac(float2(_Scroll2X, _Scroll2Y) * _RealTime);
		
		o.color = _MMultiplier * _Color * v.color;
		return o;
	}
	ENDCG


	Pass {
		Name "SINE_BLEND"
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest		
		fixed4 frag (v2f i) : COLOR
		{
			fixed4 o;
			fixed4 tex = tex2D (_MainTex, i.uv.xy);
			fixed4 tex2 = tex2D (_DetailTex, i.uv.zw);
			
			o = tex * tex2 * i.color;
						
			return o;
		}
		ENDCG 
	}

	} 
}
