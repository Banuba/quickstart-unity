using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNB
{
    public class FaceMeshControllerBeauty : FaceMeshController
    {
        public void setCoeffParam(string paramName, float val)
        {
            meshMaterial.SetFloat(paramName, val);
        }

        public void setBeautyFeatureEnabled(string paramName, int val)
        {
            meshMaterial.SetInt(paramName, val);
        }
    }
}
