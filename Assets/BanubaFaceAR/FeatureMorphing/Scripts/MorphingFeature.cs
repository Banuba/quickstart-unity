using UnityEngine;

namespace BNB.Morphing
{
    // Morphing feature enabling
    // 1. Add MorphingPostEffect script to ResultCamera in EffectBase hierarchy. It will automatically set default morphing shader.
    // 2. Add MorphingFeature prefab to EffectBase’s hierarchy. Then setup MorphingShape and provide references to MorphingPostEffect and FacesController.

    public class MorphingFeature : MonoBehaviour
    {
        [SerializeField]
        private MorphDraw _morphShape;

        [Header("Required references")]
        [SerializeField]
        private FacesController _facesController;
        [SerializeField]
        private MorphingPostEffect _effectReference;

        private RenderToTexture _morphingResult;

        private void Awake()
        {
            CheckReferences();
            _morphingResult = GetComponentInChildren<RenderToTexture>();
            GetComponentInChildren<MorphingController>().Initialize(_facesController, _morphShape);
        }

        private void LateUpdate()
        {
            _effectReference.warpResult = _morphingResult.texture;
        }

        private void CheckReferences()
        {
            if (_facesController == null) {
                Debug.LogError("FacesController reference isn't provided to MorphingFeature script", this);
                return;
            }
            if (_effectReference == null) {
                Debug.LogError("MorphingPostEffect reference isn't provided to MorphingFeature script", this);
                return;
            }
            if (_morphShape == null) {
                Debug.LogError("Morph shape isn't set in MorphingFeature script", this);
            }
        }
    }
}