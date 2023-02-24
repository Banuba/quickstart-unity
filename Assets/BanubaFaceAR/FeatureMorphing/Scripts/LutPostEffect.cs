using UnityEngine;

namespace BNB.Morphing
{
    public class LutPostEffect : MonoBehaviour
    {
        [SerializeField]
        private Material _lutMaterial;
        private RenderTexture _buffer;

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            _buffer = RenderTexture.GetTemporary(src.width, src.height, 0);
            Graphics.Blit(src, _buffer, _lutMaterial);
            Graphics.Blit(_buffer, dest);
            RenderTexture.ReleaseTemporary(_buffer);
        }
    }
}