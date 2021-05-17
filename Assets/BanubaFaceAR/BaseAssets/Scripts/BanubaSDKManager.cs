using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;

namespace BNB
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class BanubaSDKManager : MonoBehaviour
    {
        public static BanubaSDKManager instance = null;

        CameraDevice mCameraDevice;
        public CameraDevice cameraDevice
        {
            get
            {
                return mCameraDevice;
            }
            private set
            {
                mCameraDevice = value;
            }
        }

        Recognizer mRecognizer;
        public Recognizer recognizer
        {
            get
            {
                return mRecognizer;
            }
            private set
            {
                mRecognizer = value;
            }
        }
        public volatile bool surfaceCreated = false;

        const int face_count = 1;

        public event Action<FrameData> onRecognitionResult;

        private delegate void SurfaceCreatedRunFn(int index);
        private void SurfaceCreated(int index)
        {
            var error = IntPtr.Zero;
            Debug.Log("SurfaceCreated call");
            BanubaSDKBridge.bnb_recognizer_surface_created(recognizer, Screen.width, Screen.height, out error);
            Utils.CheckError(error);
            surfaceCreated = true;
        }

        void Awake()
        {
            if (instance == null) {
                instance = this;
            } else {
                return;
            }

            var tokenResourceFile = Resources.Load<TextAsset>("BanubaClientToken");
            var tokenLine = tokenResourceFile.text.Trim();

            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_env_init(tokenLine, out error);
            Utils.CheckError(error);
            cameraDevice = GetComponent<CameraDevice>();

#if (UNITY_ANDROID || UNITY_WEBGL) && !UNITY_EDITOR
            var resourcesPath = Application.persistentDataPath;
#else
            var resourcesPath = Application.streamingAssetsPath;
#endif

            recognizer = new Recognizer(resourcesPath + "/BanubaFaceAR/");
            // set maximum faces to search
            BanubaSDKBridge.bnb_recognizer_set_max_faces(recognizer, face_count, out error);
            Utils.CheckError(error);

            // set face search algorithm
            BanubaSDKBridge.bnb_recognizer_set_face_search_mode(
                recognizer, BanubaSDKBridge.bnb_face_search_mode_t.bnb_good_for_first_face, out error);
            Utils.CheckError(error);

            var issupport = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
            Debug.Log("Beauty ARGBHalf texture support: " + issupport);
            Debug.Log("Screen size: " + Screen.width + "x" + Screen.height);
        }

        void Start()
        {
            cameraDevice.onCameraImage += onCameraImage;
#if UNITY_ANDROID && !UNITY_EDITOR
            var error = IntPtr.Zero;
            if (BanubaSDKBridge.bnb_use_gpu_features(out error))
            {
                Utils.CheckError(error);

                Debug.Log("SystemInfo.graphicsMultiThreaded : " + SystemInfo.graphicsMultiThreaded);
                if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3)
                {
                    Debug.LogError("SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3. Please, remove other GL API from player settings");
                }

                if (SystemInfo.graphicsMultiThreaded)
                {
                    GL.IssuePluginEvent(Marshal.GetFunctionPointerForDelegate(new SurfaceCreatedRunFn(SurfaceCreated)), 0);
                }
                else
                {
                    SurfaceCreated(0);
                }
            }

            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaClass bnbFileUtilClass = new AndroidJavaClass("com.banuba.utils.FileUtilsNN");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

            bnbFileUtilClass.CallStatic("setContext", context);
            bnbFileUtilClass.CallStatic("setResourcesBasePath", Application.persistentDataPath + "/BanubaFaceAR/android_nn");
#endif
        }

        void OnDestroy()
        {
            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_env_release(out error);
            Utils.CheckError(error);
        }

        void Update()
        {
        }

        void onCameraImage(BanubaSDKBridge.bnb_bpc8_image_t image)
        {
            var frameData = new FrameData();
            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_frame_data_set_bpc8_img(frameData, ref image, out error);
            Utils.CheckError(error);

            bool process = BanubaSDKBridge.bnb_recognizer_process_frame_data(recognizer, frameData, out error);
            Utils.CheckError(error);
            if (process && onRecognitionResult != null) {
                onRecognitionResult(frameData);
            }
        }
    }

}
