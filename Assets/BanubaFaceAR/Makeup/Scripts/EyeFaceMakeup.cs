using UnityEngine;

namespace BNB.Makeup
{
    public class EyeFaceMakeup : MonoBehaviour
    {
        private readonly int _ContourTex = Shader.PropertyToID("_ContourTex");
        private readonly int _BlushesTex = Shader.PropertyToID("_BlushesTex");
        private readonly int _HighlighterTex = Shader.PropertyToID("_HighlighterTex");
        private readonly int _EyeshadowTex = Shader.PropertyToID("_EyeshadowTex");
        private readonly int _EyelinerTex = Shader.PropertyToID("_EyelinerTex");
        private readonly int _LashesTex = Shader.PropertyToID("_LashesTex");
        private readonly int _MakeupTex = Shader.PropertyToID("_MakeupTex");
        private readonly int _ContourColor = Shader.PropertyToID("_ContourColor");
        private readonly int _BlushesColor = Shader.PropertyToID("_BlushesColor");
        private readonly int _HighlighterColor = Shader.PropertyToID("_HighlighterColor");
        private readonly int _EyeshadowColor = Shader.PropertyToID("_EyeshadowColor");
        private readonly int _EyelinerColor = Shader.PropertyToID("_EyelinerColor");
        private readonly int _LashesColor = Shader.PropertyToID("_LashesColor");
        private readonly int _IsMakeupTex = Shader.PropertyToID("_IsMakeupTex");

        [Header("Options")]
        public Color contourColor;
        public Color blushesColor;
        public Color highlighterColor;
        public Color eyeshadowColor;
        public Color eyelinerColor;
        public Color lashesColor;
        [Header("Extended Options")]
        public Texture2D makeupTexture;

        [Header("Required references")]
        [SerializeField]
        private Texture2D contourTexture;
        [SerializeField]
        private Texture2D blushesTexture;
        [SerializeField]
        private Texture2D highlighterTexture;
        [SerializeField]
        private Texture2D eyeshadowTexture;
        [SerializeField]
        private Texture2D eyelinerTexture;
        [SerializeField]
        private Texture2D lashesTexture;

        [Header("Required Face Controller reference")]
        [SerializeField]
        private GameObject _faceController;

        private Material _material;

        private void Awake()
        {
            if (_faceController == null) {
                Debug.Log("need to set up FaceController ref!");
                return;
            }
            _material = _faceController.GetComponentInChildren<Renderer>().material;
        }

        private void Start()
        {
            var featureIds = BanubaSDKBridge.bnb_recognizer_get_features_id();
            BanubaSDKBridge.bnb_recognizer_insert_feature(BanubaSDKManager.instance.Recognizer, featureIds.eyes_correction, out var error);
            Utils.CheckError(error);

            _material.SetTexture(_ContourTex, contourTexture);
            _material.SetTexture(_BlushesTex, blushesTexture);
            _material.SetTexture(_HighlighterTex, highlighterTexture);
            _material.SetTexture(_EyeshadowTex, eyeshadowTexture);
            _material.SetTexture(_EyelinerTex, eyelinerTexture);
            _material.SetTexture(_LashesTex, lashesTexture);
        }

        private void OnDestroy()
        {
            var featureIds = BanubaSDKBridge.bnb_recognizer_get_features_id();
            BanubaSDKBridge.bnb_recognizer_remove_feature(BanubaSDKManager.instance.Recognizer, featureIds.eyes_correction, out var error);
            Utils.CheckError(error);
        }

        private void Update()
        {
            if (makeupTexture != null) {
                _material.SetInt(_IsMakeupTex, 1);
                _material.SetTexture(_MakeupTex, makeupTexture);
            } else {
                _material.SetInt(_IsMakeupTex, 0);
                _material.SetVector(_ContourColor, contourColor);
                _material.SetVector(_BlushesColor, blushesColor);
                _material.SetVector(_HighlighterColor, highlighterColor);
                _material.SetVector(_EyeshadowColor, eyeshadowColor);
                _material.SetVector(_EyelinerColor, eyelinerColor);
                _material.SetVector(_LashesColor, lashesColor);
            }
        }

        private void OnEnable()
        {
            _faceController.SetActive(true);
        }

        private void OnDisable()
        {
            _faceController.SetActive(false);
        }
    }
}