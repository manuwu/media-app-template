Shader "Unlit/GLSLMyBlitCopy"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			GLSLPROGRAM
			#pragma only_renderers gles gles3
			// make fog work
			precision mediump int;
			precision mediump float;
#ifdef VERTEX
			#version 300 es

			uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
			uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
			uniform 	mediump vec4 _MainTex_ST;
			in highp vec4 in_POSITION0;
			in highp vec2 in_TEXCOORD0;
			out highp vec2 vs_TEXCOORD0;
			vec4 u_xlat0XMH;
			vec4 u_xlat1;
			void main()
			{
				vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				u_xlat0XMH = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
				u_xlat0XMH = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0XMH;
				u_xlat0XMH = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0XMH;
				u_xlat0XMH = u_xlat0XMH + hlslcc_mtx4x4unity_ObjectToWorld[3];
				u_xlat1 = u_xlat0XMH.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
				u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0XMH.xxxx + u_xlat1;
				u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0XMH.zzzz + u_xlat1;
				gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0XMH.wwww + u_xlat1;
				return;
			}

#endif
#ifdef FRAGMENT
			#version 300 es

			precision highp int;
			uniform 	mediump vec4 _MainTex_ST;
			uniform lowp sampler2D _MainTex;
			in highp vec2 vs_TEXCOORD0;
			layout(location = 0) out mediump vec4 SV_Target0;
			vec2 u_xlat0;
			lowp vec4 u_xlat10_0XMH;
			void main()
			{
				u_xlat0.xy = vs_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				u_xlat10_0XMH = texture(_MainTex, u_xlat0.xy);
				SV_Target0 = u_xlat10_0XMH;
				return;
			}

#endif
			ENDGLSL
		}
	}
		//Fallback "Unlit/Texture"
}
