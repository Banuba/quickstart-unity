using UnityEngine;

namespace BNB.Makeup
{
    public class MakeupAPI : MonoBehaviour
    {
        public LipsMakeup Lips { get; private set; }
        public EyeFaceMakeup EyeFace { get; private set; }
        public BrowMakeup Brows { get; private set; }
        public SkinMakeup Skin { get; private set; }

        private PlaneController _planeController;

        private void Awake()
        {
            Lips = GetComponent<LipsMakeup>();
            EyeFace = GetComponent<EyeFaceMakeup>();
            Brows = GetComponent<BrowMakeup>();
            Skin = GetComponent<SkinMakeup>();
        }
    }
}