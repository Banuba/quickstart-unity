using System;
using UnityEngine;

namespace BNB
{
    public class FacesController : MonoBehaviour
    {
        // Drop FacesController prefab to EffectBase hierarchy to enable face tracking and face mesh.

        public event Action<int> onInstantiateFace;

        private FaceController _faceTemplate;

        private void Awake()
        {
            _faceTemplate = GetComponentInChildren<FaceController>();
            BanubaSDKManager.instance.onRecognitionResult += OnRecognitionResult;
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

            var face_count = BanubaSDKBridge.bnb_frame_data_get_face_count(frameData, out error);
            Utils.CheckError(error);
            OnFaceInstantiatedHandler(face_count);

            if (onInstantiateFace != null) {
                onInstantiateFace(face_count);
            }
        }

        private void OnFaceInstantiatedHandler(int faceCount)
        {
            int morphsCount = transform.childCount;
            if (morphsCount == faceCount) {
                return;
            }
            if (morphsCount < faceCount) {
                for (int i = morphsCount; i < faceCount; i++) {
                    CreateFace(i);
                }
            } else if (morphsCount > faceCount) {
                for (int i = morphsCount; i > faceCount; i--) {
                    Destroy(transform.GetChild(morphsCount).gameObject);
                }
            }
        }

        private void CreateFace(int faceIndex)
        {
            FaceController face = Instantiate(_faceTemplate, transform);
            face.Initialize(faceIndex);
            face.name = "Face" + faceIndex;
            face.gameObject.SetActive(true);
        }

        public FaceController GetFace(int index)
        {
            if (index >= transform.childCount) {
                return null;
            }
            return transform.GetChild(index).GetComponent<FaceController>();
        }
    }

}
