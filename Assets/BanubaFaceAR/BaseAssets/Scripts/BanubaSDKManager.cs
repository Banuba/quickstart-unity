using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace BNB
{
    [DisallowMultipleComponent]
    public class BanubaSDKManager : MonoBehaviour
    {
        public event Action<FrameData> onRecognitionResult;

        public static BanubaSDKManager instance;
        [SerializeField]
        private int _maxFaceCount = 1;
        private const float _refresh = 1.0f;

        private float _timer;

        public int CurrentFps { get; private set; }
        public Recognizer Recognizer { get; private set; }

        private void Awake()
        {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
            }

            var tokenResourceFile = Resources.Load<TextAsset>("BanubaClientToken");
            var tokenLine = tokenResourceFile.text.Trim();

            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_env_init(tokenLine, out error);
            Utils.CheckError(error);

#if (UNITY_ANDROID || UNITY_WEBGL) && !UNITY_EDITOR
            var resourcesPath = Application.persistentDataPath;
#else
            var resourcesPath = Application.streamingAssetsPath;
#endif

            Recognizer = new Recognizer(resourcesPath + "/BanubaFaceAR/");

            // set maximum faces to search
            BanubaSDKBridge.bnb_recognizer_set_max_faces(Recognizer, _maxFaceCount, out error);
            Utils.CheckError(error);

            var isSupporting = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
            Debug.Log($"Beauty ARGBHalf texture support: {isSupporting}");
            Debug.Log($"Screen size: {Screen.width}x{Screen.height}");
        }

        private void Update()
        {
            CalculateFPS();
        }

        private void CalculateFPS()
        {
            var timelapse = Time.smoothDeltaTime;
            _timer = _timer <= 0 ? _refresh : _timer -= timelapse;
            if (_timer <= 0) {
                CurrentFps = (int) (1f / timelapse);
            }
        }

        private void OnDestroy()
        {
            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_env_release(out error);
            Utils.CheckError(error);
        }

        public static bool processCameraImage(BanubaSDKBridge.bnb_bpc8_image_t cameraImage)
        {
            if (instance == null) {
                return false;
            }

            var error = IntPtr.Zero;
            var frameData = BanubaSDKBridge.bnb_frame_data_init(out error);
            Utils.CheckError(error);
            BanubaSDKBridge.bnb_frame_data_set_bpc8_img(frameData, ref cameraImage, out error);
            Utils.CheckError(error);

            BanubaSDKBridge.bnb_recognizer_push_frame_data(instance.Recognizer, frameData, out error);
            Utils.CheckError(error);

            var outFrameData = new FrameData();
            bool process = BanubaSDKBridge.bnb_recognizer_pop_frame_data(instance.Recognizer, outFrameData, out error);
            Utils.CheckError(error);
            if (process) {
                instance.onRecognitionResult?.Invoke(outFrameData);
            }

            return process;
        }
    }

}
