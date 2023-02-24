using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BNB.Beautification
{
    public class UIScript : MonoBehaviour
    {
        private List<FaceMeshControllerBeauty> _faceControllers;
        private Dictionary<string, string> _onCoeffChangedDict = new Dictionary<string, string> {
            { "SoftSkinCoeff", "_SkinSoftIntensity" },
            { "SharpedEyesCoeff", "_EyesSharpenIntensity" },
            { "SharpedTeethCoeff", "_TeethSharpenIntensity" },
            { "WhiteTeethCoeff", "_TeethWhiteningCoeff" },
            { "WhiteEyesCoeff", "_EyesWhiteningCoeff" },
        };
        private Dictionary<string, string> _onToggleDict = new Dictionary<string, string> {
            { "MakeUpToggle", "_EnableMakeup" },
            { "BlushToggle", "_EnableEyesBlush" },
            { "SoftSkinToggle", "_EnableSoftSkin" },
            { "SharpedEyesToggle", "_EnableSharpenEyes" },
            { "SharpedTeethToggle", "_EnableSharpenTeeth" },
            { "WhiteTeethToggle", "_EnableLookUpTeeth" },
            { "WhiteEyesToggle", "_EnableLookUpEyes" },
            { "EyesFlaresToggle", "_EnableEyesFlare" },
        };

        [SerializeField]
        private FacesController _facesController;

        public void SetEnabled()
        {
            enabled = !enabled;
            gameObject.SetActive(enabled);
        }

        public void OnToggle(bool val)
        {
            var toggleID = EventSystem.current.currentSelectedGameObject.name;
            foreach (Transform child in _facesController.transform) {
                child.gameObject.transform.GetChild(0).GetComponent<FaceMeshControllerBeauty>().SetBeautyFeatureEnabled(_onToggleDict[toggleID], val ? 1 : 0);
            }
        }

        public void OnSlider(float val)
        {
            var slideID = EventSystem.current.currentSelectedGameObject.name;
            foreach (Transform child in _facesController.transform) {
                child.gameObject.transform.GetChild(0).GetComponent<FaceMeshControllerBeauty>().SetCoeffParam(_onCoeffChangedDict[slideID], val);
            }
        }
    }

}