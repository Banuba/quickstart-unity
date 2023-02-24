using UnityEngine;

namespace BNB.Morphing
{
    public class MorphDraw : MonoBehaviour
    {
        // Cached shader properties
        private readonly int _DrawID = Shader.PropertyToID("_DrawID");
        private readonly int _UVMorphTex = Shader.PropertyToID("_UVMorphTex");
        private readonly int _StaticPosTex = Shader.PropertyToID("_StaticPosTex");

        private int _iteration;
        private RenderToTexture _uvMorph;
        private Texture _staticPos;
        private Material _meshMaterial;

        private void OnEnable()
        {
            BanubaSDKManager.instance.onRecognitionResult += UpdateMaterial;
        }

        private void OnDisable()
        {
            BanubaSDKManager.instance.onRecognitionResult -= UpdateMaterial;
            Destroy(gameObject);
        }

        public void Create(RenderToTexture uvMorph, Texture staticPos, int iteration)
        {
            _iteration = iteration;
            _uvMorph = uvMorph;
            _staticPos = staticPos;
            var rendererComponent = GetComponent<Renderer>();
            rendererComponent.sortingOrder = 9 - _iteration;
            _meshMaterial = rendererComponent.material;
        }

        private void UpdateMaterial(FrameData data)
        {
            _meshMaterial.SetInt(_DrawID, _iteration);
            _meshMaterial.SetTexture(_UVMorphTex, _uvMorph.texture);
            _meshMaterial.SetTexture(_StaticPosTex, _staticPos);
        }
    }
}
