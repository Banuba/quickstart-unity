using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BNB
{
    public class UIScript : MonoBehaviour
    {
        List<FaceMeshControllerBeauty> faceControllers;
        Dictionary<string, string> onCoeffChangedDict = new Dictionary<string, string>() {
            { "SoftSkinCoeff", "_SkinSoftIntensity" },
            { "SharpedEyesCoeff", "_EyesSharpenIntensity" },
            { "SharpedTeethCoeff", "_TeethSharpenIntensity" },
            { "WhiteTeethCoeff", "_TeethWhiteningCoeff" },
            { "WhiteEyesCoeff", "_EyesWhiteningCoeff" },

        };
        Dictionary<string, string> onToggleDict = new Dictionary<string, string>() {
            { "MakeUpToggle", "_EnableMakeup" },
            { "BlushToggle", "_EnableEyesBlush" },
            { "SoftSkinToggle", "_EnableSoftSkin" },
            { "SharpedEyesToggle", "_EnableSharpenEyes" },
            { "SharpedTeethToggle", "_EnableSharpenTeeth" },
            { "WhiteTeethToggle", "_EnableLookUpTeeth" },
            { "WhiteEyesToggle", "_EnableLookUpEyes" },
            { "EyesFlaresToggle", "_EnableEyesFlare" },
        };
        GameObject facesController => GameObject.Find("Faces");

        public void setEnabled()
        {
            enabled = !enabled;
            gameObject.SetActive(enabled);
        }

        public void onToggle(bool val)
        {
            var toggleID = EventSystem.current.currentSelectedGameObject.name;

            foreach (Transform child in facesController.transform) {
                child.gameObject.transform.GetChild(0).GetComponent<FaceMeshControllerBeauty>().setBeautyFeatureEnabled(onToggleDict[toggleID], val ? 1 : 0);
            }
        }

        public void onSlider(float val)
        {
            var slideID = EventSystem.current.currentSelectedGameObject.name;

            foreach (Transform child in facesController.transform) {
                child.gameObject.transform.GetChild(0).GetComponent<FaceMeshControllerBeauty>().setCoeffParam(onCoeffChangedDict[slideID], val);
            }
        }
    }

}