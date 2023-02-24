using UnityEngine;

namespace BNB.Morphing
{
    public class Morph : MonoBehaviour
    {
        [Header("Children references")]
        [SerializeField]
        private MorphDrawIterations _MDI;
        [SerializeField]
        private MorphingUVDraw _MUD;

        public void Initialize(FaceController face, MorphDraw morphShape)
        {
            _MDI.Initialize(face, morphShape);
            _MUD.Initialize(face.GetComponentInChildren<MeshFilter>());
        }
    }
}
