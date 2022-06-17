using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB
{
    public class GrootController : ActionUints
    {
        protected override void UpdateModel(float[] actionUnits)
        {
            var error = IntPtr.Zero;

            var groot = gameObject;

            var headobj = groot.transform.Find("Head");
            var headSkinnedMeshRenderer = headobj.GetComponent<SkinnedMeshRenderer>();
            var headBlendShapeCount = headSkinnedMeshRenderer.sharedMesh.blendShapeCount;

            var teethobj = groot.transform.Find("teeth");
            var teethSkinnedMeshRenderer = teethobj.GetComponent<SkinnedMeshRenderer>();
            var teethBlendShapeCount = teethSkinnedMeshRenderer.sharedMesh.blendShapeCount;

            {
                int i = 0;
                int j = 0;
                while (i < headBlendShapeCount) {
                    // groot model have no this blendshapes
                    if (j == (int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthDimpleLeft || j == (int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthDimpleRight) {
                        j++;
                        continue;
                    }

                    headSkinnedMeshRenderer.SetBlendShapeWeight(i++, actionunits[j++] * 100F);
                }
            }

            if (teethBlendShapeCount > 0) {
                // teeth moves with jaw
                var teeth = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_JawOpen];
                teethSkinnedMeshRenderer.SetBlendShapeWeight(0, teeth * 100F);
            }
        }
    }

}
