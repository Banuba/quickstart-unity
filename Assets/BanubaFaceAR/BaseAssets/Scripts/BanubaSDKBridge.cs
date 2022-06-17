using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


namespace BNB
{
    /**
 * Last argument in all calls is a referense to `bnb_error*`, may be null.
 */
    public class BanubaSDKBridge
    {
        const string pluginname =
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID
            "BNBRecognizer"
#elif UNITY_IOS || UNITY_WEBGL
            "__Internal"
#endif
            ;


        // Types

        /**
     * Pixel format
     */
        public enum bnb_pixel_format_t {
            BNB_RGB,
            BNB_RGBA,
            BNB_BGR,
            BNB_BGRA,
            BNB_ARGB
        }
        ;

        /**
     * Image orientation in degrees counterclockwise
     */
        public enum bnb_image_orientation_t {
            BNB_DEG_0,
            BNB_DEG_90,
            BNB_DEG_180,
            BNB_DEG_270
        }
        ;

        /**
     * Recognizer features id's
     */
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_recognizer_features_id_t
        {
            public ulong frx;
            public ulong ruler;
            public ulong eye_state;
            public ulong open_mouth;
            public ulong smile;
            public ulong raised_brows;
            public ulong shifted_brows;
            public ulong background_squared;
            public ulong occlusion_mask;
            public ulong action_units;
        };

        /**
     * Image format describes dimensions, orientation, horizontal flip (for selfie mode)
     * and face orientation in degrees (counterclockwise)
     */
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_image_format_t
        {
            public uint width;
            public uint height;
            public bnb_image_orientation_t orientation;
            public byte require_mirroring; // bool
            public int face_orientation;
        };

        /**
     * YUV422 image with full-sized Y plane and half-sized UV plane (width / 2, height / 2)
     */
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_yuv_422_image_t
        {
            public IntPtr y_plane;  // uint8_t*
            public IntPtr uv_plane; // uint8_t*
            public bnb_image_format_t format;
        };

        /**
     * BPC8 image with data plane and pixel format attached
     */
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_bpc8_image_t
        {
            public IntPtr data; // uint8_t*
            public bnb_image_format_t format;
            public bnb_pixel_format_t pixel_format;
        };

        /**
     * Detected face rectangle coordinates
     * @note hasFaceRectangle should be greater than 0, elsewhere face not found
     */
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_face_rectangle_t
        {
            public int hasFaceRectangle;
            public float leftTop_x;
            public float leftTop_y;
            public float rightTop_x;
            public float rightTop_y;
            public float rightBottom_x;
            public float rightBottom_y;
            public float leftBottom_x;
            public float leftBottom_y;
        };

        /**
     * Camera position describe reprojected face in face tracker scene
     */
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_camera_position_t
        {
            public int hasCameraPosition;
            public float frustum_l;
            public float frustum_r;
            public float frustum_t;
            public float frustum_b;

            /** frustrum near/far distance */
            public float frustum_n;
            public float frustum_f;

            /** model rotation in radians */
            public float model_r_x;
            public float model_r_y;
            public float model_r_z;

            /** 
         * model translation with zero point in the center of image
         * model_t_z is a distance from camera to face in millimeters
         */
            public float model_t_x;
            public float model_t_y;
            public float model_t_z;

            /** head center position */
            public float head_center_x;
            public float head_center_y;

            /** MV matrix 4x4 */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] model_view_m;

