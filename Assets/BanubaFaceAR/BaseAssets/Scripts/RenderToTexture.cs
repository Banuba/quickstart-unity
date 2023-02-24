using UnityEngine;

namespace BNB
{
    [RequireComponent(typeof(Camera))]
    public class RenderToTexture : MonoBehaviour
    {
        public RenderTexture texture;
        public RenderToTexture external;

        [SerializeField]
        private int _depth;
        [SerializeField]
        private int _msaa = 1;
        [SerializeField]
        private bool _screenSize = true;
        [SerializeField]
        private Vector2 _size = Vector2.one;
        [SerializeField]
        private RenderTextureFormat _format;
        [SerializeField]
        private PlaneController _сameraPlane;

        private Camera _camera;
        private RectTransform _planeRectTransform;

        private void Start()
        {
            _camera = GetComponent<Camera>();

            if (external) {
                _camera.targetTexture = external.texture;
                return;
            }
            texture = _screenSize
                          ? new RenderTexture(Screen.width, Screen.height, _depth, _format)
                          : new RenderTexture((int) _size.x, (int) _size.y, _depth, _format);

            _camera.targetTexture = texture;

            if (_сameraPlane != null) {
                _planeRectTransform = _сameraPlane.GetComponent<RectTransform>();
                UpdateRTSize();
            }
        }

        private void Update()
        {
            if (external) {
                _camera.targetTexture = external.texture;
                return;
            }
            if (_сameraPlane != null) {
                UpdateRTSize();
            }
        }

        private void UpdateRTSize()
        {
            if (external) {
                return;
            }

            var size = _planeRectTransform.rect.size;
            if (texture != null && (texture.width != size.x || texture.height != size.y)) {
                _camera.targetTexture = null;
                Destroy(texture);
                CreateRenderTarget((int) _planeRectTransform.sizeDelta.x, (int) size.y);
            }
        }

        private void OnDestroy()
        {
            if (external) {
                return;
            }
            Destroy(texture);
        }

        private void CreateRenderTarget(int width, int height)
        {
            texture = _сameraPlane.cameraAngle == 90 || _сameraPlane.cameraAngle == 270
                          ? new RenderTexture(height, width, 0)
                          : new RenderTexture(width, height, 0);
            texture.antiAliasing = _msaa;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.format = _format;
            texture.depth = _depth;

            _camera.targetTexture = texture;
        }

        public void SetRenderTargetSize(int w, int h)
        {
            Destroy(texture);
            texture = new RenderTexture(w, h, _depth, _format);
            _camera.targetTexture = texture;
        }
    }
}
