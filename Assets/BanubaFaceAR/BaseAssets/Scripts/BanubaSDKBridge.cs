using System;
using System.Runtime.InteropServices;

namespace BNB
{
    public static class BanubaSDKBridge
    {
        private const string pluginName =
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID
            "BNBRecognizer"
#elif UNITY_IOS || UNITY_WEBGL
            "__Internal"
#endif
            ;

        // TYPES

        /// <summary>
        /// Pixel format
        /// </summary>
        public enum bnb_pixel_format_t
        {
            BNB_RGB,
            BNB_RGBA,
            BNB_BGR,
            BNB_BGRA,
            BNB_ARGB
        }
        ;


        /// <summary>
        /// Image orientation in degrees counterclockwise
        /// </summary>
        public enum bnb_image_orientation_t
        {
            BNB_DEG_0,
            BNB_DEG_90,
            BNB_DEG_180,
            BNB_DEG_270
        }
        ;


        ///< summary>
        /// Recognizer features id's
        /// </summary>
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
            public ulong occlusion_mask;
            public ulong action_units;
            public ulong background_segm;
            public ulong body_segm;
            public ulong face_segm;
            public ulong face_skin_segm;
            public ulong hair_segm;
            public ulong neck_segm;
            public ulong skin_segm;
            public ulong lips_segm;
            public ulong brows_segm;
            public ulong eyes_segm;
            public ulong hand_skeleton;
            public ulong hand_gestures;
            public ulong lips_shine;
            public ulong eyes_correction;
            public ulong lips_correction;
            public ulong brows_correction;
            public ulong face_mesh_correction;
        };


        ///< summary>
        /// Image format describes dimensions, orientation, horizontal flip (for selfie mode)
        /// and face orientation in degrees (counterclockwise)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_image_format_t
        {
            public uint width;
            public uint height;
            public bnb_image_orientation_t orientation;
            public byte require_mirroring; // bool
            public int face_orientation;
        };


        ///< summary>
        /// YUV422 image with full-sized Y plane and half-sized UV plane (width / 2, height / 2)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_yuv_422_image_t
        {
            public IntPtr y_plane;  // uint8_t*
            public IntPtr uv_plane; // uint8_t*
            public bnb_image_format_t format;
        };


        ///< summary>
        /// BPC8 image with data plane and pixel format attached
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_bpc8_image_t
        {
            public IntPtr data; // uint8_t*
            public bnb_image_format_t format;
            public bnb_pixel_format_t pixel_format;
        };


        ///< summary>
        /// Detected face rectangle coordinates.
        /// Note: hasFaceRectangle should be greater than 0, elsewhere face not found
        /// </summary>
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


        ///< summary>
        /// Camera position describe reprojected face in face tracker scene
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_camera_position_t
        {
            // MV matrix 4x4
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] model_view_m;