            /** P matrix 4x4 */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] projection_m;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] face_box;
        };

        /**
     * Detected face data
     */
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_face_data_t
        {
            public bnb_face_rectangle_t rectangle;
            public bnb_camera_position_t camera_position;

            /** face landmarks in format [x,y, x,y, x,y, ...]
         * landmark points count is landmarks_count / 2
         */
            public int landmarks_count;
            public IntPtr landmarks; // float*

            /** internal latents (used for face tracker only) */
            public int latents_count;
            public IntPtr latents; // float*

            /** face mesh verticies */
            public int vertices_count;
            public IntPtr vertices; // float*

            /** emotions not supported in current face tracker version */
            public int emotions_count;
            public IntPtr emotions; // float*
        };

        /**
    * Static face data
    */
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_static_face_data_t
        {
            /** face mesh verticies */
            public int vertices_count;
            public IntPtr vertices; // float*
        };

        /**
     * Action Units mapping enum
     * indices of enum elements have the same order as values in action units data array
     */
        public enum bnb_action_units_mapping_t {
            BNB_AU_BrowDownLeft, // 0
            BNB_AU_BrowDownRight,
            BNB_AU_BrowInnerUp,
            BNB_AU_BrowOuterUpLeft,
            BNB_AU_BrowOuterUpRight,
            BNB_AU_CheekPuff,
            BNB_AU_CheekSquintLeft,
            BNB_AU_CheekSquintRight,
            BNB_AU_JawForward,
            BNB_AU_JawLeft, // 9
            BNB_AU_JawRight,
            BNB_AU_JawOpen,
            BNB_AU_MouthClose,
            BNB_AU_MouthFunnel,
            BNB_AU_MouthPucker,
            BNB_AU_MouthLeft,
            BNB_AU_MouthRight,
            BNB_AU_MouthSmileLeft,
            BNB_AU_MouthSmileRight,
            BNB_AU_MouthDimpleLeft, // 19
            BNB_AU_MouthDimpleRight,
            BNB_AU_MouthRollUpper,
            BNB_AU_MouthShrugUpper,
            BNB_AU_MouthShrugLower,
            BNB_AU_MouthRollLower,
            BNB_AU_MouthFrownLeft,
            BNB_AU_MouthFrownRight,
            BNB_AU_MouthUpperUpLeft,
            BNB_AU_MouthUpperUpRight,
            BNB_AU_MouthLowerDownLeft, // 29
            BNB_AU_MouthLowerDownRight,
            BNB_AU_NoseSneerLeft,
            BNB_AU_NoseSneerRight,
            BNB_AU_MouthPressLeft,
            BNB_AU_MouthPressRight,
            BNB_AU_MouthStretchLeft,
            BNB_AU_MouthStretchRight,
            BNB_AU_EyeBlinkLeft,
            BNB_AU_EyeBlinkRight,
            BNB_AU_EyeWideLeft, // 39
            BNB_AU_EyeWideRight,
            BNB_AU_EyeSquintLeft,
            BNB_AU_EyeSquintRight,
            BNB_AU_EyeLookDownLeft,
            BNB_AU_EyeLookInLeft,
            BNB_AU_EyeLookOutLeft,
            BNB_AU_EyeLookUpLeft,
            BNB_AU_EyeLookDownRight,
            BNB_AU_EyeLookInRight,
            BNB_AU_EyeLookOutRight,
            BNB_AU_EyeLookUpRight, // 50
            BNB_AU_total_au_count = 51
        }
        ;

        /**
     * Detected Action Units
     * fields rot_x, rot_y, rot_z describes face rotation
     */
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_action_units_t
        {
            public float rot_x;
            public float rot_y;
            public float rot_z;
            public IntPtr units; // float*
        };

        public enum bnb_rect_fit_mode_t {
            bnb_fit_width,  //!< Always fit to width, rect_scale = w_t / w_f
            bnb_fit_height, //!< Always fit to height, rect_scale = h_t / h_f
            bnb_fit_inside, //!< Fit all source inside target = fit to min rect_scale of width and height modes
            bnb_fit_outside //!< Fit to fill all target = fit to max rect_scale of width and height modes
        }
        ;

        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_pixel_rect_t
        {
            public int x;
            public int y;
            public int w;
            public int h;
        };

        public enum bnb_face_search_mode_t {
            bnb_fast,
            bnb_medium,
            bnb_good
        }
        ;

        /**
     * face data transformed to viewport
     */
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_face_transform_t
        {
            /** face Model View matrix 4x4 */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] mv;

            /** Projection matrix 4x4 */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] p;

            /** View matrix 4x4 */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] v;
        };

        /**
     * bg mask common data
     */
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_segm_mask_t
        {
            public int width;
            public int height;
            public int channel;
            public int inverse; //bool

            /** common basis transform */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public float[] commonTransform;
            /** transform for gl spaces */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] glTransform;
        };

        /**
     * bg mask data
     */
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_bg_mask_data_t
        {
            public bnb_segm_mask_t commonData;
            public IntPtr data;
        };

        /**
     * bg mask data gpu
     */
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_bg_mask_data_gpu_t
        {
            public bnb_segm_mask_t commonData;
            public int textureHandle;
        };


        // API

        /**
     * Get error message.
     * @param error instance of `bnb_error*`
     * @return `char*`
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* const char* */ bnb_error_get_message(IntPtr /* const bnb_error* */ error);

        /**
     * Get error type.
     * @param error instance of `bnb_error*`
     * @return `char*`
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr /* const char* */ bnb_error_get_type(IntPtr /* const bnb_error* */ error);

        /**
     * Destroy an error object. You must always call it if error was returned.
     * @param error instance of `bnb_error*`
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_error_destroy(IntPtr /* bnb_error* */ error);


        /** 
     * Recognizer environment initialization. Should be called once, before bnb_recognizer_init
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_env_init(string client_token, out IntPtr /* bnb_error** */ error);

        /**
     * Recognizer environment deinitialization. Should be called once, when the program finishing
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_env_release(out IntPtr /* bnb_error** */ error);

        /**
     * Create recognizer instance
     * @param resource_folder path to FRX resources folder with scheme "file:///"
     * @param frx_model name of FRX model to use, can be empty for defult model
     * @param aux_dir directory name with AUX, can be empty for default
     * @param is_async UNUSED will be removed soon (TODO)
     * @param error reference to bnb_error, may be null
     * @return new recognizer instance
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bnb_recognizer_init(string resource_folder, string frx_model, string aux_dir, bool is_async, out IntPtr /* bnb_error** */ error);

        /**
     * Release recognizer instance
     * @param recognizer instance
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_release(IntPtr recognizer, out IntPtr /* bnb_error** */ error);

        /** Return struct with id's of available features */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_recognizer_features_id_t bnb_recognizer_get_features_id();

        /**
     * Enable provided features in processing pipeline
     * @param recognizer instance
     * @param features array of feature id's to enable
     * @param features_count features array size
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_set_features(IntPtr recognizer, ulong[] features, int features_count, out IntPtr /* bnb_error** */ error);

        /**
     * Enable provided features in processing pipeline
     * @param recognizer instance
     * @param features array of feature id's to enable
     * @param features_count features array size
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_insert_feature(IntPtr recognizer, ulong feature, out IntPtr /* bnb_error** */ error);


        /**
    * Disable provided features in processing pipeline
    * @param recognizer instance
    * @param features array of feature id's to enable
    * @param features_count features array size
    */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_remove_feature(IntPtr recognizer, ulong feature, out IntPtr /* bnb_error** */ error);

        /**
     * Set maximum count of faces to detect. 1 by default
     * @param recognizer instance
     * @param count of faces to detect
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_set_max_faces(IntPtr recognizer, int count, out IntPtr /* bnb_error** */ error);

        /**
     * Set offline mode. In this mode previous frames will not consider
     * @param recognizer instance
     * @param on is enable
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_set_offline_mode(IntPtr recognizer, bool on, out IntPtr /* bnb_error** */ error);

        /**
     * Process frame with configured features
     * @param recognizer instance
     * @param frame_data instance
     * @return true if no error
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_recognizer_process_frame_data(IntPtr recognizer, IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Set alrogithm for face search
     * @param recognizer instance
     * @param frame_data instance
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_set_face_search_mode(IntPtr recognizer, bnb_face_search_mode_t mode, out IntPtr /* bnb_error** */ error);

        /**
     * share egl context from unity render thread
     * @param recognizer instance
     * @param context width
     * @param context height
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_surface_created(IntPtr recognizer, int width, int height, out IntPtr /* bnb_error** */ error);

        /**
     * destroy shared egl context
     * @param recognizer instance
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_surface_destroyed(IntPtr recognizer, out IntPtr /* bnb_error** */ error);


        /**
     * Create frame data instance. This object contains image to process and processing result
     * @note frame data object should be new on each frame processing
     * @return frame data instance
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bnb_frame_data_init(out IntPtr /* bnb_error** */ error);

        /**
     * Release frame data instance
     * @param frame_data instance
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_frame_data_release(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Set YUV422 image to process
     * @param frame_data instance
     * @param image YUV422 image
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_frame_data_set_yuv_img(IntPtr frame_data, ref bnb_yuv_422_image_t image, out IntPtr /* bnb_error** */ error);

        /**
     * Set BPC8 image to process
     * @param frame_data instance
     * @param image BPC8 image
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_frame_data_set_bpc8_img(IntPtr frame_data, ref bnb_bpc8_image_t image, out IntPtr /* bnb_error** */ error);

        /**
     * Check if recognition result exist
     * @param frame_data instance
     * @return true when face recognition result exist in frame_data instance
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_frame_data_has_frx_result(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Get face transformed to viewport
     * @param frame_data instance
     * @param face_index index of face
     * @param width viewport width
     * @param height viewport height
     * @param mode fitting mode to adjust viewport aspect ratio
     * @return face data transformed to viewport
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_face_transform_t bnb_frame_data_get_face_transform(IntPtr frame_data, int face_index, int width, int height, bnb_rect_fit_mode_t mode, out IntPtr /* bnb_error** */ error);

        /**
     * Adjust rects to have the same aspect ratio as if fitting source_rect into target_rect according to mode.
     * @return source_rect, target_rect have the same (+-rounding) aspect ratio,
     * and are always not exceeding corresponding input rect, preserving original centers.
     * May do some per-axis scale adjustments within small margin for fast, integral and/or pixel-perfect scaling between pixel rects
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_pixel_rect_t bnb_fit_rects_aspect_ratio(bnb_pixel_rect_t source_rect, bnb_pixel_rect_t target_rect, bnb_rect_fit_mode_t mode, out IntPtr /* bnb_error** */ error);

        /**
     * Get detected face count
     * @param frame_data instance
     * @return detected face count
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bnb_frame_data_get_face_count(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Get detected face data
     * @param frame_data instance
     * @param face_index index of face
     * @return face data structure
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_face_data_t bnb_frame_data_get_face(IntPtr frame_data, int face_index, out IntPtr /* bnb_error** */ error);

        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_static_face_data_t get_static_pos_data();

        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_static_uv_size();

        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern void get_static_uv(IntPtr data);

        /**
     * Get texture coordinates array size
     * @param frame_data instance
     * @return texture coordinates size
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bnb_frame_data_get_tex_coords_size(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Get texture coordinates array
     * @param frame_data instance
     * @return texture coordinates array of floats
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern /* float* */ IntPtr bnb_frame_data_get_tex_coords(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Get triangles array size
     * @param frame_data instance
     * @return triangles array size
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bnb_frame_data_get_triangles_size(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Get triangles array
     * @param frame_data instance
     * @return triangles array
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern /* int* */ IntPtr bnb_frame_data_get_triangles(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Get wire indicies array size
     * @param frame_data instance
     * @return wire indicies array size
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bnb_frame_data_get_wire_indicies_size(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Get wire indicies array
     * @param frame_data instance
     * @return wire indicies array
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern /* int* */ IntPtr bnb_frame_data_get_wire_indicies(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Check if action units exist
     * @param frame_data instance
     * @return true if action units exists in frame_data instance
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_frame_data_has_action_units(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Get action units data
     * @param frame_data instance
     * @return action units structure
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_action_units_t bnb_frame_data_get_action_units(IntPtr frame_data, int face_index, out IntPtr /* bnb_error** */ error);

        /**
     * Check if mouth open on first face
     * @param frame_data instance
     * @return true if mouth open
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_frame_data_get_is_mouth_open(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Check if smile on first face
     * @param frame_data instance
     * @return true if smile
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_frame_data_get_is_smile(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Check if brows raised on first face
     * @param frame_data instance
     * @return true if brows raised
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_frame_data_get_is_brows_raised(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
     * Check if brows shifted on first face
     * @param frame_data instance
     * @return true if brows shifted
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_frame_data_get_is_brows_shifted(IntPtr frame_data, out IntPtr /* bnb_error** */ error);
        /**
     * Get background mask
     * @param frame_data instance
     * @return bnb_bg_mask_data_t struct
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_bg_mask_data_t bnb_frame_data_get_background_mask_data(IntPtr frame_data, out IntPtr /* bnb_error** */ error);
        /**
     * Get background mask for android devices on gpu runner
     * @param frame_data instance
     * @return bnb_bg_mask_data_gpu_t struct
     */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_bg_mask_data_gpu_t bnb_frame_data_get_background_mask_data_gpu(IntPtr frame_data, out IntPtr /* bnb_error** */ error);

        /**
    * check is bg feature use gpu runner
    * @return bool
    */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_background_use_gpu(out IntPtr /* bnb_error** */ error);

        /**
    * check is sdk use gpu features 
    * @return bool
    */
        [DllImport(pluginname, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_use_gpu_features(out IntPtr /* bnb_error** */ error);
    }

}