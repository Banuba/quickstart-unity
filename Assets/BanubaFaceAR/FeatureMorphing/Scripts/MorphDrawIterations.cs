using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB
{
    public class MorphDrawIterations : MonoBehaviour
    {
        public FaceController face;
        public GameObject MorphPrefab;

        public RenderToTexture uv_morph;
        public RenderToTexture static_pos;

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            for (var i = 0; i < 9; ++i) {
                var prefab = GameObject.Instantiate(MorphPrefab, gameObject.transform);

                var md_component = prefab.GetComponent<MorphDraw>();

                if (md_component == null) {
                    //error
                }
                md_component.iteration = i;
                md_component.static_pos = static_pos;
                md_component.uv_morph = uv_morph;

                md_component.Init();
            }
        }

        private void Update()
        {
            transform.localScale = face.gameObject.transform.localScale;
            transform.position = face.gameObject.transform.position;
            transform.rotation = face.gameObject.transform.rotation;
        }
    }
}
