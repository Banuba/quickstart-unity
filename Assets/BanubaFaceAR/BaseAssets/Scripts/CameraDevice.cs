using System;
using System.Collections;
using UnityEngine;
#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.iOS;
#endif
namespace BNB
{
    [DisallowMultipleComponent]
    public class CameraDevice : MonoBehaviour
    {
        public event Action<BanubaSDKBridge.bnb_bpc8_image_t> onCameraImage;
        public event Action<Texture2D, CameraTextureData> onCameraTexture;

        public static CameraDevice instance;

        public bool isLocal;
        public CameraTextureData cameraTextureData;
        private Texture2D _cameraTexture;
        [SerializeField]
        private Texture2D localTexture;

        private int _texSize = 0;
        private bool _dataChanged = false;
        private Color32[] _data;
        private Color32[] _prevData;
        private Color32Pinner _dataPinner;
        private WebCamTexture _webCamTexture;
        private BanubaSDKBridge.bnb_bpc8_image_t _cameraImage;

        public Texture2D CameraTexture => _cameraTexture;

        public struct CameraTextureData
        {
            public bool isVerticallyFlipped;
            public bool isRotated90; // clockwise
            public int angle;
        }

        private void Awake()
        {
            _cameraTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            if (instance == null) {
                instance = this;
            } else {
                return;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera)) {
                Permission.RequestUserPermission(Permission.Camera);
                return;
            }
#endif
            cameraTextureData = new CameraTextureData();
            OpenCameraDevice();
        }

#if UNITY_IOS && !UNITY_EDITOR
        IEnumerator Start()
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam)) {
                OpenCameraDevice();
            }
        }
