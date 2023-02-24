using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Video;

namespace BNB
{
    public class FaceMeshController : MonoBehaviour
    {
        private readonly int _EnableMakeup = Shader.PropertyToID("_EnableMakeup");
        private readonly int _TextureMvp = Shader.PropertyToID("_TextureMVP");
        private readonly int _TextureRotate = Shader.PropertyToID("_TextureRotate");
        private readonly int _TextureYFlip = Shader.PropertyToID("_TextureYFlip");

        protected Material _meshMaterial;
        private int[] _triangles;
        private int[] _wireIndicies;
        private Vector2[] _uv;
        private CameraDevice.CameraTextureData _cameraTextureData;

        private Mesh _mesh;
        private Renderer _rendererComponent;
        private VideoPlayer _videoPlayer;

        public int FaceIndex { get; set; }

        [SerializeField]
        private FaceMeshController _face;

        private void Awake()
        {
            _mesh = GetComponent<MeshFilter>().mesh;
            _rendererComponent = GetComponent<Renderer>();
            _videoPlayer = GetComponent<VideoPlayer>();

            BanubaSDKManager.instance.onRecognitionResult += OnRecognitionResult;
            CameraDevice.instance.onCameraTexture += OnCameraTexture;
            _meshMaterial = GetComponent<Renderer>().material;
            if (_videoPlayer) {
                _videoPlayer.sendFrameReadyEvents = true;
                _videoPlayer.frameReady += OnVideoTexture;
                _meshMaterial.SetInt(_EnableMakeup, 0);
            }
            OnCameraTexture(CameraDevice.instance.CameraTexture, CameraDevice.instance.cameraTextureData);
        }

        private void OnDestroy()
        {
            BanubaSDKManager.instance.onRecognitionResult -= OnRecognitionResult;
            CameraDevice.instance.onCameraTexture -= OnCameraTexture;
        }

        private void OnCameraTexture(Texture2D tex, CameraDevice.CameraTextureData cameraTextureData)
        {
            if (tex == null) {
                return;
            }
            _cameraTextureData = cameraTextureData;
            _rendererComponent.material.mainTexture = tex;
        }

        private void OnVideoTexture(VideoPlayer source, long frameIdx)
        {
            _meshMaterial.SetInt(_EnableMakeup, 1);
            _videoPlayer.sendFrameReadyEvents = false;
            _videoPlayer.frameReady -= OnVideoTexture;
        }

        private void OnRecognitionResult(FrameData frameData)
        {
            var error = IntPtr.Zero;
            var res = BanubaSDKBridge.bnb_frame_data_has_frx_result(frameData, out error);
            Utils.CheckError(error);
            if (!res) {
                Debug.Log("frx false");
                return;
            }

            var face = BanubaSDKBridge.bnb_frame_data_get_face(frameData, FaceIndex, out error);
            Utils.CheckError(error);

            if (face.rectangle.hasFaceRectangle == 0) {
                Debug.Log("face has rectangles = false");
                return;
            }

            UpdateIndicies(frameData);

            var vert_array = new float[face.vertices_count];
            Marshal.Copy(face.vertices, vert_array, 0, vert_array.Length);

            var vertices = new Vector3[vert_array.Length / 3];
            for (int j = 0, i = 0; j < vertices.Length; ++j, i += 3) {
                vertices[j] = new Vector3(vert_array[i], vert_array[i + 1], vert_array[i + 2]);
            }

            var faceMesh = _mesh;
            faceMesh.vertices = vertices;
            faceMesh.uv = _uv;
            faceMesh.triangles = _triangles;

            var width = _meshMaterial.mainTexture.width;
            var height = _meshMaterial.mainTexture.height;
            if (_cameraTextureData.isRotated90) {
                var tmp = width;
                width = height;
                height = tmp;
            }

            // get MVP transform for camera texture oriented in GL basis
            var mvp = BanubaSDKBridge.bnb_frame_data_get_face_transform(
                frameData, FaceIndex, width, height, BanubaSDKBridge.bnb_rect_fit_mode_t.bnb_fit_height, out error
            );
            Utils.CheckError(error);
            var mv = Utils.ArrayToMatrix4x4(mvp.mv);
            var p = Utils.ArrayToMatrix4x4(mvp.p);

            _meshMaterial.SetMatrix(_TextureMvp, p * mv);
            _meshMaterial.SetInt(_TextureRotate, _cameraTextureData.isRotated90 ? 1 : 0);
            _meshMaterial.SetInt(_TextureYFlip, _cameraTextureData.isVerticallyFlipped ? 1 : 0);
        }

        private void UpdateIndicies(FrameData frameData)
        {
            var error = IntPtr.Zero;

            if (_uv == null) {
                var uv_size = BanubaSDKBridge.bnb_frame_data_get_tex_coords_size(frameData, out error);
                Utils.CheckError(error);
                var uv_ptr = BanubaSDKBridge.bnb_frame_data_get_tex_coords(frameData, out error);
                Utils.CheckError(error);
                if (uv_ptr != IntPtr.Zero) {
                    var uv_array = new float[uv_size];
                    Marshal.Copy(uv_ptr, uv_array, 0, uv_array.Length);

                    _uv = new Vector2[uv_size / 2];
                    for (int j = 0, i = 0; j < _uv.Length; ++j, i += 2) {
                        _uv[j] = new Vector2(uv_array[i], uv_array[i + 1]);
                    }
                }
            }

            if (_triangles == null) {
                var triangles_size = BanubaSDKBridge.bnb_frame_data_get_triangles_size(frameData, out error);
                Utils.CheckError(error);
                var triangles_ptr = BanubaSDKBridge.bnb_frame_data_get_triangles(frameData, out error);
                Utils.CheckError(error);
                if (triangles_ptr != IntPtr.Zero) {
                    _triangles = new int[triangles_size];
                    Marshal.Copy(triangles_ptr, _triangles, 0, _triangles.Length);
                }
            }

            if (_wireIndicies == null) {
                var indicies_size = BanubaSDKBridge.bnb_frame_data_get_wire_indicies_size(frameData, out error);
                Utils.CheckError(error);
                var indicies_ptr = BanubaSDKBridge.bnb_frame_data_get_wire_indicies(frameData, out error);
                Utils.CheckError(error);
                if (indicies_ptr != IntPtr.Zero) {
                    _wireIndicies = new int[indicies_size];
                    Marshal.Copy(indicies_ptr, _wireIndicies, 0, _wireIndicies.Length);
                }
            }
        }
    }
}
