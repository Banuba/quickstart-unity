using UnityEngine;

namespace BNB.Morphing
{
    // Add this to your ResultCamera and provide MorphingFeature with this component's reference to get morphing working
    public class MorphingPostEffect : MonoBehaviour
    {
        private const string _defaultMorphingShader = "BNB/PostEffects/Morphing";
        private readonly int _TexWarp = Shader.PropertyToID("_TexWarp");

        [HideInInspector]
        public Texture warpResult;
        public Shader warpShader;
        private Material _warpMaterial;
        private RenderTexture _buffer;

        private void Awake()
        {
            Reset();
            _warpMaterial = new Material(warpShader) {
                hideFlags = HideFlags.DontSave
            };
            if (!warpShader || !_warpMaterial.shader.isSupported) {
                Debug.LogError("Morphing Shader is not supported.", this);
                enabled = false;
            }
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            _warpMaterial.SetTexture(_TexWarp, warpResult);
            _buffer = RenderTexture.GetTemporary(src.width, src.height, 0);
            Graphics.Blit(src, _buffer, _warpMaterial);
            Graphics.Blit(_buffer, dest);
            RenderTexture.ReleaseTemporary(_buffer);
        }

        private void Reset()
        {
            warpShader = Shader.Find(_defaultMorphingShader);
        }
    }
}