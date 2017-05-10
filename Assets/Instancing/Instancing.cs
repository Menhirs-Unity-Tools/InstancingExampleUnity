using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Instancing {

	public class Instancing : MonoBehaviour {
		public const string CS_INDEX_BUFFER = "indexBuf";
		public const string CS_VERTEX_BUFFER = "vertexBuf";
		public const string CS_UV_BUFFER = "uvBuf";
		public const string CS_WORLD_BUFFER = "worldBuf";

		public GameObject prefab;
		public int count;
		public float range = 10f;
		public float rotationSpeed = 90f;

		private ComputeBuffer _indexBuf;
		private ComputeBuffer _vertexBuf;
		private ComputeBuffer _uvBuf;
		private ComputeBuffer _posBuf;
		private ComputeBuffer _speeddBuf;
		private float[] _poss;
		private float[] _speeds;
		private Material _mat;

		public ComputeShader computeShader;
		private int mComputeShaderKernelID;

		void OnDestroy() {
			_indexBuf.Release();
			_vertexBuf.Release();
			_uvBuf.Release();
			_posBuf.Release ();
			_speeddBuf.Release ();
		}

		void Start() {
			var mf = prefab.GetComponent<MeshFilter>();
			var mesh = mf.sharedMesh;

			_indexBuf = new ComputeBuffer(mesh.triangles.Length, Marshal.SizeOf(mesh.triangles[0]));
			_indexBuf.SetData(mesh.triangles);
			
			_vertexBuf = new ComputeBuffer(mesh.vertices.Length, Marshal.SizeOf(mesh.vertices[0]));
			_vertexBuf.SetData(mesh.vertices);

			_uvBuf = new ComputeBuffer(mesh.uv.Length, Marshal.SizeOf(mesh.uv[0]));
			_uvBuf.SetData(mesh.uv);

			_poss = new float[3 * count];
			_speeds = new float[3 * count];
			for (int i = 0; i < count; i++) {
				_poss [3 * i + 0] = Random.Range (-range, range);
				_poss [3 * i + 1] = Random.Range (0.0f, 2*range);
				_poss [3 * i + 2] = Random.Range (-range, range);
				_speeds [3 * i + 0] = 0.0f;
				_speeds [3 * i + 1] = 0.0f;
				_speeds [3 * i + 2] = 0.0f;
			}
			_posBuf = new ComputeBuffer(count, 3 * Marshal.SizeOf(_poss[0]));
			_posBuf.SetData(_poss);
			_speeddBuf = new ComputeBuffer(count, 3 * Marshal.SizeOf(_speeds[0]));
			_speeddBuf.SetData(_speeds);

			mComputeShaderKernelID = computeShader.FindKernel("CSMain");
			computeShader.SetBuffer(mComputeShaderKernelID, "posBuf", _posBuf);
			computeShader.SetBuffer(mComputeShaderKernelID, "speedBuf", _speeddBuf);

			_mat = new Material(prefab.GetComponent<Renderer>().sharedMaterial);
			_mat.SetBuffer(CS_INDEX_BUFFER, _indexBuf);
			_mat.SetBuffer(CS_VERTEX_BUFFER, _vertexBuf);
			_mat.SetBuffer(CS_UV_BUFFER, _uvBuf);
			_mat.SetBuffer("posBuf", _posBuf);
			_mat.SetBuffer("speedBuf", _speeddBuf);
		}
		void FixedUpdate()
		{
			computeShader.SetFloat("deltaTime", Time.fixedDeltaTime/100.0f);
			computeShader.Dispatch(mComputeShaderKernelID, count, 1, 1);
		}

		void OnRenderObject() {
			_mat.SetPass(0);
			Graphics.DrawProcedural(MeshTopology.Triangles, _indexBuf.count, count);
		}

		/*
		void UpdateWorlds() {
			// HLSL : colum major matrix
			var c = 0;
			for (var i = 0; i < _trs.Length; i++) {
				var w = _trs[i].localToWorldMatrix;
				_worlds [c++] = w.m00;
				_worlds [c++] = w.m10;
				_worlds [c++] = w.m20;
				_worlds [c++] = w.m30;
				_worlds [c++] = w.m01;
				_worlds [c++] = w.m11;
				_worlds [c++] = w.m21;
				_worlds [c++] = w.m31;
				_worlds [c++] = w.m02;
				_worlds [c++] = w.m12;
				_worlds [c++] = w.m22;
				_worlds [c++] = w.m32;
				_worlds [c++] = w.m03;
				_worlds [c++] = w.m13;
				_worlds [c++] = w.m23;
				_worlds [c++] = w.m33;
			}
			_worldBuf.SetData(_worlds);
		}

		void UpdateRotations() {
			var rot = Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
			foreach (var tr in _trs) {
				tr.localRotation = rot * tr.localRotation;
			}
		}

		Transform[] GenerateRandom(GameObject prefab, int count) {
			var trs = new Transform[count];
			for (var i = 0; i < count; i++) {
				var pos = new Vector3(Random.Range(-range, range), Random.Range(0.0f, 2.0f*range), Random.Range(-range, range));
				trs [i] = Generate (prefab, pos, Random.rotationUniform, 0.02f*Vector3.one);//prefab.transform.localScale);
				//trs[i] = Generate(prefab, Vector3.zero, Quaternion.identity, Vector3.one);
			}
			return trs;
		}

		Transform Generate(GameObject prefab, Vector3 localPosition, Quaternion localRotation, Vector3 localScale) {
			var go = (GameObject)Instantiate(prefab);
			go.transform.parent = transform;
			go.transform.localPosition = localPosition;
			go.transform.localRotation = localRotation;
			go.transform.localScale = localScale;
			return go.transform;
		}*/
	}
}