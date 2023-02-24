using System;

namespace BNB
{
    public class FrameData
    {
        private IntPtr _context;

        public FrameData()
        {
            IntPtr error = IntPtr.Zero;
            _context = BanubaSDKBridge.bnb_frame_data_init(out error);
            Utils.CheckError(error);
        }

        ~FrameData()
        {
            IntPtr error = IntPtr.Zero;
            BanubaSDKBridge.bnb_frame_data_release(_context, out error);
            Utils.CheckError(error);
        }

        public static implicit operator IntPtr(FrameData frameData)
        {
            return frameData._context;
        }
    }

}
