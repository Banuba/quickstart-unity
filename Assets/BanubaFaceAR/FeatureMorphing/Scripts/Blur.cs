using UnityEngine;

namespace BNB.Morphing
{
    public class Blur : MonoBehaviour
    {
        public float _blurSpread = 1f;
        public int _iterations = 4;

        public Shader blurShader;
        private Material _blurMaterial;
        private RenderToTexture _rrtComponent;

        private void OnDisable()
        {
            if (_blurMaterial) {
                DestroyImmediate(_blurMaterial);
            }
        }

        private void Start()
        {
            _blurMaterial = new Material(blurShader) {
                hideFlags = HideFlags.DontSave
            };
            _rrtComponent = GetComponent<RenderToTexture>();
            // Disable if the shader can't run on the users graphics card
            if (!blurShader || !_blurMaterial.shader.isSupported) {
                enabled = false;
            }
        }

        // Performs one blur iteration.
        private void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
        {
            float off = 0.5f + iteration * _blurSpread;
            Graphics.BlitMultiTap(source, dest, _blurMaterial, new Vector2(-off, -off), new Vector2(-off, off), new Vector2(off, off), new Vector2(off, -off));
        }

        // Downsamples the texture to a quarter resolution.
        private void DownSample4x(RenderTexture source, RenderTexture dest)
        {
            float off = 1.0f;
            Graphics.BlitMultiTap(source, dest, _blurMaterial, new Vector2(-off, -off), new Vector2(-off, off), new Vector2(off, off), new Vector2(off, -off));
        }
        private void OnPostRender()
        {
            if (_rrtComponent.texture == null) {
                return;
            }
            var source = _rrtComponent.texture;
            int rtW = source.width / 4;
            int rtH = source.height / 4;
            var d = source.depth;
            var format = source.format;
            RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, d, format);

            // Copy source to the 4x4 smaller texture.
            DownSample4x(source, buffer);

            // Blur the small texture
            for (int i = 0; i < _iterations; i++) {
                RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, d, format);
                FourTapCone(buffer, buffer2, i);
                RenderTexture.ReleaseTemporary(buffer);
                buffer = buffer2;
            }
            Graphics.Blit(buffer, source);

            RenderTexture.ReleaseTemporary(buffer);
        }
    }
}
