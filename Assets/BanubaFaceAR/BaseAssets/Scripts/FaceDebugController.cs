using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB
{
    public class FaceDebugController : MonoBehaviour
    {
        FrameData frameData = null;

        // Start is called before the first frame update
        void Start()
        {
            BanubaSDKManager.instance.onRecognitionResult += onRecognitionResult;
        }

        void onRecognitionResult(FrameData frameData)
        {
            var error = IntPtr.Zero;
            if (!BanubaSDKBridge.bnb_frame_data_has_frx_result(frameData, out error))
                this.frameData = null;
            else
                this.frameData = frameData;
        }

        void OnGUI()
        {
            var error = IntPtr.Zero;
            if (frameData == null)
                return;

            var face_count = BanubaSDKBridge.bnb_frame_data_get_face_count(frameData, out error);
            Utils.CheckError(error);
            for (int index = 0; index < face_count; index++) {
                var face = BanubaSDKBridge.bnb_frame_data_get_face(frameData, index, out error);
                Utils.CheckError(error);
                if (face.rectangle.hasFaceRectangle == 0)
                    continue;

                var mvp = BanubaSDKBridge.bnb_frame_data_get_face_transform(
                    frameData, index, Screen.width, Screen.height, BanubaSDKBridge.bnb_rect_fit_mode_t.bnb_fit_height, out error);
                Utils.CheckError(error);
                var view = Utils.ArrayToMatrix4x4(mvp.v);

                DrawFaceRect(face.rectangle, view);
                DrawLandmarks(face.landmarks, face.landmarks_count, view);
            }
        }

        void DrawFaceRect(BanubaSDKBridge.bnb_face_rectangle_t rect, Matrix4x4 view)
        {
            var lt = view.MultiplyPoint(new Vector3(rect.leftTop_x, rect.leftTop_y, 1F));
            var lb = view.MultiplyPoint(new Vector3(rect.leftBottom_x, rect.leftBottom_y, 1F));

            var rt = view.MultiplyPoint(new Vector3(rect.rightTop_x, rect.rightTop_y, 1F));
            var rb = view.MultiplyPoint(new Vector3(rect.rightBottom_x, rect.rightBottom_y, 1F));

            GL.Color(Color.red);
            GL.Begin(GL.LINES);
            {
                GL.Vertex3(lt.x, lt.y, lt.z);
                GL.Vertex3(rt.x, rt.y, rt.z);

                GL.Vertex3(rt.x, rt.y, rt.z);
                GL.Vertex3(rb.x, rb.y, rb.z);

                GL.Vertex3(rb.x, rb.y, rb.z);
                GL.Vertex3(lb.x, lb.y, lb.z);

                GL.Vertex3(lb.x, lb.y, lb.z);
                GL.Vertex3(lt.x, lt.y, lt.z);
            }
            GL.End();
        }

        void DrawLandmarks(IntPtr lnd_ptr, int landmarksCount, Matrix4x4 view)
        {
            var lnd_array = new float[landmarksCount];
            Marshal.Copy(lnd_ptr, lnd_array, 0, lnd_array.Length);

            var size = 5;

            GL.Color(Color.cyan);
            GL.Begin(GL.QUADS);
            for (int i = 0; i < lnd_array.Length; i += 2) {
                var p = view.MultiplyPoint(new Vector3(lnd_array[i], lnd_array[i + 1], 1F));

                GL.Vertex3(p.x, p.y, p.z);
                GL.Vertex3(p.x + size, p.y, p.z);
                GL.Vertex3(p.x + size, p.y + size, p.z);
                GL.Vertex3(p.x, p.y + size, p.z);
            }
            GL.End();
        }
    }

}
