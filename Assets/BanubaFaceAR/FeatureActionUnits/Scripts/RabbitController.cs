using UnityEngine;

namespace BNB.ActionUnits
{
    public class RabbitController : ActionUnits
    {
        private SkinnedMeshRenderer _headSkinnedMeshRenderer;
        private SkinnedMeshRenderer _eyelashesSkinnedMeshRenderer;
        private SkinnedMeshRenderer _mouthSkinnedMeshRenderer;

        private void Awake()
        {
            var rabbit = gameObject;
            var headobj = rabbit.transform.Find("Head");
            _headSkinnedMeshRenderer = headobj.GetComponent<SkinnedMeshRenderer>();
            var eyelashesobj = rabbit.transform.Find("EyeLashes");
            _eyelashesSkinnedMeshRenderer = eyelashesobj.GetComponent<SkinnedMeshRenderer>();
            var mouthobj = rabbit.transform.Find("Mouth");
            _mouthSkinnedMeshRenderer = mouthobj.GetComponent<SkinnedMeshRenderer>();
        }

        protected override void UpdateModel(float[] actionUnits)
        {
            var headBlendShapeCount = _headSkinnedMeshRenderer.sharedMesh.blendShapeCount;
            var eyelashesBlendShapeCount = _eyelashesSkinnedMeshRenderer.sharedMesh.blendShapeCount;
            var mouthBlendShapeCount = _mouthSkinnedMeshRenderer.sharedMesh.blendShapeCount;

            var browDownRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowDownRight];
            var browDownLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowDownLeft];
            var browInnerUpWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowInnerUp];
            var browOuterUpRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowOuterUpRight];
            var browOuterUpLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowOuterUpLeft];
            var cheekPuffWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_CheekPuff];
            var cheekSquintRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_CheekSquintRight];
            var cheekSquintLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_CheekSquintLeft];
            var jawOpenWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_JawOpen];
            var jawLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_JawLeft];
            var jawRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_JawRight];
            var mouthFunnelWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthFunnel];
            var mouthPuckerWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthPucker];
            var mouthRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthRight];
            var mouthLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthLeft];
            var mouthSmileRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthSmileRight];
            var mouthSmileLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthSmileLeft];
            var mouthDimpleRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthDimpleRight];
            var mouthDimpleLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthDimpleLeft];
            var mouthRollUpperWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthRollUpper]; // unused
            var mouthShrugUpperWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthShrugUpper];
            var mouthShrugLowerWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthShrugLower];
            var mouthRollLowerWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthRollLower]; // usused
            var mouthFrownRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthFrownRight];
            var mouthFrownLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthFrownLeft];
            var mouthUpperUpRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthUpperUpRight];     // unused
            var mouthUpperUpLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthUpperUpLeft];       // unused
            var mouthLowerDownRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthLowerDownRight]; // unused
            var mouthLowerDownLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthLowerDownLeft];   // unused
            var noseSneerRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_NoseSneerRight];
            var noseSneerLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_NoseSneerLeft];
            var mouthPressRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthPressRight];
            var mouthPressLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthPressLeft];
            var mouthStretchRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthStretchRight];
            var mouthStretchLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthStretchLeft];
            var eyeBlinkRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeBlinkRight];
            var eyeBlinkLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeBlinkLeft];
            var eyeWideRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeWideRight];
            var eyeWideLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeWideLeft];
            var eyeSquintRightWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeSquintRight];
            var eyeSquintLeftWeight = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeSquintLeft];

            for (int i = 0; i < headBlendShapeCount; i++) {
                _headSkinnedMeshRenderer.SetBlendShapeWeight(0, browDownRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(1, browDownLeftWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(2, browInnerUpWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(3, browOuterUpRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(4, browOuterUpLeftWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(5, cheekPuffWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(6, cheekSquintRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(7, cheekSquintLeftWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(8, jawRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(9, jawLeftWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(10, jawOpenWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(11, mouthFunnelWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(12, mouthPuckerWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(13, mouthRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(14, mouthLeftWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(15, mouthSmileRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(16, mouthSmileLeftWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(17, mouthDimpleRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(18, mouthDimpleLeftWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(19, mouthShrugUpperWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(20, mouthShrugLowerWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(21, mouthFrownRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(22, mouthFrownLeftWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(23, noseSneerRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(24, noseSneerLeftWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(25, mouthPressRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(26, mouthPressLeftWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(27, mouthStretchRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(28, mouthStretchLeftWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(29, eyeBlinkRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(30, eyeBlinkLeftWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(31, eyeWideRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(32, eyeWideLeftWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(33, eyeSquintRightWeight * 100F);
                _headSkinnedMeshRenderer.SetBlendShapeWeight(34, eyeSquintLeftWeight * 100F);
            }

            if (mouthBlendShapeCount > 0) {
                _mouthSkinnedMeshRenderer.SetBlendShapeWeight(0, jawRightWeight * 100F);
                _mouthSkinnedMeshRenderer.SetBlendShapeWeight(1, jawLeftWeight * 100F);
                _mouthSkinnedMeshRenderer.SetBlendShapeWeight(2, jawOpenWeight * 100F);
            }

            if (eyelashesBlendShapeCount > 0) {
                _eyelashesSkinnedMeshRenderer.SetBlendShapeWeight(0, browDownRightWeight * 100F);
                _eyelashesSkinnedMeshRenderer.SetBlendShapeWeight(1, browDownLeftWeight * 100F);
                _eyelashesSkinnedMeshRenderer.SetBlendShapeWeight(2, eyeBlinkRightWeight * 100F);
                _eyelashesSkinnedMeshRenderer.SetBlendShapeWeight(3, eyeBlinkLeftWeight * 100F);
                _eyelashesSkinnedMeshRenderer.SetBlendShapeWeight(4, eyeWideRightWeight * 100F);
                _eyelashesSkinnedMeshRenderer.SetBlendShapeWeight(5, eyeWideLeftWeight * 100F);
            }
        }
    }

}
