using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BNB
{
    public class RenderFromTexture : MonoBehaviour
    {
        public RenderToTexture renderTexture;
        private RawImage image => GetComponent<RawImage>();

        public void setRenderTexture(RenderToTexture texture)
        {
            renderTexture = texture;
            if (renderTexture) {
                renderTexture.onStateChanged += onStateChanged;
            }
        }

        private void Start()
        {
            if (renderTexture) {
                renderTexture.onStateChanged += onStateChanged;
            }
        }

        protected void onStateChanged(bool state)
        {
            if (gameObject == null) {
                return;
            }
            gameObject.SetActive(state);
        }

        // Update is called once per frame
        void Update()
        {
            bool rtEnabled = renderTexture != null && renderTexture.gameObject.activeSelf;
            image.texture = rtEnabled ? renderTexture?.texture : null;
        }
    }
}
