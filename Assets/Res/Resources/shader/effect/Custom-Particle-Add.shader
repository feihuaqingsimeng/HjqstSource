Shader "Custom/Particles/Additive" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_AlphaTex("Alpha Texture",2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	//----------------------add------------------------------
	_Position ("Position", Vector) = (0,0,0,0)
	_Scale ("Scale", Vector) = (1,1,1,1)
	_XMin ("x axis min value", Range(-20,20)) = -20
	_XMax ("x axis max value", Range(-20,20)) = 20
	_Yhighest ("Y axis lowest position", Range(0,1)) = 0
	[HideInInspector]_ClipPosition ("Clip Position", Range(0,1)) = 0
	//----------------------add------------------------------	
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma multi_compile CLIP_POSITION_OFF CLIP_POSITION_ON
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			fixed4 _TintColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD1;
				#endif
				#ifdef CLIP_POSITION_ON
					//float y:TEXCOORD2;
					//float x:TEXCOORD3;
					float2 clipRange:TEXCOORD2;
				#endif
			};
			
			float4 _MainTex_ST;
			fixed4 _AlphaTex_ST;
			//----------------------add------------------------------
			float4 _Position;
			float4 _Scale;
			
			#ifdef CLIP_POSITION_ON
				float _XMin;
				float _XMax;
				float _Yhighest;
			#endif
			//----------------------add------------------------------
			v2f vert (appdata_t v)
			{
				v2f o;
				//----------------------add------------------------------
				float4 objV = mul(UNITY_MATRIX_MV, v.vertex);
				objV.xyz -= _Position;
				objV.xyz = float3(_Scale.x * objV.x, _Scale.y * objV.y, _Scale.z * objV.z);
				objV.xyz += _Position;
				o.vertex = mul(UNITY_MATRIX_P, objV);				
				#ifdef CLIP_POSITION_ON
					float4 pos = mul(_Object2World,v.vertex);
					o.clipRange = pos.xy;
				#endif
				//----------------------add------------------------------
				//o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			float _InvFade;

			fixed4 frag (v2f i) : SV_Target
			{
				#ifdef CLIP_POSITION_ON
					clip(_Yhighest - i.clipRange.y);
					clip(i.clipRange.x - _XMin);
					clip(_XMax - i.clipRange.x);
				#endif
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif
				
				fixed4 tex= tex2D(_MainTex, i.texcoord);
				tex.a = tex2D(_AlphaTex, i.texcoord).r;
				return 2.0f * i.color * _TintColor * tex;
			}
			ENDCG 
		}
	}	
}
	CustomEditor "ParticleMaterialEditor"
}
