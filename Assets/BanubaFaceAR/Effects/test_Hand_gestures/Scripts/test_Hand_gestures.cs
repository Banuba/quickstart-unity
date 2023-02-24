using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;


namespace BNB
{
    public class test_Hand_gestures : MonoBehaviour
    {
        [SerializeField]
        Camera _renderCamera;
        [SerializeField]
        LineRenderer[] _fingers;

        [SerializeField]
        LineRenderer _additional;

        [SerializeField]
        GameObject _UI_Text;

        private Text text;
        private readonly int[] _additional_points = { 2, 5, 9, 13, 17 };

        public BanubaSDKBridge.bnb_hand_gesture_t Gesture { private set; get; }


        // Start is called before the first frame update
        void Start()
        {
            BanubaSDKManager.instance.onRecognitionResult += OnRecognitionResult;
            text = _UI_Text.GetComponent<Text>();

            var error = IntPtr.Zero;
            var featuresId = BanubaSDKBridge.bnb_recognizer_get_features_id();

            BanubaSDKBridge.bnb_recognizer_insert_feature(BanubaSDKManager.instance.Recognizer, featuresId.hand_gestures, out error);
            Utils.CheckError(error);
        }

        private void OnDestroy()
        {
            BanubaSDKManager.instance.onRecognitionResult -= OnRecognitionResult;
            var featuresId = BanubaSDKBridge.bnb_recognizer_get_features_id();
            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_remove_feature(BanubaSDKManager.instance.Recognizer, featuresId.hand_gestures, out error);
            Utils.CheckError(error);
        }

        private void OnRecognitionResult(FrameData frameData)
        {
            var error = IntPtr.Zero;
            var hand = BanubaSDKBridge.bnb_frame_data_get_hand(frameData, Screen.height, Screen.height, BanubaSDKBridge.bnb_rect_fit_mode_t.bnb_fit_height, out error);
            Utils.CheckError(error);

            if (hand.vertices_count < 0) {
                foreach (var finger in _fingers) {
                    finger.enabled = false;
                }
                _additional.enabled = false;
                return;
            }

            Gesture = hand.gesture;
            text.text = Gesture.ToString().Substring(4);
            var vert_array = new float[hand.vertices_count];
            Marshal.Copy(hand.vertices, vert_array, 0, vert_array.Length);

            var vertices = new Vector3[vert_array.Length / 2];
            for (int j = 0, i = 0; j < vertices.Length; ++j, i += 2) {
                vertices[j] = new Vector3(
                    vert_array[i + 0],
                    vert_array[i + 1],
                    -1f
                );
            }
            int index = 0;
            foreach (var finger in _fingers) {
                finger.enabled = true;
                finger.positionCount = index > 4 ? 4 : 5;
                for (int i = 0; i < finger.positionCount; ++i) {
                    finger.SetPosition(i, vertices[index++]);
                }
            }

            _additional.positionCount = _additional_points.Length;
            _additional.enabled = true;
            for (var i = 0; i < _additional_points.Length; ++i) {
                _additional.SetPosition(i, vertices[_additional_points[i]]);
            }

            var mv = Utils.ArrayToMatrix4x4(hand.transform);
            var scale = mv.lossyScale;
            var pos = mv.GetColumn(3);
            var rot = mv.rotation;

            // apply correct fov effect for 2d lines
            float sx = Mathf.Tan(_renderCamera.fieldOfView / 2f * Mathf.PI / 180f);
            transform.localScale = Vector3.Scale(scale, new Vector3(sx, sx, 1));
            transform.position = new Vector3(pos.x * sx, pos.y * sx, pos.z);
            transform.rotation = rot;
        }
    }
}
