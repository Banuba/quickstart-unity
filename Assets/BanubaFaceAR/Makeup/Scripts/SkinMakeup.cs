using UnityEngine;
using UnityEngine.UI;

namespace BNB.Makeup
{
    public class SkinMakeup : MonoBehaviour
    {
        private readonly int _MaskTex = Shader.PropertyToID("_MaskTex");
        private readonly int _CameraTex = Shader.PropertyToID("_CameraTex");
        private readonly int _Color = Shader.PropertyToID("_Color");
        private readonly int _AddRotation = Shader.PropertyToID("_AddRotation");
        private readonly int _SofteningStrength = Shader.PropertyToID("_SofteningStrength");
        private readonly int _MaskTransform = Shader.PropertyToID("_MaskTransform");

        [Header("Options")]
        public Color color;
        [Range(0, 1)]
        public float softeningStrength;

        [Header("Required references")]
        [SerializeField]
        private SegmentationFeature _segmMask;

        private Material _material;

        private void Awake()
        {
            _material = _segmMask.GetComponent<RawImage>().material;
        }

        private void Start()
        {
            BanubaSDKManager.instance.onRecognitionResult += OnRecognitionResult;
            _segmMask.onMaskReady += OnSegmMaskReady;
        }

        private void OnDestroy()
        {
            BanubaSDKManager.instance.onRecognitionResult -= OnRecognitionResult;
            _segmMask.onMaskReady -= OnSegmMaskReady;
        }

        private void OnRecognitionResult(FrameData frameData)
        {
            if (!gameObject.activeInHierarchy || !enabled) {
                return;
            }

            Matrix4x4 addRotation =
                Matrix4x4.Rotate(Quaternion.Euler(0, 0, CameraDevice.instance.cameraTextureData.angle));

            _material.SetMatrix(_AddRotation, addRotation);
            _material.SetTexture(_CameraTex, CameraDevice.instance.CameraTexture);
            _material.SetFloat(_SofteningStrength, softeningStrength);
            _material.SetColor(_Color, color);
        }

        private void OnSegmMaskReady(Texture2D skinSegmTexture, BanubaSDKBridge.bnb_segm_mask_t segmMaskData)
        {
            Matrix4x4 maskTransform = Utils.ArrayToMatrix4x4(segmMaskData.glTransform);
            _material.SetMatrix(_MaskTransform, maskTransform);
            _material.SetTexture(_MaskTex, skinSegmTexture);
        }

        private void OnEnable()
        {
            _segmMask.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            _segmMask.gameObject.SetActive(false);
        }
    }
}