#endif

        private void Update()
        {
            if (_webCamTexture == null) {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (Permission.HasUserAuthorizedPermission(Permission.Camera)) {
                    OpenCameraDevice();
                    return;
                }
#endif
                Debug.Log("ERROR: Camera not opened!");
                return;
            }

#if UNITY_WEB && !UNITY_EDITOR

            UpdateCameraImage();

#else
            if (_webCamTexture.didUpdateThisFrame) {
                UpdateCameraImage();
            }
#endif
        }

        private void OnDestroy()
        {
            if (_webCamTexture != null) {
                _webCamTexture.Stop();
                _webCamTexture = null;
            }
        }

        private void OpenCameraDevice()
        {
            string deviceName = null;

            Debug.Log("Webcam's available: ");
            foreach (WebCamDevice device in WebCamTexture.devices) {
                Debug.Log("Name: " + device.name);
                Debug.Log("isFrontFacing: " + device.isFrontFacing);
                var resolutions = device.availableResolutions;
                if (resolutions != null) {
                    foreach (Resolution resolution in resolutions) {
                        Debug.Log("\t" + resolution.ToString());
                    }
                }
                if (device.isFrontFacing) {
                    deviceName = device.name;
                    break;
                }
            }

            if (WebCamTexture.devices.Length > 0) {
                if (deviceName != null) {
                    _webCamTexture = new WebCamTexture(deviceName);
                } else {
                    // default device
                    _webCamTexture = new WebCamTexture();
                }
                _webCamTexture.requestedFPS = 30;
                _webCamTexture.Play();
            }

            if (_webCamTexture == null) {
                Debug.Log("Camera creation error!");
                return;
            }

            _cameraImage = new BanubaSDKBridge.bnb_bpc8_image_t {
                format = new BanubaSDKBridge.bnb_image_format_t()
            };
            _data = new Color32[_texSize];
            UpdateCameraImage();
        }

        private void UpdateCameraImage()
        {
            updateCameraTexture();
            if (_prevData != null && _prevData.Length == (CameraTexture.width * CameraTexture.height)) {
                CameraTexture.SetPixels32(_prevData);
                CameraTexture.Apply();
            }

            _dataChanged = cameraTextureData.angle != _webCamTexture.videoRotationAngle
                           || cameraTextureData.isVerticallyFlipped != _webCamTexture.videoVerticallyMirrored;
            cameraTextureData.angle = _webCamTexture.videoRotationAngle;
            cameraTextureData.isVerticallyFlipped = cameraTextureData.angle == 90 || cameraTextureData.angle == 180;
            cameraTextureData.isRotated90 = cameraTextureData.angle == 90 || cameraTextureData.angle == 270;
            _texSize = (isLocal ? localTexture.width * localTexture.height : _webCamTexture.width * _webCamTexture.height);
            if (_texSize != _data.Length || _dataChanged) {
                _texSize = (isLocal ? localTexture.width * localTexture.height : _webCamTexture.width * _webCamTexture.height);
                _data = new Color32[_texSize];
                _dataPinner = new Color32Pinner(_data);
            }

            if (isLocal) {
                _data = localTexture.GetPixels32();
            } else {
                _webCamTexture.GetPixels32(_data);
            }

            if (_data == null) {
                Debug.Log("ERROR: GetPixels32 return not valid data (null)");
                return;
            }

            _cameraImage.format.orientation = AngleToOrientation(cameraTextureData.angle);
            _cameraImage.format.require_mirroring = 1; // selfie mode: true
            _cameraImage.format.face_orientation = 0;
            if (isLocal) {
                _cameraImage.format.width = (uint) localTexture.width;
                _cameraImage.format.height = (uint) localTexture.height;
                _dataPinner = new Color32Pinner(localTexture.GetPixels32()); // texture passed to FRX HERE
            } else {
                _cameraImage.format.width = (uint) _webCamTexture.width;
                _cameraImage.format.height = (uint) _webCamTexture.height;
            }
            _cameraImage.data = _dataPinner; // Use the operator to retrieve the IntPtr
            _cameraImage.pixel_format = BanubaSDKBridge.bnb_pixel_format_t.BNB_RGBA;

            BanubaSDKManager.processCameraImage(_cameraImage);
            storePixels();
        }

        private void storePixels()
        {
            if (_prevData == null || _prevData.Length != _data.Length) {
                _prevData = new Color32[_data.Length];
            }
            _data.CopyTo(_prevData, 0);
        }

        private void updateCameraTexture()
        {
            if (_prevData == null || (!_dataChanged && _prevData.Length == (CameraTexture.width * CameraTexture.height))) {
                return;
            }

            if (CameraTexture != null) {
                Destroy(CameraTexture);
            }

            if (isLocal) {
                _cameraTexture = new Texture2D(localTexture.width, localTexture.height, TextureFormat.RGBA32, false);
                CameraTexture.SetPixels32(localTexture.GetPixels32());
            } else {
                _cameraTexture = new Texture2D(_webCamTexture.width, _webCamTexture.height, TextureFormat.RGBA32, false);
            }
            onCameraTexture?.Invoke(CameraTexture, cameraTextureData);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (_webCamTexture == null) {
                return;
            }
            if (pauseStatus) {
                _webCamTexture.Stop();
            } else {
                _webCamTexture.Play();
            }
        }

        public void CallCameraEvents()
        {
            onCameraImage?.Invoke(_cameraImage);
            onCameraTexture?.Invoke(CameraTexture, cameraTextureData);
        }

        private BanubaSDKBridge.bnb_image_orientation_t AngleToOrientation(int angle)
        {
            switch (angle) {
                // swap for 180 and 0 due to unity Texture2D.GetPixels32 return vertically flipped image
                case 0:
                    return BanubaSDKBridge.bnb_image_orientation_t.BNB_DEG_180;
                case 90:
                    return BanubaSDKBridge.bnb_image_orientation_t.BNB_DEG_90;
                case 180:
                    return BanubaSDKBridge.bnb_image_orientation_t.BNB_DEG_0;
                case 270:
                    return BanubaSDKBridge.bnb_image_orientation_t.BNB_DEG_270;
                default:
                    return BanubaSDKBridge.bnb_image_orientation_t.BNB_DEG_0;
            }
        }
    }

}