using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB.Morphing
{
    public class MorphingUVDraw : MonoBehaviour
    {
        private Mesh _faceMesh;
        private Vector3[] _staticPosVert;
        private Vector2[] _staticUvVert;

        private Mesh _meshComponent;

        public void Initialize(MeshFilter faceMeshFilter)
        {
            _faceMesh = faceMeshFilter.mesh;
            _meshComponent = GetComponent<MeshFilter>().mesh;

            var staticPos = BanubaSDKBridge.get_static_pos_data();
            var staticVertArray = new float[staticPos.vertices_count];
            Marshal.Copy(staticPos.vertices, staticVertArray, 0, staticVertArray.Length);

            _staticPosVert = new Vector3[staticVertArray.Length / 3];
            for (int j = 0, i = 0; j < _staticPosVert.Length; ++j, i += 3) {
                _staticPosVert[j] = new Vector3(staticVertArray[i], staticVertArray[i + 1], staticVertArray[i + 2]);
            }

            var uvSize = BanubaSDKBridge.get_static_uv_size();
            var uvArray = new float[uvSize];
            GCHandle handle = GCHandle.Alloc(uvArray, GCHandleType.Pinned);
            IntPtr address = handle.AddrOfPinnedObject();
            BanubaSDKBridge.get_static_uv(address);
            handle.Free();

            _staticUvVert = new Vector2[uvSize / 2];
            for (int j = 0, i = 0; j < _staticUvVert.Length; ++j, i += 2) {
                _staticUvVert[j] = new Vector2(uvArray[i], uvArray[i + 1]);
            }
        }

        private void Update()
        {
            if (_faceMesh.vertices.Length == 0) {
                return;
            }
            _meshComponent.vertices = _faceMesh.vertices;
            _meshComponent.normals = _staticPosVert;
            _meshComponent.uv = _faceMesh.uv;
            _meshComponent.triangles = _faceMesh.triangles;
        }
    }
}
