using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace BNB
{
    public class PlaneController : MonoBehaviour
    {
        Resolution resolution;
        public int cameraAngle
        {
            get;
            private set;
        }
        public bool camVerticalFlip
        {
            get;
            private set;
        }
        private bool getRotated = false;
        private RawImage image;

        // Start is called before the first frame update
        void Start()
        {
            if (BanubaSDKManager.instance == null) {
                return;
            }
            image = GetComponent<RawImage>();

            resolution.width = Screen.width;
            resolution.height = Screen.height;

            BanubaSDKManager.instance.cameraDevice.onCameraTexture += OnCameraTexture;
            OnCameraTexture(BanubaSDKManager.instance.cameraDevice.cameraTexture, BanubaSDKManager.instance.cameraDevice.cameraTextureData);
        }
        void Update()
        {
            if (resolution.width != Screen.width || resolution.height != Screen.height) {
                Debug.Log("Resolution changed: " + Screen.width + "x" + Screen.height);
                resolution.width = Screen.width;
                resolution.height = Screen.height;
                OnCameraTexture(BanubaSDKManager.instance.cameraDevice.cameraTexture, BanubaSDKManager.instance.cameraDevice.cameraTextureData);
            }
        }

        void OnCameraTexture(Texture2D tex, CameraDevice.CameraTextureData cameraTextureData)
        {
            if (tex == null) {
                return;
            }
            image.texture = tex;
            cameraAngle = cameraTextureData.Angle;
            camVerticalFlip = false;
            transform.rotation = Quaternion.AngleAxis(cameraAngle, Vector3.back);

            UpdatePlaneRect();
        }

        protected void UpdatePlaneRect()
        {
            var w = image.texture.width;
            var h = image.texture.height;
            getRotated = cameraAngle == 90 || cameraAngle == 270;

            if (getRotated) {
                if (camVerticalFlip)
                    image.uvRect = new Rect(0, 1, 1, -1);
                else
                    image.uvRect = new Rect(0, 0, 1, 1);

                // rotate rect to correct fit on screen
                w = image.texture.height;
                h = image.texture.width;
            } else {
                image.uvRect = new Rect(0, 0, 1, 1);
            }

            var src = new BanubaSDKBridge.bnb_pixel_rect_t();
            src.x = 0;
            src.y = 0;
            src.w = w;
            src.h = h;

            var dst = new BanubaSDKBridge.bnb_pixel_rect_t();
            dst.x = 0;
            dst.y = 0;
            dst.w = Screen.width;
            dst.h = Screen.height;

            var error = IntPtr.Zero;
            var res = BanubaSDKBridge.bnb_fit_rects_aspect_ratio(src, dst, BanubaSDKBridge.bnb_rect_fit_mode_t.bnb_fit_inside, out error);
            Utils.CheckError(error);

            // compensate height, due to all fitting made by height. For full screen appearance
            var scale = (float) dst.h / res.h;

            // unrotate rect
            if (getRotated) {
                var tmp = res.w;
                res.w = res.h;
                res.h = tmp;
            }

            var rt = GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(res.w * scale, res.h * scale);
        }
    }
}
