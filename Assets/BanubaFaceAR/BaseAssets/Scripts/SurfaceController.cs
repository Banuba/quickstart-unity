using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BNB
{
    public class SurfaceController : MonoBehaviour
    {
        void Start()
        {
        }

        void Update()
        {
            var canvas = GetComponent<Canvas>();
            canvas.planeDistance = canvas.worldCamera.farClipPlane - 50F; // -50F to avoid Z-fighting
        }
    }

}
