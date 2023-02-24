using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace BNB.Makeup
{
    public class LipsMakeup : MonoBehaviour
    {
        private readonly int _ShineMaskTex = Shader.PropertyToID("_ShineMaskTex");
        private readonly int _CameraTex = Shader.PropertyToID("_CameraTex");
        private readonly int _LipsMaskTex = Shader.PropertyToID("_LipsMaskTex");
        private readonly int _NoiseTex = Shader.PropertyToID("_NoiseTex");
        private readonly int _LipsTransform = Shader.PropertyToID("_LipsTransform");
        private readonly int _LipsShineParams = Shader.PropertyToID("_LipsShineParams");
        private readonly int _IsMatt = Shader.PropertyToID("_IsMatt");
        private readonly int _AddRotation = Shader.PropertyToID("_AddRotation");
        private readonly int _IsVerticalRatioCorrection = Shader.PropertyToID("_IsVerticalRatioCorrection");
        private readonly int _Color = Shader.PropertyToID("_Color");
        private readonly int _SaturationBrightness = Shader.PropertyToID("_SaturationBrightness");
        private readonly int _ShineIntensityBleedingScale = Shader.PropertyToID("_ShineIntensityBleedingScale");
        private readonly int _GlitterBleedingIntensityGrain = Shader.PropertyToID("_GlitterBleedingIntensityGrain");

        [Header("Common options")]
        public Color color;
        [Range(0, 1)]
        public float brightness = 1;
        [Header("Extended options")]
        [Range(0, 1)]
        public float saturation = 1;
        [Range(0, 2)]
        public float shineIntensity = 1;
        [Range(0, 1)]
        public float shineBleeding;
        [Range(0, 1)]
        public float shineScale;
        [Range(0, 2)]
        public float glitterGrain;
        [Range(0, 2)]
        public float glitterIntensity;
        [Range(0, 2)]
        public float glitterBleeding;

        [Header("Required references")]
        [SerializeField]
        private SegmentationFeature _segmMask;
        [SerializeField]
        private Texture2D _noiseTexture;

        private Material _material;
        private Texture2D _lipsShineTexture;

        private void Awake()
        {
            _material = _segmMask.GetComponent<RawImage>().material;
        }

        private void Start()
        {
            var featuresID = BanubaSDKBridge.bnb_recognizer_get_features_id();
            BanubaSDKBridge.bnb_recognizer_insert_feature(BanubaSDKManager.instance.Recognizer, featuresID.lips_shine, out var error);
            Utils.CheckError(error);

            BanubaSDKManager.instance.onRecognitionResult += OnRecognitionResult;
            _segmMask.onMaskReady += OnSegmMaskReady;
        }

        private void OnDestroy()
        {
            var featuresID = BanubaSDKBridge.bnb_recognizer_get_features_id();
            BanubaSDKBridge.bnb_recognizer_remove_feature(BanubaSDKManager.instance.Recognizer, featuresID.lips_shine, out var error);
            Utils.CheckError(error);

            BanubaSDKManager.instance.onRecognitionResult -= OnRecognitionResult;
            _segmMask.onMaskReady -= OnSegmMaskReady;
        }

        private void OnRecognitionResult(FrameData frameData)
        {
            if (!gameObject.activeInHierarchy || !enabled) {
                return;
            }

            var shineData = CreateLipsShineTexture(frameData);

            bool isMatt = shineScale == 0 && glitterGrain == 0;
            _material.SetInt(_IsMatt, isMatt ? 1 : 0);

            _material.SetTexture(_CameraTex, CameraDevice.instance.CameraTexture);
            _material.SetTexture(_ShineMaskTex, _lipsShineTexture);
            _material.SetTexture(_NoiseTex, _noiseTexture);
            _material.SetVector(_LipsShineParams, new Vector4(shineData.v_min, shineData.v_max, 0, 0));

            _material.SetVector(_Color, color);
            _material.SetVector(_SaturationBrightness, new Vector4(saturation, brightness, 0, 0));
            _material.SetVector(_ShineIntensityBleedingScale, new Vector4(shineIntensity, shineBleeding, shineScale));
            _material.SetVector(_GlitterBleedingIntensityGrain, new Vector4(glitterBleeding, glitterIntensity, glitterGrain, 0));


            if (Application.isMobilePlatform) {
                var angle = CameraDevice.instance.cameraTextureData.angle;
                if (angle == 180) {
                    angle = 0;
                } else if (angle == 0) {
                    angle = 180;
                }
                Matrix4x4 rotate = Matrix4x4.Rotate(Quaternion.Euler(0, 180, angle));
                _material.SetMatrix(_AddRotation, rotate);
            }

            var isMobilePortrait = Application.isMobilePlatform && (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown);
            _material.SetInt(_IsVerticalRatioCorrection, isMobilePortrait ? 1 : 0);
        }

        private void OnSegmMaskReady(Texture2D lipsSegmTexture, BanubaSDKBridge.bnb_segm_mask_t segmMaskData)
        {
            Matrix4x4 maskTransform = Utils.ArrayToMatrix4x4(segmMaskData.glTransform);
            _material.SetMatrix(_LipsTransform, Matrix4x4.Rotate(maskTransform.rotation));
            _material.SetTexture(_LipsMaskTex, lipsSegmTexture);
        }

        private BanubaSDKBridge.bnb_lips_shine_mask_t CreateLipsShineTexture(FrameData frameData)
        {
            if (_lipsShineTexture != null) {
                Destroy(_lipsShineTexture);
            }
            var lipsData = BanubaSDKBridge.bnb_frame_data_get_lips_shine_mask(frameData, out var error);
            Utils.CheckError(error);
            if (lipsData.data == IntPtr.Zero) {
                Debug.LogWarning("LipsShineData is empty. ");
                return default;
            }
            _lipsShineTexture = new Texture2D(lipsData.commonData.width, lipsData.commonData.height, TextureFormat.R8, false);
            var bytes = new byte[lipsData.commonData.width * lipsData.commonData.height];
            Marshal.Copy(lipsData.data, bytes, 0, bytes.Length);
            _lipsShineTexture.LoadRawTextureData(bytes);
            _lipsShineTexture.Apply();

            return lipsData.commonData;
        }

        private void OnEnable()
        {
            _segmMask.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            _segmMask.gameObject.SetActive(false);
        }

        [ContextMenu("Shiny Preset")]
        public void Shiny()
        {
            saturation = 1;
            shineIntensity = 1;
            shineBleeding = 0.5f;
            shineScale = 1;
            glitterIntensity = 0;
            glitterBleeding = 0;
        }

        [ContextMenu("Glitter Preset")]
        public void Glitter()
        {
            saturation = 1;
            shineIntensity = 0.9f;
            shineBleeding = 0.6f;
            shineScale = 1;
            glitterGrain = 0.4f;
            glitterIntensity = 1;
            glitterBleeding = 1;
        }

        [ContextMenu("Matt Preset")]
        public void Matt()
        {
            saturation = 1;
            shineIntensity = 0;
            shineBleeding = 0;
            shineScale = 0;
            glitterGrain = 0;
            glitterIntensity = 0;
            glitterBleeding = 0;
        }
    }
}