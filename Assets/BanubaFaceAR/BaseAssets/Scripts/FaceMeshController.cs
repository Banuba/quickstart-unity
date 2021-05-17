using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB
{
    public class FaceMeshController : MonoBehaviour
    {
        int faceIndex;

        int[] triangles;
        Vector2[] uv;
        int[] wireIndicies;

        protected Material meshMaterial;
        CameraDevice.CameraTextureData cameraTextureData;

        // Start is called before the first frame update
        void Start()
        {
            var faceController = transform.parent.gameObject.GetComponent<FaceController>();
            faceIndex = faceController.faceIndex;

            BanubaSDKManager.instance.onRecognitionResult += onRecognitionResult;
            BanubaSDKManager.instance.cameraDevice.onCameraTexture += OnCameraTexture;

            meshMaterial = GetComponent<Renderer>().material;
            OnCameraTexture(BanubaSDKManager.instance.cameraDevice.cameraTexture, BanubaSDKManager.instance.cameraDevice.cameraTextureData);
        }
        void OnCameraTexture(Texture2D tex, CameraDevice.CameraTextureData cameraTextureData)
        {
            if (tex == null) {
                return;
            }

            this.cameraTextureData = cameraTextureData;
            GetComponent<Renderer>().material.mainTexture = tex;
        }

        void onRecognitionResult(FrameData frameData)
        {
            var error = IntPtr.Zero;
            var res = BanubaSDKBridge.bnb_frame_data_has_frx_result(frameData, out error);
            Utils.CheckError(error);
            if (!res)
                return;

            var face = BanubaSDKBridge.bnb_frame_data_get_face(frameData, faceIndex, out error);
            Utils.CheckError(error);

            if (face.rectangle.hasFaceRectangle == 0)
                return;

            UpdateIndicies(frameData);

            var vert_array = new float[face.vertices_count];
            Marshal.Copy(face.vertices, vert_array, 0, vert_array.Length);

            var verticies = new Vector3[vert_array.Length / 3];
            for (int j = 0, i = 0; j < verticies.Length; ++j, i += 3) {
                verticies[j] = new Vector3(vert_array[i], vert_array[i + 1], vert_array[i + 2]);
            }

            var faceMesh = GetComponent<MeshFilter>().mesh;

            faceMesh.vertices = verticies;
            faceMesh.uv = uv;
            faceMesh.triangles = triangles;
            //faceMesh.SetIndices(wireIndicies, MeshTopology.Lines, 0);


            var width = meshMaterial.mainTexture.width;
            var height = meshMaterial.mainTexture.height;
            var cameraAngle = cameraTextureData.Angle;
            var cameraVerticalFlip = cameraTextureData.isVerticallyFlipped;
            var rotate = false;
            if (cameraAngle == 90 || cameraAngle == 270) {
                var tmp = width;
                width = height;
                height = tmp;
                rotate = true;
            }

            // get MVP transform for camera texture oriented in GL basis
            var mvp = BanubaSDKBridge.bnb_frame_data_get_face_transform(
                frameData, faceIndex, width, height, BanubaSDKBridge.bnb_rect_fit_mode_t.bnb_fit_height, out error);
            Utils.CheckError(error);
            var mv = Utils.ArrayToMatrix4x4(mvp.mv);
            var p = Utils.ArrayToMatrix4x4(mvp.p);

            meshMaterial.SetMatrix("_TextureMVP", p * mv);
            meshMaterial.SetInt("_TextureRotate", rotate ? 1 : 0);
            meshMaterial.SetInt("_TextureYFlip", cameraVerticalFlip ? 1 : 0);
        }

        void UpdateIndicies(FrameData frameData)
        {
            var error = IntPtr.Zero;

            if (uv == null) {
                var uv_size = BanubaSDKBridge.bnb_frame_data_get_tex_coords_size(frameData, out error);
                Utils.CheckError(error);
                var uv_ptr = BanubaSDKBridge.bnb_frame_data_get_tex_coords(frameData, out error);
                Utils.CheckError(error);
                if (uv_ptr != IntPtr.Zero) {
                    var uv_array = new float[uv_size];
                    Marshal.Copy(uv_ptr, uv_array, 0, uv_array.Length);

                    uv = new Vector2[uv_size / 2];
                    for (int j = 0, i = 0; j < uv.Length; ++j, i += 2) {
                        uv[j] = new Vector2(uv_array[i], uv_array[i + 1]);
                    }
                }
            }

            if (triangles == null) {
                var triangles_size = BanubaSDKBridge.bnb_frame_data_get_triangles_size(frameData, out error);
                Utils.CheckError(error);
                var triangles_ptr = BanubaSDKBridge.bnb_frame_data_get_triangles(frameData, out error);
                Utils.CheckError(error);
                if (triangles_ptr != IntPtr.Zero) {
                    triangles = new int[triangles_size];
                    Marshal.Copy(triangles_ptr, triangles, 0, triangles.Length);
                }
            }

            if (wireIndicies == null) {
                var indicies_size = BanubaSDKBridge.bnb_frame_data_get_wire_indicies_size(frameData, out error);
                Utils.CheckError(error);
                var indicies_ptr = BanubaSDKBridge.bnb_frame_data_get_wire_indicies(frameData, out error);
                Utils.CheckError(error);
                if (indicies_ptr != IntPtr.Zero) {
                    wireIndicies = new int[indicies_size];
                    Marshal.Copy(indicies_ptr, wireIndicies, 0, wireIndicies.Length);
                }
            }
        }
    }
}
