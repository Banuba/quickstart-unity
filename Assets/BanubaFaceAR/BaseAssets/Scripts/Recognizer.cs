using System;

namespace BNB
{
    public class Recognizer
    {
        IntPtr context;

        public Recognizer(string resourcesPath)
        {
            IntPtr error = IntPtr.Zero;
            context = BanubaSDKBridge.bnb_recognizer_init("file:///" + resourcesPath, "", "", false, out error);
            Utils.CheckError(error);
        }

        ~Recognizer()
        {
            var error = IntPtr.Zero;
            BanubaSDKBridge.bnb_recognizer_release(context, out error);
            Utils.CheckError(error);
        }

        public static implicit operator IntPtr(Recognizer recognizer)
        {
            return recognizer.context;
        }
    }

}
