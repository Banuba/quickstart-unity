using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


namespace BNB
{
    public class StaticPosDraw : MonoBehaviour
    {
        public MeshFilter face;
        Vector3[] static_pos_vert;
        Vector2[] static_uv_vert;
        int[] triangles;
        MeshFilter mesh;

        // Start is called before the first frame update
        void Start()
        {
            var static_pos = BanubaSDKBridge.get_static_pos_data();
            mesh = GetComponent<MeshFilter>();

            var static_vert_array = new float[static_pos.vertices_count];
            Marshal.Copy(static_pos.vertices, static_vert_array, 0, static_vert_array.Length);

            static_pos_vert = new Vector3[static_vert_array.Length / 3];
            for (int j = 0, i = 0; j < static_pos_vert.Length; ++j, i += 3) {
                static_pos_vert[j] = new Vector3(static_vert_array[i], static_vert_array[i + 1], static_vert_array[i + 2]);
            }

            var uv_size = BanubaSDKBridge.get_static_uv_size();
            var uv_array = new float[uv_size];
            GCHandle handle = GCHandle.Alloc(uv_array, GCHandleType.Pinned);
            IntPtr address = handle.AddrOfPinnedObject();

            BanubaSDKBridge.get_static_uv(address);

            handle.Free();

            static_uv_vert = new Vector2[uv_size / 2];
            for (int j = 0, i = 0; j < static_uv_vert.Length; ++j, i += 2) {
                static_uv_vert[j] = new Vector2(uv_array[i], uv_array[i + 1]);
            }
        }

        private void Update()
        {
            if (face.mesh.vertices.Length == 0) {
                return;
            }
            mesh.mesh.vertices = static_pos_vert;
            mesh.mesh.uv = static_uv_vert;
            mesh.mesh.triangles = face.mesh.triangles;
        }
    }

}