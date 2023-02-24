using UnityEngine;
using UnityEngine.UI;

namespace BNB.Makeup
{
    public class BrowMakeup : MonoBehaviour
    {
        private readonly int _MaskTex = Shader.PropertyToID("_BrowMaskTex");
        private readonly int _CameraTex = Shader.PropertyToID("_CameraTex");
        private readonly int _MaskTransform = Shader.PropertyToID("_MaskTransform");
        private readonly int _Color = Shader.PropertyToID("_Color");
        private readonly int _AddRotation = Shader.PropertyToID("_AddRotation");

        [Header("Options")]
        public Color leftBrowColor;
        public Color rightBrowColor;

        [Header("Required references")]
        [SerializeField]
        private SegmentationFeature _leftSegmMask;
        [SerializeField]
        private SegmentationFeature _rightSegmMask;

        private Material _leftBrowMaterial;
        private Material _rightBrowMaterial;

        private void Awake()
        {
            _leftBrowMaterial = _leftSegmMask.GetComponent<RawImage>().material;
            _rightBrowMaterial = _rightSegmMask.GetComponent<RawImage>().material;
        }

        private void Start()
        {
            BanubaSDKManager.instance.onRecognitionResult += OnRecognitionResult;
            _leftSegmMask.onMaskReady += OnLeftSegmMaskReady;
            _rightSegmMask.onMaskReady += OnRightSegmMaskReady;
        }

        private void OnDestroy()
        {
            BanubaSDKManager.instance.onRecognitionResult -= OnRecognitionResult;
            _leftSegmMask.onMaskReady -= OnLeftSegmMaskReady;
            _rightSegmMask.onMaskReady -= OnRightSegmMaskReady;
        }

        private void OnRecognitionResult(FrameData frameData)
        {
            if (!gameObject.activeInHierarchy || !enabled) {
                return;
            }

            Matrix4x4 addRotation =
                Matrix4x4.Rotate(Quaternion.Euler(0, 0, CameraDevice.instance.cameraTextureData.angle));

            _leftBrowMaterial.SetMatrix(_AddRotation, addRotation);
            _leftBrowMaterial.SetTexture(_CameraTex, CameraDevice.instance.CameraTexture);
            _leftBrowMaterial.SetColor(_Color, leftBrowColor);

            _rightBrowMaterial.SetMatrix(_AddRotation, addRotation);
            _rightBrowMaterial.SetTexture(_CameraTex, CameraDevice.instance.CameraTexture);
            _rightBrowMaterial.SetColor(_Color, rightBrowColor);
        }

        private void OnLeftSegmMaskReady(Texture2D browSegmTexture, BanubaSDKBridge.bnb_segm_mask_t segmMaskData)
        {
            Matrix4x4 maskTransform = Utils.ArrayToMatrix4x4(segmMaskData.glTransform);
            _leftBrowMaterial.SetMatrix(_MaskTransform, maskTransform);
            _leftBrowMaterial.SetTexture(_MaskTex, browSegmTexture);
        }

        private void OnRightSegmMaskReady(Texture2D browSegmTexture, BanubaSDKBridge.bnb_segm_mask_t segmMaskData)
        {
            Matrix4x4 maskTransform = Utils.ArrayToMatrix4x4(segmMaskData.glTransform);
            _rightBrowMaterial.SetMatrix(_MaskTransform, maskTransform);
            _rightBrowMaterial.SetTexture(_MaskTex, browSegmTexture);
        }

        private void OnEnable()
        {
            _leftSegmMask.gameObject.SetActive(true);
            _rightSegmMask.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            _leftSegmMask.gameObject.SetActive(false);
            _rightSegmMask.gameObject.SetActive(false);
        }
    }
}