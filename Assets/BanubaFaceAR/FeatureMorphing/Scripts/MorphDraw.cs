using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB
{
    public class MorphDraw : MonoBehaviour
    {
        public int iteration;
        public RenderToTexture uv_morph;
        public RenderToTexture static_pos;

        protected Material meshMaterial;

        ////// Start is called before the first frame update
        //void Start()
        //{
        //    Init();
        //}

        public void Init()
        {
            meshMaterial = GetComponent<Renderer>().material;
            GetComponent<Renderer>().sortingOrder = 9 - iteration;
        }
        private void OnDisable()
        {
            Destroy(this.gameObject);
        }
        private void Update()
        {
            meshMaterial.SetInt("_DrawID", iteration);
            meshMaterial.SetTexture("_UVMorphTex", uv_morph.texture);
            meshMaterial.SetTexture("_StaticPosTex", static_pos.texture);
        }
    }
}
