using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB
{
    public class FacesController : MonoBehaviour
    {
        GameObject[] faces;
        public event Action<int> onInstanciateFace;
        // Start is called before the first frame update
        void Start()
        {
            faces = new GameObject[1];
            faces[0] = GameObject.Find("Face0");

            BanubaSDKManager.instance.onRecognitionResult += onRecognitionResult;
        }

        protected void OnDestroy()
        {
            BanubaSDKManager.instance.onRecognitionResult -= onRecognitionResult;
        }

        void onRecognitionResult(FrameData frameData)
        {
            var error = IntPtr.Zero;
            var res = BanubaSDKBridge.bnb_frame_data_has_frx_result(frameData, out error);
            Utils.CheckError(error);

            if (!res)
                return;

            var face_count = BanubaSDKBridge.bnb_frame_data_get_face_count(frameData, out error);
            Utils.CheckError(error);
            InstantiateFaces(face_count);
            if (onInstanciateFace != null) {
                onInstanciateFace(face_count);
            }
        }

        void InstantiateFaces(int face_count)
        {
            var len = faces.Length;
            if (len < face_count) {
                Array.Resize(ref faces, face_count);
                for (int i = len; i < face_count; ++i) {
                    faces[i] = GameObject.Instantiate(faces[0], transform);
                    faces[i]
                        .SetActive(true);
                    faces[i].name = "Face" + i;

                    var faceController = faces[i].GetComponent<FaceController>();
                    faceController.faceIndex = i;
                }
            }
        }
    }

}
