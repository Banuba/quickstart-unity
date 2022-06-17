using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB
{
    public class RabbitController : ActionUints
    {
        protected override void UpdateModel(float[] actionUnits)
        {
            var error = IntPtr.Zero;

            var rabbit = gameObject;

            var headobj = rabbit.transform.Find("Head");
            var headSkinnedMeshRenderer = headobj.GetComponent<SkinnedMeshRenderer>();
            var headBlendShapeCount = headSkinnedMeshRenderer.sharedMesh.blendShapeCount;

            var eyelashesobj = rabbit.transform.Find("EyeLashes");
            var eyelashesSkinnedMeshRenderer = eyelashesobj.GetComponent<SkinnedMeshRenderer>();
            var eyelashesBlendShapeCount = eyelashesSkinnedMeshRenderer.sharedMesh.blendShapeCount;

            var mouthobj = rabbit.transform.Find("Mouth");
            var mouthSkinnedMeshRenderer = mouthobj.GetComponent<SkinnedMeshRenderer>();
            var mouthBlendShapeCount = mouthSkinnedMeshRenderer.sharedMesh.blendShapeCount;

            var browDownRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowDownRight];
            var browDownLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowDownLeft];
            var browInnerUpWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowInnerUp];
            var browOuterUpRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowOuterUpRight];
            var browOuterUpLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowOuterUpLeft];
            var cheekPuffWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_CheekPuff];
            var cheekSquintRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_CheekSquintRight];
            var cheekSquintLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_CheekSquintLeft];
            var jawOpenWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_JawOpen];
            var jawLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_JawLeft];
            var jawRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_JawRight];
            var mouthFunnelWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthFunnel];
            var mouthPuckerWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthPucker];
            var mouthRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthRight];
            var mouthLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthLeft];
            var mouthSmileRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthSmileRight];
            var mouthSmileLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthSmileLeft];
            var mouthDimpleRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthDimpleRight];
            var mouthDimpleLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthDimpleLeft];
            var mouthRollUpperWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthRollUpper]; // unused
            var mouthShrugUpperWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthShrugUpper];
            var mouthShrugLowerWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthShrugLower];
            var mouthRollLowerWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthRollLower]; // usused
            var mouthFrownRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthFrownRight];
            var mouthFrownLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthFrownLeft];
            var mouthUpperUpRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthUpperUpRight];     // unused
            var mouthUpperUpLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthUpperUpLeft];       // unused
            var mouthLowerDownRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthLowerDownRight]; // unused
            var mouthLowerDownLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthLowerDownLeft];   // unused
            var noseSneerRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_NoseSneerRight];
            var noseSneerLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_NoseSneerLeft];
            var mouthPressRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthPressRight];
            var mouthPressLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthPressLeft];
            var mouthStretchRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthStretchRight];
            var mouthStretchLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthStretchLeft];
            var eyeBlinkRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeBlinkRight];
            var eyeBlinkLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeBlinkLeft];
            var eyeWideRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeWideRight];
            var eyeWideLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeWideLeft];
            var eyeSquintRightWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeSquintRight];
            var eyeSquintLeftWeight = actionunits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeSquintLeft];

            for (int i = 0; i < headBlendShapeCount; i++) {
                headSkinnedMeshRenderer.SetBlendShapeWeight(0, browDownRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(1, browDownLeftWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(2, browInnerUpWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(3, browOuterUpRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(4, browOuterUpLeftWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(5, cheekPuffWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(6, cheekSquintRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(7, cheekSquintLeftWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(8, jawRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(9, jawLeftWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(10, jawOpenWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(11, mouthFunnelWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(12, mouthPuckerWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(13, mouthRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(14, mouthLeftWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(15, mouthSmileRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(16, mouthSmileLeftWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(17, mouthDimpleRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(18, mouthDimpleLeftWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(19, mouthShrugUpperWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(20, mouthShrugLowerWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(21, mouthFrownRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(22, mouthFrownLeftWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(23, noseSneerRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(24, noseSneerLeftWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(25, mouthPressRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(26, mouthPressLeftWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(27, mouthStretchRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(28, mouthStretchLeftWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(29, eyeBlinkRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(30, eyeBlinkLeftWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(31, eyeWideRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(32, eyeWideLeftWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(33, eyeSquintRightWeight * 100F);
                headSkinnedMeshRenderer.SetBlendShapeWeight(34, eyeSquintLeftWeight * 100F);
            }

            if (mouthBlendShapeCount > 0) {
                mouthSkinnedMeshRenderer.SetBlendShapeWeight(0, jawRightWeight * 100F);
                mouthSkinnedMeshRenderer.SetBlendShapeWeight(1, jawLeftWeight * 100F);
                mouthSkinnedMeshRenderer.SetBlendShapeWeight(2, jawOpenWeight * 100F);
            }

            if (eyelashesBlendShapeCount > 0) {
                eyelashesSkinnedMeshRenderer.SetBlendShapeWeight(0, browDownRightWeight * 100F);
                eyelashesSkinnedMeshRenderer.SetBlendShapeWeight(1, browDownLeftWeight * 100F);
                eyelashesSkinnedMeshRenderer.SetBlendShapeWeight(2, eyeBlinkRightWeight * 100F);
                eyelashesSkinnedMeshRenderer.SetBlendShapeWeight(3, eyeBlinkLeftWeight * 100F);
                eyelashesSkinnedMeshRenderer.SetBlendShapeWeight(4, eyeWideRightWeight * 100F);
                eyelashesSkinnedMeshRenderer.SetBlendShapeWeight(5, eyeWideLeftWeight * 100F);
            }
        }
    }

}