            // P matrix 4x4
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] projection_m;
        };


        ///< summary>
        /// Detected face data
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_face_data_t
        {
            public bnb_face_rectangle_t rectangle;
            public bnb_camera_position_t camera_position;

            // face landmarks in format [x,y, x,y, x,y, ...]
            // landmark points count is landmarks_count / 2
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


        ///< summary>
        /// Static face data
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_static_face_data_t
        {
            // face mesh vertices
            public int vertices_count;
            public IntPtr vertices; // float*
        };


        ///< summary>
        /// Action Units mapping enum
        /// indices of enum elements have the same order as values in action units data array
        /// </summary>
        public enum bnb_action_units_mapping_t
        {
            BNB_AU_BrowDownLeft = 0,
            BNB_AU_BrowDownRight = 1,
            BNB_AU_BrowInnerUp = 2,
            BNB_AU_BrowOuterUpLeft = 3,
            BNB_AU_BrowOuterUpRight = 4,
            BNB_AU_CheekPuff = 5,
            BNB_AU_CheekSquintLeft = 6,
            BNB_AU_CheekSquintRight = 7,
            BNB_AU_JawForward = 8,
            BNB_AU_JawLeft = 9,
            BNB_AU_JawRight = 10,
            BNB_AU_JawOpen = 11,
            BNB_AU_MouthClose = 12,
            BNB_AU_MouthFunnel = 13,
            BNB_AU_MouthPucker = 14,
            BNB_AU_MouthLeft = 15,
            BNB_AU_MouthRight = 16,
            BNB_AU_MouthSmileLeft = 17,
            BNB_AU_MouthSmileRight = 18,
            BNB_AU_MouthDimpleLeft = 19,
            BNB_AU_MouthDimpleRight = 20,
            BNB_AU_MouthRollUpper = 21,
            BNB_AU_MouthShrugUpper = 22,
            BNB_AU_MouthShrugLower = 23,
            BNB_AU_MouthRollLower = 24,
            BNB_AU_MouthFrownLeft = 25,
            BNB_AU_MouthFrownRight = 26,
            BNB_AU_MouthUpperUpLeft = 27,
            BNB_AU_MouthUpperUpRight = 28,
            BNB_AU_MouthLowerDownLeft = 29,
            BNB_AU_MouthLowerDownRight = 30,
            BNB_AU_NoseSneerLeft = 31,
            BNB_AU_NoseSneerRight = 32,
            BNB_AU_MouthPressLeft = 33,
            BNB_AU_MouthPressRight = 34,
            BNB_AU_MouthStretchLeft = 35,
            BNB_AU_MouthStretchRight = 36,
            BNB_AU_EyeBlinkLeft = 37,
            BNB_AU_EyeBlinkRight = 38,
            BNB_AU_EyeWideLeft = 39,
            BNB_AU_EyeWideRight = 40,
            BNB_AU_EyeSquintLeft = 41,
            BNB_AU_EyeSquintRight = 42,
            BNB_AU_EyeLookDownLeft = 43,
            BNB_AU_EyeLookInLeft = 44,
            BNB_AU_EyeLookOutLeft = 45,
            BNB_AU_EyeLookUpLeft = 46,
            BNB_AU_EyeLookDownRight = 47,
            BNB_AU_EyeLookInRight = 48,
            BNB_AU_EyeLookOutRight = 49,
            BNB_AU_EyeLookUpRight = 50,
            BNB_AU_total_au_count = 51
        }
        ;

        ///< summary>
        /// Segmentation mask types
        /// </summary>
        public enum bnb_segm_type_t
        {
            BNB_BACKGROUND = 0,
            BNB_BODY,
            BNB_FACE,
            BNB_FACE_SKIN,
            BNB_HAIR,
            BNB_NECK,
            BNB_SKIN,
            BNB_LIPS,
            BNB_BROW_LEFT,
            BNB_BROW_RIGHT,
            BNB_EYE_PUPIL_LEFT,
            BNB_EYE_PUPIL_RIGHT,
            BNB_EYE_SCLERA_LEFT,
            BNB_EYE_SCLERA_RIGHT,
            BNB_EYE_IRIS_LEFT,
            BNB_EYE_IRIS_RIGHT
        }
        ;

        ///< summary>
        /// Hand gestures
        /// </summary>
        public enum bnb_hand_gesture_t
        {
            BNB_NONE = 0,
            BNB_LIKE,
            BNB_OK,
            BNB_PALM,
            BNB_ROCK,
            BNB_V
        }
        ;


        ///< summary>
        /// Detected Action Units
        /// fields rot_x, rot_y, rot_z describes face rotation
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_action_units_t
        {
            public float rot_x;
            public float rot_y;
            public float rot_z;
            public IntPtr units; // float*
        };


        public enum bnb_rect_fit_mode_t
        {
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


        ///< summary>
        /// face data transformed to viewport
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_face_transform_t
        {
            // face Model View matrix 4x4
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] mv;

            // Projection matrix 4x4
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] p;

            // View matrix 4x4
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] v;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_hand_data_t
        {
            public bnb_hand_gesture_t gesture;
            public int vertices_count;
            public IntPtr vertices;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] transform;
        };


        ///< summary>
        /// bg mask common data
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_segm_mask_t
        {
            public int width;
            public int height;
            public int channel;
            public int inverse; // bool

            // common basis transform
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public float[] commonTransform;
            // transform for gl spaces
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] glTransform;
        };


        ///< summary>
        /// bg mask data
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_segm_mask_data_t
        {
            public bnb_segm_mask_t commonData;
            public IntPtr data;
        };

        ///< summary>
        /// Lips shine mask common data
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_lips_shine_mask_t
        {
            public int width;
            public int height;
            public int channel;
            public int inverse; // bool
            public float v_min;
            public float v_max;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public float[] commonTransform;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] glTransform;
        };


        ///< summary>
        /// Lips shine mask data
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_lips_shine_mask_data_t
        {
            public bnb_lips_shine_mask_t commonData;
            public IntPtr data;
        };


        ///< summary>
        /// bg mask data gpu
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct bnb_bg_mask_data_gpu_t
        {
            public bnb_segm_mask_t commonData;
            public int textureHandle;
        };


        // API

        /// <summary>Get error message</summary>
        /// <param name="error">Error instance of bnb_error</param>
        /// <returns>char*</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bnb_error_get_message(IntPtr error);


        /// <summary>Get error type</summary>
        /// <param name="error">error instance of bnb_error*</param>
        /// <returns>char*</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bnb_error_get_type(IntPtr error);


        /// <summary>Destroy an error object. You must always call it if error was returned.</summary>
        /// <param name="error">Error instance of bnb_error*</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_error_destroy(IntPtr error);


        /// <summary>Recognizer environment initialization. Should be called once, before bnb_recognizer_init</summary>
        /// <param name="client_token">Your personal client-token</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_env_init(string client_token, out IntPtr error);


        /// <summary>Recognizer environment de-initialization. Should be called once, when the program finishing</summary>
        /// <param name="error">bnb_error**, may be NULL</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_env_release(out IntPtr error);


        /// <summary>Create recognizer instance</summary>
        /// <param name="resource_folder">Path to FRX resources folder with scheme "file:///"</param>
        /// <param name="is_async">UNUSED will be removed soon</param>
        /// <param name="error">bnb_error**, may be null</param>
        /// <returns>New recognizer instance</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bnb_recognizer_init(string resource_folder, bool is_async, out IntPtr error);


        /// <summary>Release recognizer instance</summary>
        /// <param name="recognizer">Recognizer instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_release(IntPtr recognizer, out IntPtr error);


        /// <summary>Return struct with id's of available feature</summary>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_recognizer_features_id_t bnb_recognizer_get_features_id();


        /// <summary>Enable provided features in processing pipeline</summary>
        /// <param name="recognizer">recognizer instance</param>
        /// <param name="features">array of feature id's to enable</param>
        /// <param name="features_count">features array size</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_set_features(IntPtr recognizer, ulong[] features, int features_count, out IntPtr error);


        /// <summary>Enable provided features in processing pipeline</summary>
        /// <param name="recognizer">recognizer instance</param>
        /// <param name="feature">id of feature to enable</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_insert_feature(IntPtr recognizer, ulong feature, out IntPtr error);


        /// <summary>Disable provided features in processing pipeline</summary>
        /// <param name="recognizer">recognizer instance</param>
        /// <param name="feature">id of feature to remove</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_remove_feature(IntPtr recognizer, ulong feature, out IntPtr error);


        /// <summary>Set maximum count of faces to detect. 1 by default</summary>
        /// <param name="recognizer">recognizer instance</param>
        /// <param name="count">count of faces to detect</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_set_max_faces(IntPtr recognizer, int count, out IntPtr error);


        /// <summary>Set offline mode. In this mode previous frames will not consider</summary>
        /// <param name="recognizer">recognizer instance</param>
        /// <param name="on">is enable</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_set_offline_mode(IntPtr recognizer, bool on, out IntPtr error);


        /// <summary>Process frame with configured features</summary>
        /// <param name="recognizer">recognizer instance</param>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>true if no error</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_recognizer_process_frame_data(IntPtr recognizer, IntPtr frame_data, out IntPtr error);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_push_frame_data(IntPtr recognizer, IntPtr frame_data, out IntPtr error);

        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_recognizer_pop_frame_data(IntPtr recognizer, IntPtr frame_data, out IntPtr error);


        /// <summary>Share egl context from unity render thread</summary>
        /// <param name="recognizer">recognizer instance</param>
        /// <param name="width">context width</param>
        /// <param name="height">context height</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_surface_created(IntPtr recognizer, int width, int height, out IntPtr error);


        /// <summary>Destroy shared egl context</summary>
        /// <param name="recognizer">recognizer instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_recognizer_surface_destroyed(IntPtr recognizer, out IntPtr error);


        /// <summary>
        /// Create frame data instance. This object contains image to process and processing result.
        /// Note: frame data object should be new on each frame processing
        /// </summary>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>frame data instance</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bnb_frame_data_init(out IntPtr error);


        /// <summary>Release frame data instance</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_frame_data_release(IntPtr frame_data, out IntPtr error);


        /// <summary>Set YUV422 image to process</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="image">YUV422 image</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_frame_data_set_yuv_img(IntPtr frame_data, ref bnb_yuv_422_image_t image, out IntPtr error);


        /// <summary>Set BPC8 image to process</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="image">BPC8 image</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bnb_frame_data_set_bpc8_img(IntPtr frame_data, ref bnb_bpc8_image_t image, out IntPtr error);

        /// <summary>Check if recognition result exist</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>true when face recognition result exist in frame_data instance</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_frame_data_has_frx_result(IntPtr frame_data, out IntPtr error);


        /// <summary>Get face transformed to viewport</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="face_index">index of face</param>
        /// <param name="width">viewport width</param>
        /// <param name="height">viewport height</param>
        /// <param name="mode">fitting mode to adjust viewport aspect ratio</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>face data transformed to viewport</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_face_transform_t bnb_frame_data_get_face_transform(IntPtr frame_data, int face_index, int width, int height, bnb_rect_fit_mode_t mode, out IntPtr error);


        /// <summary>Adjust rects to have the same aspect ratio as if fitting source_rect into target_rect according to mode.</summary>
        /// <param name="source_rect">source rectangle</param>
        /// <param name="target_rect">target rectangle</param>
        /// <param name="mode">fitting mode to adjust viewport aspect ratio</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>
        /// source_rect, target_rect have the same (+-rounding) aspect ratio,
        /// and are always not exceeding corresponding input rect, preserving original centers.
        /// May do some per-axis scale adjustments within small margin for fast, integral and/or pixel-perfect scaling between pixel rects
        /// </returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_pixel_rect_t bnb_fit_rects_aspect_ratio(bnb_pixel_rect_t source_rect, bnb_pixel_rect_t target_rect, bnb_rect_fit_mode_t mode, out IntPtr error);


        /// <summary>Get detected face count</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>detected face count</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bnb_frame_data_get_face_count(IntPtr frame_data, out IntPtr error);


        /// <summary>Get detected face data</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="face_index">index of face</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>face data structure</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_face_data_t bnb_frame_data_get_face(IntPtr frame_data, int face_index, out IntPtr error);


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_static_face_data_t get_static_pos_data();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_static_uv_size();


        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void get_static_uv(IntPtr data);


        /// <summary>Get texture coordinates array size</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>texture coordinates size</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bnb_frame_data_get_tex_coords_size(IntPtr frame_data, out IntPtr error);


        /// <summary>Get texture coordinates array</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>texture coordinates array of floats (float*)</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bnb_frame_data_get_tex_coords(IntPtr frame_data, out IntPtr error);


        /// <summary>Get triangles array size</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>triangles array size</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bnb_frame_data_get_triangles_size(IntPtr frame_data, out IntPtr error);


        /// <summary>Get triangles array</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>triangles array (int*)</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bnb_frame_data_get_triangles(IntPtr frame_data, out IntPtr error);


        /// <summary>Get wire indicies array size</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>wire indicies array size</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int bnb_frame_data_get_wire_indicies_size(IntPtr frame_data, out IntPtr error);


        /// <summary>Get wire indicies array</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>wire indicies array (int*)</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bnb_frame_data_get_wire_indicies(IntPtr frame_data, out IntPtr error);


        /// <summary>Check if action units exist</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>true if action units exists in frame_data instance</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_frame_data_has_action_units(IntPtr frame_data, out IntPtr error);


        /// <summary>Get action units data</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="face_index">index of face, which action units will be returned</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>action units structure</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_action_units_t bnb_frame_data_get_action_units(IntPtr frame_data, int face_index, out IntPtr error);


        /// <summary>Check if mouth open on first face</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>true if mouth open</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_frame_data_get_is_mouth_open(IntPtr frame_data, out IntPtr error);


        /// <summary>Check if smile on first face</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>true if smile</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_frame_data_get_is_smile(IntPtr frame_data, out IntPtr error);


        /// <summary>Check if brows raised on first face</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>true if brows raised</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_frame_data_get_is_brows_raised(IntPtr frame_data, out IntPtr error);


        /// <summary>Check if brows shifted on first face</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>true if brows shifted</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool bnb_frame_data_get_is_brows_shifted(IntPtr frame_data, out IntPtr error);


        /// <summary>Get segmentation mask data</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="maskType">defines what will be segmentated</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>bnb_bg_mask_data_gpu_t struct</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_segm_mask_data_t bnb_frame_data_get_segmentation_mask_data(IntPtr frame_data, bnb_segm_type_t maskType, out IntPtr error);

        /// <summary>Get hand gesture. (Requires 'hand_gestures' feature enabled)</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>hand gesture enum</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_hand_gesture_t bnb_frame_data_get_gesture(IntPtr frame_data, out IntPtr error);

        /// <summary>Get hand skeleton with hand_gestures enabled. (Requires 'hand_skeleton' feature enabled)</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="width">screen width</param>
        /// <param name="height">screen height</param>
        /// <param name="mode">screen fit mode</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>hand gesture enum</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_hand_data_t bnb_frame_data_get_hand(IntPtr fd, int width, int height, bnb_rect_fit_mode_t mode, out IntPtr error);

        /// <summary>Get lips shine mask data</summary>
        /// <param name="frame_data">frame_data instance</param>
        /// <param name="error">bnb_error**, may be NULL</param>
        /// <returns>mask data</returns>
        [DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bnb_lips_shine_mask_data_t bnb_frame_data_get_lips_shine_mask(IntPtr frame_data, out IntPtr error);
    }

}