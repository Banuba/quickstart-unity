using System;
using UnityEngine;

namespace BNB
{
    /// <summary>
    /// This class realised triggers based on facial expressions
    /// </summary>.
    public abstract class TriggerObject : MonoBehaviour
    {
        private void Start()
        {
            BanubaSDKManager.instance.onRecognitionResult += ProcessTriggers;
            FeaturesSettings();
        }
        /// <summary>
        /// This method keeps in itself check of facial expressions.
        /// </summary>
        /// <param name="frameData"></param>
        protected void ProcessTriggers(FrameData frameData)
        {
            var error = IntPtr.Zero;
            bool isOpen = BanubaSDKBridge.bnb_frame_data_get_is_mouth_open(frameData, out error);
            Utils.CheckError(error);
            bool isSmile = BanubaSDKBridge.bnb_frame_data_get_is_smile(frameData, out error);
            Utils.CheckError(error);
            bool isBrowsRaised = BanubaSDKBridge.bnb_frame_data_get_is_brows_raised(frameData, out error);
            Utils.CheckError(error);
            bool isBrowsShifted = BanubaSDKBridge.bnb_frame_data_get_is_brows_shifted(frameData, out error);
            Utils.CheckError(error);
            if (isOpen) {
                OnMouthOpen();
            }
            if (isSmile) {
                IsSmile();
            }
            if (isBrowsRaised) {
                IsBrowsRaised();
            }
            if (isBrowsShifted) {
                IsBrowsShifted();
            }
        }
        /// <summary>
        /// This method sets up features
        /// </summary>
        public virtual void FeaturesSettings()
        {
            var featuresId = BanubaSDKBridge.bnb_recognizer_get_features_id();
            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_insert_feature(BanubaSDKManager.instance.recognizer, featuresId.open_mouth, out error);
            Utils.CheckError(error);
            BanubaSDKBridge.bnb_recognizer_insert_feature(BanubaSDKManager.instance.recognizer, featuresId.smile, out error);
            Utils.CheckError(error);
            BanubaSDKBridge.bnb_recognizer_insert_feature(BanubaSDKManager.instance.recognizer, featuresId.shifted_brows, out error);
            Utils.CheckError(error);
            BanubaSDKBridge.bnb_recognizer_insert_feature(BanubaSDKManager.instance.recognizer, featuresId.raised_brows, out error);
            Utils.CheckError(error);
        }
        /// <summary>
        /// This method do something if mouth is open
        /// </summary>
        protected virtual void OnMouthOpen()
        {
        }
        /// <summary>
        /// This method do something if smiles
        /// </summary>
        protected virtual void IsSmile()
        {
        }
        /// <summary>
        /// This method do something if brows are raised
        /// </summary>
        protected virtual void IsBrowsRaised()
        {
        }
        /// <summary>
        /// This method do something if brows are shifted
        /// </summary>
        protected virtual void IsBrowsShifted()
        {
        }
    }
}