using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB
{
    public class Color32Pinner
    {
        GCHandle _pinnedArray;

        public Color32Pinner(Color32[] obj)
        {
            _pinnedArray = GCHandle.Alloc(obj, GCHandleType.Pinned);
        }

        ~Color32Pinner()
        {
            _pinnedArray.Free();
        }

        public static implicit operator IntPtr(Color32Pinner ap)
        {
            return ap._pinnedArray.AddrOfPinnedObject();
        }
    }

}