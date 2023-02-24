using System;
using UnityEngine;

namespace BNB
{
    public class FaceController : MonoBehaviour
    {
        private int _faceIndex;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            BanubaSDKManager.instance.onRecognitionResult += OnRecognitionResult;
        }

        public void Initialize(int faceIndex)
        {
            _faceIndex = faceIndex;
            GetComponentInChildren<FaceMeshController>().FaceIndex = _faceIndex;
        }

        private void OnDestroy()
        {
            BanubaSDKManager.instance.onRecognitionResult -= OnRecognitionResult;
        }

        private void OnRecognitionResult(FrameData frameData)
        {
            var error = IntPtr.Zero;
            var res = BanubaSDKBridge.bnb_frame_data_has_frx_result(frameData, out error);
            Utils.CheckError(error);
            if (!res) {
                return;
            }

            var face = BanubaSDKBridge.bnb_frame_data_get_face(frameData, _faceIndex, out error);
            Utils.CheckError(error);
            if (face.rectangle.hasFaceRectangle > 0) {
                gameObject.SetActive(true);
            } else {
                gameObject.SetActive(false); // hide when no face detected
                return;
            }

            var mvp = BanubaSDKBridge.bnb_frame_data_get_face_transform(
                frameData, _faceIndex, Screen.width, Screen.height, BanubaSDKBridge.bnb_rect_fit_mode_t.bnb_fit_height, out error
            );
            Utils.CheckError(error);

            var mv = Utils.ArrayToMatrix4x4(mvp.mv);
            _transform.localScale = mv.lossyScale;
            _transform.position = mv.GetColumn(3);
            _transform.rotation = mv.rotation;
        }
    }

}