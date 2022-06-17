using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB
{
    public class MorphingController : MonoBehaviour
    {
        GameObject[] MorphComponents;
        public FacesController facesController;

        // Start is called before the first frame update
        void Start()
        {
            MorphComponents = new GameObject[1];
            MorphComponents[0] = GameObject.Find("Morph0");

            facesController.onInstanciateFace += InstantiateMorphComponent;
        }

        void InstantiateMorphComponent(int face_count)
        {
            var len = MorphComponents.Length;
            if (len < face_count) {
                Array.Resize(ref MorphComponents, face_count);
                for (int i = len; i < face_count; ++i) {
                    MorphComponents[i] = GameObject.Instantiate(MorphComponents[0], transform);
                    MorphComponents[i]
                        .SetActive(true);
                    MorphComponents[i].name = "Morph" + i;
                    var morphController = MorphComponents[i].GetComponent<Morph>();
                    var faceController = GameObject.Find("Face" + i).GetComponent<FaceController>();
                    if (faceController != null) {
                        morphController.faceController = faceController;
                    }
                    var faceMesh = faceController.gameObject.transform.GetChild(0).GetComponent<MeshFilter>();
                    if (faceController != null) {
                        morphController.faceMesh = faceMesh;
                    }
                }
            }
        }
    }
}
