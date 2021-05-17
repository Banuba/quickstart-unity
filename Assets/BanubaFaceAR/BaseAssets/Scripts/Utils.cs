using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace BNB
{
    public static class Utils
    {
        public static Matrix4x4 ArrayToMatrix4x4(float[] arr)
        {
            Assert.AreEqual(arr.Length, 16);
            var mat = new Matrix4x4();
            for (int i = 0; i < 16; i += 4) {
                mat.SetColumn(i / 4, new Vector4(arr[i], arr[i + 1], arr[i + 2], arr[i + 3]));
            }
            return mat;
        }

        public static Matrix4x4 Array3x3ToMatrix4x4(float[] arr)
        {
            Assert.AreEqual(arr.Length, 9);
            var mat = new Matrix4x4();
            for (int i = 0; i < 9; i += 3) {
                mat.SetColumn(i / 3, new Vector4(arr[i], arr[i + 1], arr[i + 2], 0.0f));
            }
            mat.SetColumn(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            return mat;
        }

        /**
     * Check if error isn't null and thow an exception in other case.
     * @param error instanse of `bnb_error*`
     */
        public static void CheckError(IntPtr /* const bnb_error* */ error)
        {
            if (error != IntPtr.Zero) {
                var message = Marshal.PtrToStringAnsi(BanubaSDKBridge.bnb_error_get_message(error));
                BanubaSDKBridge.bnb_error_destroy(error);
                throw new Exception(message);
            }
        }

        public static int OrientationToAngle(BanubaSDKBridge.bnb_image_orientation_t or)
        {
            switch (or) {
                // swap for 180 and 0 due to unity Texture2D.GetPixels32 return vertically flipped image
                case BanubaSDKBridge.bnb_image_orientation_t.BNB_DEG_180:
                    return 0;
                case BanubaSDKBridge.bnb_image_orientation_t.BNB_DEG_90:
                    return 90;
                case BanubaSDKBridge.bnb_image_orientation_t.BNB_DEG_0:
                    return 180;
                case BanubaSDKBridge.bnb_image_orientation_t.BNB_DEG_270:
                    return 270;
                default:
                    return 0;
            }
        }
    }

}