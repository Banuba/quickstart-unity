using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace BNB
{
    public class ColoredBackground : MonoBehaviour
    {
        private GameObject cameraPlane => GameObject.Find("CameraPlane");
        private Material material => GetComponent<RawImage>().material;
        private RectTransform rectTransform => GetComponent<RectTransform>();
        private RectTransform cameraPlaneTranform => cameraPlane?.GetComponent<RectTransform>();
        private PlaneController planeController => cameraPlane?.GetComponent<PlaneController>();
        private Texture2D maskTexture;
        // Start is called before the first frame update
        void Start()
        {
            BanubaSDKManager.instance.onRecognitionResult += onRecognitionResult;
            Debug.Log("ColoredBackground Start()");
            var error = IntPtr.Zero;
            var featuresId = BanubaSDKBridge.bnb_recognizer_get_features_id();

            BanubaSDKBridge.bnb_recognizer_insert_feature(BanubaSDKManager.instance.recognizer, featuresId.background_squared, out error);
            Utils.CheckError(error);
        }

        protected void OnDestroy()
        {
            BanubaSDKManager.instance.onRecognitionResult -= onRecognitionResult;
            var featuresId = BanubaSDKBridge.bnb_recognizer_get_features_id();
            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_remove_feature(BanubaSDKManager.instance.recognizer, featuresId.background_squared, out error);
            Utils.CheckError(error);
        }


        BanubaSDKBridge.bnb_segm_mask_t createGPUMaskTexture(FrameData frameData)
        {
            if (!BanubaSDKManager.instance.surfaceCreated) {
                Debug.LogWarning("!BanubaSDKManager.instance.surfaceCreated");
                return default(BanubaSDKBridge.bnb_segm_mask_t);
            }
            var error = IntPtr.Zero;

            var bgMaskData = BanubaSDKBridge.bnb_frame_data_get_background_mask_data_gpu(frameData, out error);
            Utils.CheckError(error);
            if ((IntPtr) bgMaskData.textureHandle == IntPtr.Zero) {
                Debug.LogWarning("bgMaskData.textureHandle == IntPtr.Zero");
                return default(BanubaSDKBridge.bnb_segm_mask_t);
            }

            maskTexture = Texture2D.CreateExternalTexture(bgMaskData.commonData.width, bgMaskData.commonData.height, TextureFormat.R8, false, false, (IntPtr) bgMaskData.textureHandle);
            return bgMaskData.commonData;
        }

        BanubaSDKBridge.bnb_segm_mask_t createMaskTexture(FrameData frameData)
        {
            var error = IntPtr.Zero;

            var bgMaskData = BanubaSDKBridge.bnb_frame_data_get_background_mask_data(frameData, out error);
            Utils.CheckError(error);
            if (bgMaskData.data == IntPtr.Zero) {
                Debug.LogWarning("bgMaskData.data == IntPtr.Zero");
                return default(BanubaSDKBridge.bnb_segm_mask_t);
            }

            maskTexture = new Texture2D(bgMaskData.commonData.width, bgMaskData.commonData.height, TextureFormat.R8, false);
            var bytes = new byte[bgMaskData.commonData.width * bgMaskData.commonData.height];
            Marshal.Copy(bgMaskData.data, bytes, 0, bytes.Length);

            maskTexture.LoadRawTextureData(bytes);
            maskTexture.Apply();

            return bgMaskData.commonData;
        }

        void onRecognitionResult(FrameData frameData)
        {
            if (!gameObject.activeSelf) {
                return;
            }

            if (maskTexture != null) {
                UnityEngine.Object.Destroy(maskTexture);
            }

            if (cameraPlaneTranform == null || planeController == null) {
                return;
            }
            var error = IntPtr.Zero;
            var commonData = BanubaSDKBridge.bnb_background_use_gpu(out error) ? createGPUMaskTexture(frameData) : createMaskTexture(frameData);
            Utils.CheckError(error);
            if (commonData.glTransform == null) {
                return;
            }

            Matrix4x4 transformMatrixFixed = Utils.ArrayToMatrix4x4(commonData.glTransform);

            Matrix4x4 rotateMatrix = Matrix4x4.Rotate(transformMatrixFixed.inverse.rotation); // rotate texture for background
            const float inverse = 0.0f;
            const bool isVerticallyMirrored = false;

            material.SetMatrix("_MaskTransform", rotateMatrix);
            material.SetFloat("_Inverse", inverse);
            material.SetInt("_IsVerticallyMirrored", isVerticallyMirrored ? 1 : 0);
            material.SetTexture("_BGMaskTex", maskTexture);

            rectTransform.transform.localScale = transformMatrixFixed.lossyScale;
            rectTransform.transform.rotation = transformMatrixFixed.rotation;

            var planeRect = Vector2.zero;
            if (transformMatrixFixed.rotation.eulerAngles.z == 90 || transformMatrixFixed.rotation.eulerAngles.z == 270) {
                planeRect = new Vector2(cameraPlaneTranform.sizeDelta.y, cameraPlaneTranform.sizeDelta.x);
            } else {
                planeRect = new Vector2(cameraPlaneTranform.sizeDelta.x, cameraPlaneTranform.sizeDelta.y);
            }

            // unrotate rect
            if (planeController.cameraAngle == 90 || planeController.cameraAngle == 270) {
                var tmp = planeRect.x;
                planeRect.x = planeRect.y;
                planeRect.y = tmp;
            }

            rectTransform.sizeDelta = planeRect;
        }
    }

}
