using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNB
{
    public class Morph : MonoBehaviour
    {
        public MeshFilter faceMesh;
        public FaceController faceController;
        // Start is called before the first frame update
        void Start()
        {
            var camerasObj = gameObject.transform.Find("MorphingCameras");
            var uvCam = GameObject.Find("MorphingUVCamera").gameObject.GetComponent<RenderToTexture>();
            var staticCam = camerasObj.transform.Find("StaticPosCamera").gameObject.GetComponent<RenderToTexture>();


            var mdi = GetComponentInChildren<MorphDrawIterations>();
            mdi.uv_morph = uvCam;
            mdi.static_pos = staticCam;
            mdi.face = faceController;

            GetComponentInChildren<MorphingUVDraw>().face = faceMesh;
        }
    }
}
