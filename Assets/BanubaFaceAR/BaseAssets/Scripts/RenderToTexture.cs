using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNB
{
    [RequireComponent(typeof(Camera))]
    public class RenderToTexture : MonoBehaviour
    {
        //[HideInInspector]
        public RenderTexture texture;

        public RenderToTexture external;

        [SerializeField]
        private Vector2 size = Vector2.one;
        [SerializeField]
        private bool screenSize = true;
        [SerializeField]
        private PlaneController сameraPlane;
        private RectTransform planeRectTransform;
        [SerializeField]
        private RenderTextureFormat format;
        [SerializeField]
        private int depth;
        [SerializeField]
        private int msaa = 1;


        public event Action<bool> onStateChanged;

        void Start()
        {
            if (external) {
                GetComponent<Camera>().targetTexture = external.texture;
                return;
            }
            if (screenSize) {
                texture = new RenderTexture(Screen.width, Screen.height, depth, format);
            } else {
                texture = new RenderTexture((int) size.x, (int) size.y, depth, format);
            }

            GetComponent<Camera>().targetTexture = texture;

            if (сameraPlane != null) {
                planeRectTransform = сameraPlane.GetComponent<RectTransform>();
                UpdateRTSize();
            }
        }

        protected void UpdateRTSize()
        {
            if (external) {
                return;
            }
            if (texture != null && (texture.width != planeRectTransform.sizeDelta.x || texture.height != planeRectTransform.sizeDelta.y)) {
                GetComponent<Camera>().targetTexture = null;
                UnityEngine.GameObject.Destroy(texture);
                createRenderTarget((int) planeRectTransform.sizeDelta.x, (int) planeRectTransform.sizeDelta.y);
            }
        }

        protected void OnEnable()
        {
            if (onStateChanged != null) {
                onStateChanged(true);
            }
        }

        protected void OnDisable()
        {
            if (onStateChanged != null) {
                onStateChanged(false);
            }
        }


        // Update is called once per frame
        void Update()
        {
            if (external) {
                GetComponent<Camera>().targetTexture = external.texture;
                return;
            }
            if (сameraPlane != null) {
                UpdateRTSize();
            }
        }

        private void OnDestroy()
        {
            if (external) {
                return;
            }
            UnityEngine.GameObject.Destroy(texture);
        }

        private void createRenderTarget(int width, int height)
        {
            if (сameraPlane.cameraAngle == 90 || сameraPlane.cameraAngle == 270) {
                texture = new RenderTexture(height, width, 0);
            } else {
                texture = new RenderTexture(width, height, 0);
            }
            texture.antiAliasing = msaa;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.format = format;
            texture.depth = depth;

            GetComponent<Camera>().targetTexture = texture;
        }

        public void setRenderTargetSize(int w, int h)
        {
            UnityEngine.GameObject.Destroy(texture);
            texture = new RenderTexture(w, h, depth, format);
            GetComponent<Camera>().targetTexture = texture;
        }
    }
}
