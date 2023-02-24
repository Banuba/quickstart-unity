using System;
using UnityEngine;

namespace BNB
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private float _timer;
        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            BanubaSDKManager.instance.onRecognitionResult += OnRecognitionResult;
        }

        private void OnDestroy()
        {
            BanubaSDKManager.instance.onRecognitionResult -= OnRecognitionResult;
        }

        private void OnRecognitionResult(FrameData frameData)
        {
            var error = IntPtr.Zero;
            bool result = BanubaSDKBridge.bnb_frame_data_has_frx_result(frameData, out error);
            Utils.CheckError(error);
            if (!result) {
                return;
            }

            var face = BanubaSDKBridge.bnb_frame_data_get_face(frameData, 0, out error);
            Utils.CheckError(error);
            if (face.rectangle.hasFaceRectangle <= 0) {
                return;
            }

            var mvp = BanubaSDKBridge.bnb_frame_data_get_face_transform(
                frameData, 0, Screen.width, Screen.height, BanubaSDKBridge.bnb_rect_fit_mode_t.bnb_fit_height, out error
            );
            Utils.CheckError(error);

            _camera.fieldOfView = 2F * Mathf.Atan(1F / mvp.p[5]) * Mathf.Rad2Deg;
            _camera.aspect = mvp.p[5] / mvp.p[0];
        }
    }

}
