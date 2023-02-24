using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace BNB
{
    // Segmentation feature enabling
    // 1. Add SegmentationFeature prefab to canvas in your effect's hierarchy.
    // 2. Provide it with the reference of your effect's PlaneController object.
    // 3. Set custom material with BNB/Segmentation shader to SegmentationFeature object.

    [RequireComponent(typeof(RawImage))]
    public class SegmentationFeature : MonoBehaviour
    {
        public event Action<Texture2D, BanubaSDKBridge.bnb_segm_mask_t> onMaskReady;

        private readonly int _MaskTransform = Shader.PropertyToID("_MaskTransform");
        private readonly int _Inverse = Shader.PropertyToID("_Inverse");
        private readonly int _IsVerticallyMirrored = Shader.PropertyToID("_IsVerticallyMirrored");
        private readonly int _MaskTex = Shader.PropertyToID("_MaskTex");
        private readonly int _RequireMirroring = Shader.PropertyToID("_RequireMirroring");

        private readonly List<BanubaSDKBridge.bnb_segm_type_t> _transformRequiringMasks =
            new List<BanubaSDKBridge.bnb_segm_type_t> {
                BanubaSDKBridge.bnb_segm_type_t.BNB_NECK,
                BanubaSDKBridge.bnb_segm_type_t.BNB_LIPS,
                BanubaSDKBridge.bnb_segm_type_t.BNB_FACE,
                BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_PUPIL_LEFT,
                BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_PUPIL_RIGHT,
                BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_IRIS_LEFT,
                BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_IRIS_RIGHT,
                BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_SCLERA_LEFT,
                BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_SCLERA_RIGHT
            };

        private readonly List<BanubaSDKBridge.bnb_segm_type_t> _leftEyeMasks =
            new List<BanubaSDKBridge.bnb_segm_type_t> {
                BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_PUPIL_LEFT,
                BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_IRIS_LEFT,
                BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_SCLERA_LEFT,
            };

        [SerializeField]
        public BanubaSDKBridge.bnb_segm_type_t _type;

        // Check if material with "BNB/Segmentation" shader is attached to the same object
        [SerializeField]
        private bool _useSegmentationShader;
        [Header("Required references")]
        [SerializeField]
        private PlaneController _plane;

        private Material _material;
        private RectTransform _rect;
        private RectTransform _planeRectTransform;
        private Texture2D _maskTexture;

        private bool _isPositionTransformRequired;
        private bool _isLeftEyeMask;

        private void Start()
        {
            if (_plane == null) {
                Debug.LogError("PlaneController reference is not provided.", this);
            }

            _material = GetComponent<RawImage>().material;
            _rect = GetComponent<RectTransform>();
            _planeRectTransform = _plane.GetComponent<RectTransform>();

            _isPositionTransformRequired = _transformRequiringMasks.Contains(_type);
            _isLeftEyeMask = _leftEyeMasks.Contains(_type);

            BanubaSDKManager.instance.onRecognitionResult += OnRecognitionResult;

            var featureId = GetFeatureIdBySegmType(_type);
            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_insert_feature(BanubaSDKManager.instance.Recognizer, featureId, out error);
            Utils.CheckError(error);
        }

        private void OnDestroy()
        {
            if (BanubaSDKManager.instance == null) {
                return;
            }

            BanubaSDKManager.instance.onRecognitionResult -= OnRecognitionResult;

            var featureId = GetFeatureIdBySegmType(_type);
            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_remove_feature(BanubaSDKManager.instance.Recognizer, featureId, out error);
            Utils.CheckError(error);
        }

        private ulong GetFeatureIdBySegmType(BanubaSDKBridge.bnb_segm_type_t segmType)
        {
            var featuresId = BanubaSDKBridge.bnb_recognizer_get_features_id();
            switch (segmType) {
                case BanubaSDKBridge.bnb_segm_type_t.BNB_BACKGROUND:
                    return featuresId.background_segm;
                case BanubaSDKBridge.bnb_segm_type_t.BNB_BODY:
                    return featuresId.body_segm;
                case BanubaSDKBridge.bnb_segm_type_t.BNB_FACE:
                    return featuresId.face_segm;
                case BanubaSDKBridge.bnb_segm_type_t.BNB_FACE_SKIN:
                    return featuresId.face_skin_segm;
                case BanubaSDKBridge.bnb_segm_type_t.BNB_HAIR:
                    return featuresId.hair_segm;
                case BanubaSDKBridge.bnb_segm_type_t.BNB_NECK:
                    return featuresId.neck_segm;
                case BanubaSDKBridge.bnb_segm_type_t.BNB_SKIN:
                    return featuresId.skin_segm;
                case BanubaSDKBridge.bnb_segm_type_t.BNB_LIPS:
                    return featuresId.lips_segm;
                case BanubaSDKBridge.bnb_segm_type_t.BNB_BROW_LEFT:
                case BanubaSDKBridge.bnb_segm_type_t.BNB_BROW_RIGHT:
                    return featuresId.brows_segm;
                case BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_PUPIL_LEFT:
                case BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_PUPIL_RIGHT:
                case BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_IRIS_LEFT:
                case BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_IRIS_RIGHT:
                case BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_SCLERA_LEFT:
                case BanubaSDKBridge.bnb_segm_type_t.BNB_EYE_SCLERA_RIGHT:
                    return featuresId.eyes_segm;
                default:
                    throw new ArgumentOutOfRangeException(nameof(segmType), segmType, null);
            }
        }

        private BanubaSDKBridge.bnb_segm_mask_t CreateMaskTexture(FrameData frameData)
        {
            var error = IntPtr.Zero;
            var segmMaskData = BanubaSDKBridge.bnb_frame_data_get_segmentation_mask_data(frameData, _type, out error);
            Utils.CheckError(error);
            if (segmMaskData.data == IntPtr.Zero) {
                Debug.LogWarning("segmMaskData.data == IntPtr.Zero");
                return default;
            }

            _maskTexture = new Texture2D(segmMaskData.commonData.width, segmMaskData.commonData.height, TextureFormat.R8, false);
            var bytes = new byte[segmMaskData.commonData.width * segmMaskData.commonData.height];
            Marshal.Copy(segmMaskData.data, bytes, 0, bytes.Length);
            _maskTexture.LoadRawTextureData(bytes);
            _maskTexture.Apply();

            onMaskReady?.Invoke(_maskTexture, segmMaskData.commonData);

            return segmMaskData.commonData;
        }

        private void OnRecognitionResult(FrameData frameData)
        {
            if (!gameObject.activeSelf) {
                return;
            }
            if (_maskTexture != null) {
                Destroy(_maskTexture);
            }

            var commonData = CreateMaskTexture(frameData);
            if (commonData.glTransform == null) {
                return;
            }

            Matrix4x4 transformMatrix = Utils.ArrayToMatrix4x4(commonData.glTransform);
            Matrix4x4 rotateMatrix = Matrix4x4.Rotate(transformMatrix.inverse.rotation); // rotate texture for background

            _rect.transform.localScale = transformMatrix.lossyScale;
            ApplyRotationTransform(transformMatrix);
            if (_isPositionTransformRequired) {
                ApplyPositionTransform(transformMatrix);
            }

            var angles = transformMatrix.rotation.eulerAngles;
            var planeRect = angles.z == 90 || angles.z == 270
                                ? new Vector2(_planeRectTransform.rect.size.y, _planeRectTransform.rect.size.x)
                                : new Vector2(_planeRectTransform.rect.size.x, _planeRectTransform.rect.size.y);

            // unrotate rect
            if (_plane.cameraAngle == 90 || _plane.cameraAngle == 270) {
                var tmp = planeRect.x;
                planeRect.x = planeRect.y;
                planeRect.y = tmp;
            }
            _rect.sizeDelta = planeRect;

            if (_useSegmentationShader) {
                _material.SetMatrix(_MaskTransform, rotateMatrix);
                _material.SetFloat(_Inverse, 0.0f);
                _material.SetInt(_IsVerticallyMirrored, 0);
                _material.SetInt(_RequireMirroring, 1);
                _material.SetTexture(_MaskTex, _maskTexture);
            }
        }

        private void ApplyPositionTransform(Matrix4x4 transformMatrix)
        {
            Vector3 pos = transformMatrix.GetColumn(3);
            var halfWidth = _planeRectTransform.rect.size.x * 0.5f;
            var halfHeight = _planeRectTransform.rect.size.y * 0.5f;

#if UNITY_STANDALONE || UNITY_EDITOR
            _rect.anchoredPosition = new Vector2(halfWidth * pos.x * -1, halfHeight * pos.y);
#else
            if (Screen.orientation == ScreenOrientation.LandscapeRight || Screen.orientation == ScreenOrientation.LandscapeLeft) {
                _rect.anchoredPosition = new Vector2(halfWidth * pos.x * -1, halfHeight * pos.y);
            } else if (Screen.orientation == ScreenOrientation.Portrait) {
                _rect.anchoredPosition = new Vector2(halfHeight * pos.x * -1, halfWidth * pos.y);
            }
#endif
        }

        private void ApplyRotationTransform(Matrix4x4 transformMatrix)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            _rect.transform.rotation = !_isLeftEyeMask
                                           ? transformMatrix.rotation
                                           : Quaternion.Euler(0, 0, -transformMatrix.rotation.eulerAngles.z);
#else
            var offset = Screen.orientation == ScreenOrientation.Portrait ? 180f : 0f;
            _rect.transform.rotation = !_isLeftEyeMask
                                           ? transformMatrix.rotation
                                           : Quaternion.Euler(0, 0, -transformMatrix.rotation.eulerAngles.z + offset);
#endif
        }
    }
}