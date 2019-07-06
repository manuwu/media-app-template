﻿// <copyright file="VideoUnlitShader.cs" company="Google Inc.">
// Copyright (C) 2017 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

//
// This shader renders from OES_external_image textures which require special
// OpenGLES extensions and a special texture sampler.
//
Shader "StevenLi/Bend Video Unlit Shader For MultiView" {
	Properties{
		_Gamma("Video gamma", Range(0.01,3.0)) = 1.0
		_MainTex("Base (RGB)", 2D) = "white" {}
	[KeywordEnum(None, TopBottom, LeftRight)] _StereoMode("Stereo mode", Float) = 0
		[Toggle(FLIP_X)] _FlipX("Flip X", Float) = 0

		_P("BendSize", float) = 0.3
		_XScaleIsYTimes("XScaleIsY Times", float) = 1.0
		_XScaleIsZTimes("XScaleIsZ Times", float) = 1.0
		_CurveXBendRatio("CurveX Bend Ratio", Range(0, 1)) = 0.0
		_CurveYBendRatio("CurveY Bend Ratio", Range(0, 1)) = 0.0

		_R("Radius", float) = 0.3
		_BendRatio("Bend Ratio", Range(0, 1)) = 0.0
		[Toggle(DEBUG)] _Debug("Debug", Float) = 0
		_Offset("Offset",Float) = 1.0
	}

		SubShader
	{
		Pass
	{
		Tags{ "RenderType" = "Opaque" }

		Lighting Off
		Cull Off

		GLSLPROGRAM
#pragma only_renderers gles gles3
		#extension GL_OVR_multiview2 : require
		#extension GL_OES_EGL_image_external : require
		#extension GL_OES_EGL_image_external_essl3 : enable

#pragma multi_compile DEBUG_OFF DEBUG
#pragma multi_compile ___ _STEREOMODE_TOPBOTTOM _STEREOMODE_LEFTRIGHT
#pragma multi_compile ___ FLIP_X

		precision mediump int;
	precision mediump float;

#ifdef VERTEX
	uniform mat4 video_matrix;
	uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
	//uniform int Svr_StereoEyeIndex;
	uniform 	vec4 _MainTex_ST;
	layout(std140) uniform UnityStereoGlobals{
		vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
	};
	layout(num_views = 2) in;
	in highp vec4 in_POSITION0;
	in highp vec2 in_TEXCOORD0;
	out highp vec2 vs_TEXCOORD0;
	out highp vec4 debug_Color;
	vec4 u_xlat0;
	int u_xlati1;
	vec4 u_xlat2;
	//varying vec2 uv;
	uniform float _Offset;
	uniform float _P;//抛物线顶点P
	uniform float _XScaleIsYTimes;//x缩放是y缩放几倍
	uniform float _XScaleIsZTimes;//x缩放是z缩放几倍

	uniform float _CurveXBendRatio;//抛物线水平弯曲度
	uniform float _CurveYBendRatio;//抛物线垂直弯曲度

	uniform float _R;//半径
	uniform float _BendRatio;//总的弯曲度

	vec4 CreateCurvePoint(vec4 vertex)
	{
		float xBendRatio = _CurveXBendRatio * _BendRatio;
		float yBendRatio = _CurveYBendRatio * _BendRatio;
		float disSquare = pow(vertex.x * xBendRatio, 2.0) + pow(vertex.y * yBendRatio, 2.0);
		vertex.z = -disSquare / (2.0 * _P) * _XScaleIsZTimes;
		return vertex;
	}

	vec4 CreateCylinderPoint(vec4 vertex)
	{
		float pi = 3.14159265358979;
		if (_BendRatio > 0.0)
		{
			float r = _R / sin(pi / 2.0 * _BendRatio);

			vec2 circleZeroPoint = vec2(0.0, -r);
			float radian;
			if (r != 0.0)
				radian = abs(vertex.x) / r;
			else
				radian = 0.0;

			int dir = 0;
			if (vertex.x > 0.0)
				dir = 1;
			else if (vertex.x < 0.0)
				dir = -1;

			if (dir > 0)
				radian = pi / 2.0 - radian;
			else
				radian = radian + pi / 2.0;

			float newX = cos(radian) * r;
			float newZ = sin(radian) * r;
			vec2 circlePoint = vec2(newX, newZ) + circleZeroPoint;

			vertex.x = circlePoint.x;
			vertex.z = circlePoint.y * _XScaleIsZTimes;
		}

		return vertex;
	}

	vec4 CreateSpherePoint(vec4 vertex)
	{
		float pi = 3.14159265358979;
		if (_BendRatio > 0.0)
		{
			float r = _R / sin(pi / 2.0 * _BendRatio);
			float alpha;
			if (vertex.x != 0.0)
				alpha = atan(vertex.y / vertex.x);
			else
			{
				if (vertex.y > 0.0)
					alpha = pi / 2.0;
				else if (vertex.y < 0.0)
					alpha = -pi / 2.0;
				else
					return vec4(0, 0, 0, 1);
			}

			if (vertex.x < 0.0)
				alpha = pi + alpha;

			float dis = sqrt(pow(vertex.x, 2.0) + pow(vertex.y, 2.0));
			float beta;
			if (r != 0.0)
				beta = dis / r;
			else
				beta = 0.0;

			float newX = sin(beta) * r * cos(alpha);
			float newY = sin(beta) * r * sin(alpha);
			float newZ = cos(beta) * r - r;

			return vec4(newX, newY * _XScaleIsYTimes, newZ * _XScaleIsZTimes, 1.0);
		}
		else
			return vertex;
	}

	void main()
	{
		vec4 bendVertex = CreateCurvePoint(in_POSITION0);
		bendVertex.y *= _Offset;

		//gl_Position = gl_ModelViewProjectionMatrix * bendVertex;
		u_xlat0 = bendVertex.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
		u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * bendVertex.xxxx + u_xlat0;
		u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * bendVertex.zzzz + u_xlat0;
		u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
		u_xlati1 = int(gl_ViewID_OVR) << 2;
		u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
		u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
		u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
		gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;

		vec4 untransformedUV = gl_MultiTexCoord0;
#ifdef FLIP_X
		untransformedUV.x = 1.0 - untransformedUV.x;
#endif  // FLIP_X
#ifdef DEBUG
		if (u_xlati1 == 0) {
			debug_Color = vec4(1, 0, 0, 1);
		}
		else
		{
			debug_Color = vec4(0, 1, 0, 1);
		}

#endif
#ifdef _STEREOMODE_TOPBOTTOM
		untransformedUV.y *= 0.5;
		if (u_xlati1 == 0) {
			untransformedUV.y += 0.5;
		}
#endif  // _STEREOMODE_TOPBOTTOM
#ifdef _STEREOMODE_LEFTRIGHT
		untransformedUV.x *= 0.5;
		if (u_xlati1 != 0) {
			untransformedUV.x += 0.5;
		}
#endif  // _STEREOMODE_LEFTRIGHT

		vs_TEXCOORD0 = (video_matrix * untransformedUV).xy;
	}
#endif  // VERTEX

#ifdef FRAGMENT
	#version 300 es

		precision highp int;
	uniform lowp samplerExternalOES _MainTex;
	uniform float _Gamma;
	in highp vec2 vs_TEXCOORD0;
	in highp vec4 debug_Color;
	layout(location = 0) out mediump vec4 SV_Target0;
	lowp vec4 u_xlat10_0;
	vec3 gammaCorrect(vec3 v, float gamma) {
		return pow(v, vec3(1.0 / gamma));
	}

	// Apply the gamma correction.  One possible optimization that could
	// be applied is if _Gamma == 2.0, then use gammaCorrectApprox since sqrt will be faster.
	// Also, if _Gamma == 1.0, then there is no effect, so this call could be skipped all together.
	vec4 gammaCorrect(vec4 v, float gamma) {
		return vec4(gammaCorrect(v.xyz, gamma), v.w);
	}

	void main()
	{
		u_xlat10_0 = gammaCorrect(texture(_MainTex, vs_TEXCOORD0.xy), _Gamma);
		SV_Target0 = u_xlat10_0;
#ifdef DEBUG
		SV_Target0 = SV_Target0 * debug_Color;
#endif
		return;
	}
#endif  // FRAGMENT
	ENDGLSL
	}
	}
		Fallback "Unlit/Texture"
}
