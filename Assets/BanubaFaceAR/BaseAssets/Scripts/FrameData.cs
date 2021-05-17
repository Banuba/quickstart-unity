using System;
using System.Collections;
using System.Collections.Generic;

namespace BNB
{
    public class FrameData
    {
        IntPtr context;

        public FrameData()
        {
            IntPtr error = IntPtr.Zero;
            context = BanubaSDKBridge.bnb_frame_data_init(out error);
            Utils.CheckError(error);
        }

        ~FrameData()
        {
            IntPtr error = IntPtr.Zero;
            BanubaSDKBridge.bnb_frame_data_release(context, out error);
            Utils.CheckError(error);
        }

        public static implicit operator IntPtr(FrameData frameData)
        {
            return frameData.context;
        }
    }

}
