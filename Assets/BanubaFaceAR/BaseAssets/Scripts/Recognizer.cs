using System;

namespace BNB
{
    public class Recognizer
    {
        private IntPtr _context;

        public Recognizer(string resourcesPath)
        {
            IntPtr error = IntPtr.Zero;
            _context = BanubaSDKBridge.bnb_recognizer_init("file:///" + resourcesPath, false, out error);
            Utils.CheckError(error);
        }

        ~Recognizer()
        {
            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_release(_context, out error);
            Utils.CheckError(error);
        }

        public static implicit operator IntPtr(Recognizer recognizer)
        {
            return recognizer._context;
        }
    }

}
