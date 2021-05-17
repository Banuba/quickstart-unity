using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BNB
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        Camera camera;

        [SerializeField]
        private Text fpsTextObj;

        private float refreshRate = 1f;
        private float timer;

        // Start is called before the first frame update
        void Start()
        {
            camera = GetComponent<Camera>();
            BanubaSDKManager.instance.onRecognitionResult += onRecognitionResult;
        }

        void Update()
        {
            // Update fps meter text every second
            if(fpsTextObj != null)
            {
                if (Time.unscaledTime > timer)
                {
                    int fps = (int)(1f / Time.unscaledDeltaTime);
                    fpsTextObj.text = "FPS: " + fps;
                    timer = Time.unscaledTime + refreshRate;
                }
            }

        }
        void onRecognitionResult(FrameData frameData)
        {
            var error = IntPtr.Zero;
            bool result = BanubaSDKBridge.bnb_frame_data_has_frx_result(frameData, out error);
            Utils.CheckError(error);
            if (result) {
                var face = BanubaSDKBridge.bnb_frame_data_get_face(frameData, 0, out error);
                Utils.CheckError(error);
                if (face.rectangle.hasFaceRectangle > 0) {
                    var mvp = BanubaSDKBridge.bnb_frame_data_get_face_transform(frameData, 0, Screen.width, Screen.height, BanubaSDKBridge.bnb_rect_fit_mode_t.bnb_fit_height, out error);
                    Utils.CheckError(error);
                    var fov = 2F * Mathf.Atan(1F / mvp.p[5]) * Mathf.Rad2Deg;
                    var aspect = mvp.p[5] / mvp.p[0];

                    camera.fieldOfView = fov;
                    camera.aspect = aspect;
                    // camera.farClipPlane = face.camera_position.frustum_f;
                    // camera.nearClipPlane = face.camera_position.frustum_n;
                }
            }
        }
    }

}