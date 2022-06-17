using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB
{
    public abstract class ActionUints : MonoBehaviour
    {
        protected int faceIndex;

        protected float[] actionunits;

        // Start is called before the first frame update
        void Start()
        {
            // set actionunits feature for Groot
            var featuresId = BanubaSDKBridge.bnb_recognizer_get_features_id();

            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_insert_feature(BanubaSDKManager.instance.recognizer, featuresId.action_units, out error);
            Utils.CheckError(error);

            actionunits = new float[(int) BanubaSDKBridge.bnb_action_units_mapping_t.BNB_AU_total_au_count];

            var faceController = gameObject.GetComponentInParent<FaceController>();
            faceIndex = faceController.faceIndex;

            BanubaSDKManager.instance.onRecognitionResult += onRecognitionResult;
        }

        void onRecognitionResult(FrameData frameData)
        {
            var error = IntPtr.Zero;
            var hasActionUnits = BanubaSDKBridge.bnb_frame_data_has_action_units(frameData, out error);
            Utils.CheckError(error);
            if (!hasActionUnits) {
                return;
            }

            var face = BanubaSDKBridge.bnb_frame_data_get_face(frameData, faceIndex, out error);
            Utils.CheckError(error);
            if (face.rectangle.hasFaceRectangle == 0) {
                return;
            }

            var au = BanubaSDKBridge.bnb_frame_data_get_action_units(frameData, faceIndex, out error);
            Utils.CheckError(error);

            Marshal.Copy(au.units, actionunits, 0, actionunits.Length);
            UpdateModel(actionunits);
        }
        abstract protected void UpdateModel(float[] actionUunits);
    }
}