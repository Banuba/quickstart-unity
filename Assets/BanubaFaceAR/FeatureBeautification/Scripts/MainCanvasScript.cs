using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BNB
{
    public class MainCanvasScript : MonoBehaviour
    {
        public RenderTexture renderTarget;
        public Camera renderTargetCamera;

        private RawImage image => GetComponent<RawImage>();
        private PlaneController сameraPlane;
        private RectTransform planeRectTransform;

        private void Start()
        {
            //create render target with any size
            сameraPlane = GameObject.Find("CameraPlane").GetComponent<PlaneController>();
            planeRectTransform = сameraPlane.GetComponent<RectTransform>();
            createRenderTarget(100, 100);
        }


        // Update is called once per frame
        void Update()
        {
            if (renderTarget != null && (renderTarget.width != planeRectTransform.sizeDelta.x || renderTarget.height != planeRectTransform.sizeDelta.y)) {
                image.texture = null;
                UnityEngine.GameObject.Destroy(renderTarget);
                createRenderTarget((int) planeRectTransform.sizeDelta.x, (int) planeRectTransform.sizeDelta.y);
            }
        }
        private void createRenderTarget(int width, int height)
        {
            if (сameraPlane.cameraAngle == 90 || сameraPlane.cameraAngle == 270) {
                renderTarget = new RenderTexture(height, width, 0);
            } else {
                renderTarget = new RenderTexture(width, height, 0);
            }
            renderTarget.antiAliasing = 2;
            renderTarget.wrapMode = TextureWrapMode.Clamp;
            renderTarget.filterMode = FilterMode.Bilinear;

            renderTargetCamera.targetTexture = renderTarget;
            image.texture = renderTarget;
        }
    }
}
