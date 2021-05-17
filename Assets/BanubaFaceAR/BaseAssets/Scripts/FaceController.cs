using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB
{
    public class FaceController : MonoBehaviour
    {
        public int faceIndex;

        // Start is called before the first frame update
        void Start()
        {
            BanubaSDKManager.instance.onRecognitionResult += onRecognitionResult;
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
            if (face.rectangle.hasFaceRectangle > 0)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false); // hide when no face detected
                return;
            }

            var mvp = BanubaSDKBridge.bnb_frame_data_get_face_transform(
                frameData, faceIndex, Screen.width, Screen.height, BanubaSDKBridge.bnb_rect_fit_mode_t.bnb_fit_height, out error);
            Utils.CheckError(error);
            var mv = Utils.ArrayToMatrix4x4(mvp.mv);

            transform.localScale = mv.lossyScale;
            transform.position = mv.GetColumn(3);
            transform.rotation = mv.rotation;
        }
    }

}