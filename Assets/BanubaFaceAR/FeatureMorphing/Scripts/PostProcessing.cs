using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNB
{
    public class PostProcessing : MonoBehaviour
    {
        public Shader warpShader = null;
        public RenderToTexture WarpResult;

        private RenderTexture WarpRT;
        private RenderTexture LutRT;


        static Material m_Material = null;
        protected Material material
        {
            get {
                if (m_Material == null) {
                    m_Material = new Material(warpShader);
                    m_Material.hideFlags = HideFlags.DontSave;
                }
                return m_Material;
            }
        }

        public Material lutMaterial;

        protected void OnDisable()
        {
            if (m_Material) {
                DestroyImmediate(m_Material);
            }
        }

        protected RenderToTexture rtt => GetComponent<RenderToTexture>();

        protected void onStateChanged(bool state)
        {
            enabled = state;
        }

        private void OnDestroy()
        {
            UnityEngine.GameObject.Destroy(WarpRT);
            UnityEngine.GameObject.Destroy(LutRT);
        }


        // --------------------------------------------------------

        protected void Start()
        {
            // Disable if we don't support image effects
            if (!SystemInfo.supportsImageEffects) {
                enabled = false;
                return;
            }
            // Disable if the shader can't run on the users graphics card
            if (!warpShader || !material.shader.isSupported) {
                enabled = false;
                return;
            }
            if (WarpResult) {
                WarpResult.onStateChanged += onStateChanged;
            }
            WarpRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
            LutRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
        }

        protected void Update()
        {
            if (WarpResult) {
                material.SetTexture("_TexWarp", WarpResult.texture);
            }
        }

        // Called by the camera to apply the image effect
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (warpShader) {
                Graphics.Blit(source, WarpRT, material);
                if (lutMaterial) {
                    Graphics.Blit(WarpRT, LutRT, lutMaterial);
                    Graphics.Blit(LutRT, destination);
                    return;
                }
                Graphics.Blit(WarpRT, destination);
            }
            if (lutMaterial) {
                Graphics.Blit(source, LutRT, lutMaterial);
                Graphics.Blit(LutRT, destination);
            }
        }
    }
}
