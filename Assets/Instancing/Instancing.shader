// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

	Shader "Custom/Instancing" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader { 
		Tags { "RenderType"="Opaque" }
		LOD 700
		Cull Off

		Pass {	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct vs2ps {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				half4 n : TEXCOORD1;
			};
			
			StructuredBuffer<uint> indexBuf;
			StructuredBuffer<float3> vertexBuf;
			StructuredBuffer<float2> uvBuf;
			//StructuredBuffer<float4x4> worldBuf;
			RWStructuredBuffer<float3> posBuf;
			RWStructuredBuffer<float3> speedBuf;

			vs2ps vert(uint vertexId : SV_VertexID, uint instanceId : SV_InstanceID) {
				vs2ps o;
				
				uint index = indexBuf[vertexId];
				float4 vertex = float4(vertexBuf[index], 1);
				float2 uv = uvBuf[index];
				//float4x4 worldMat = worldBuf[instanceId];
				float3 tr = posBuf[instanceId];

				vertex = (0.02*vertex)+float4(tr,0);//mul(worldMat, vertex);
				o.vertex = UnityObjectToClipPos(vertex);//UnityObjectToClipPos(vertex);
				o.uv = uv;
				o.n = half4(speedBuf[instanceId],1);//mul(worldMat, float4(0,0,0,1));
				return o;
			}

			float4 frag(vs2ps IN) : COLOR {
				float4 c = IN.n;//float4(IN.uv,0,1);//tex2D(_MainTex, IN.uv);
				return c;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
