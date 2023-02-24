using UnityEngine;

namespace BNB.Morphing
{
    public class MorphingController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Morph _morphPrefab;

        private FacesController _facesController;
        private MorphDraw _morphShape;

        public void Initialize(FacesController facesController, MorphDraw morphShape)
        {
            _morphShape = morphShape;
            _facesController = facesController;
            _facesController.onInstantiateFace += OnFaceInstantiatedHandler;
            CreateMorph(0);
        }

        private void CreateMorph(int faceIndex)
        {
            Morph newMorph = Instantiate(_morphPrefab, transform);
            newMorph.name = "Morph" + faceIndex;
            FaceController face = _facesController.GetFace(faceIndex);
            if (face != null) {
                newMorph.Initialize(face, _morphShape);
            }
        }

        private void OnFaceInstantiatedHandler(int faceCount)
        {
            int morphsCount = transform.childCount;
            if (morphsCount == faceCount) {
                return;
            }
            if (morphsCount < faceCount) {
                for (int i = morphsCount; i < faceCount; i++) {
                    CreateMorph(i);
                }
            } else if (morphsCount > faceCount) {
                for (int i = morphsCount; i > faceCount; i--) {
                    Destroy(transform.GetChild(morphsCount).gameObject);
                }
            }
        }
    }
}
