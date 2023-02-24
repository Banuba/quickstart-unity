using UnityEngine;
using UnityEngine.Assertions;

namespace BNB.ActionUnits
{
    public class GrootController : ActionUnits
    {
        private SkinnedMeshRenderer _headSkinnedMeshRenderer;
        private SkinnedMeshRenderer _teethSkinnedMeshRenderer;
        private BanubaSDKBridge.bnb_action_units_mapping_t[] _headBlendShapes = {
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowDownRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowDownLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowInnerUp,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowOuterUpRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_BrowOuterUpLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_CheekPuff,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_CheekSquintRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_CheekSquintLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_JawForward,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_JawRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_JawLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_JawOpen,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthClose,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthFunnel,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthPucker,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthSmileRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthSmileLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthRollUpper,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthRollLower,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthShrugUpper,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthShrugLower,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthFrownRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthFrownLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthUpperUpRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthUpperUpLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthLowerDownRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthLowerDownLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_NoseSneerRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_NoseSneerLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthPressRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthPressLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthStretchRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_MouthStretchLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeBlinkRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeBlinkLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeWideRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeWideLeft,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeSquintRight,
            BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_EyeSquintLeft,
        };

        private void Awake()
        {
            var groot = gameObject;
            var headobj = groot.transform.Find("Head");
            _headSkinnedMeshRenderer = headobj.GetComponent<SkinnedMeshRenderer>();
            var teethobj = groot.transform.Find("teeth");
            _teethSkinnedMeshRenderer = teethobj.GetComponent<SkinnedMeshRenderer>();
        }

        protected override void UpdateModel(float[] actionUnits)
        {
            var teethBlendShapeCount = _teethSkinnedMeshRenderer.sharedMesh.blendShapeCount;
            Assert.AreEqual(_headSkinnedMeshRenderer.sharedMesh.blendShapeCount, _headBlendShapes.Length);

            for (var i = 0; i < _headBlendShapes.Length; ++i) {
                _headSkinnedMeshRenderer.SetBlendShapeWeight(i, _actionUnits[(int) _headBlendShapes[i]] * 100F);
            }
            if (teethBlendShapeCount > 0) {
                // teeth moves with jaw
                var teeth = _actionUnits[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_JawOpen];
                _teethSkinnedMeshRenderer.SetBlendShapeWeight(0, teeth * 100F);
            }
        }
    }

}
