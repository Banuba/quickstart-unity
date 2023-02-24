using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB.ActionUnits
{
    public abstract class ActionUnits : MonoBehaviour
    {
        protected float[] _actionUnits;
        private int _faceIndex;

        protected abstract void UpdateModel(float[] actionUnits);

        private void Start()
        {
            // set actionunits feature for Groot
            var featuresId = BanubaSDKBridge.bnb_recognizer_get_features_id();

            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_insert_feature(BanubaSDKManager.instance.Recognizer, featuresId.action_units, out error);
            Utils.CheckError(error);

            _actionUnits = new float[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_total_au_count];

            var faceController = gameObject.GetComponentInParent<FaceController>();
            faceController.Initialize(_faceIndex);

            BanubaSDKManager.instance.onRecognitionResult += OnRecognitionResult;
        }

        private void OnRecognitionResult(FrameData frameData)
        {
            var error = IntPtr.Zero;
            var hasActionUnits = BanubaSDKBridge.bnb_frame_data_has_action_units(frameData, out error);
            Utils.CheckError(error);
            if (!hasActionUnits) {
                return;
            }

            var face = BanubaSDKBridge.bnb_frame_data_get_face(frameData, _faceIndex, out error);
            Utils.CheckError(error);
            if (face.rectangle.hasFaceRectangle == 0) {
                return;
            }

            var au = BanubaSDKBridge.bnb_frame_data_get_action_units(frameData, _faceIndex, out error);
            Utils.CheckError(error);

            Marshal.Copy(au.units, _actionUnits, 0, _actionUnits.Length);
            UpdateModel(_actionUnits);
        }
    }
}