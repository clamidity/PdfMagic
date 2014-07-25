using System;
using System.Runtime.InteropServices;

namespace PdfMagic.Engine.Events
{
    internal static class Callbacks
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void BoolCallback(IntPtr converter, bool val);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void IntCallback(IntPtr converter, int val);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void StringCallback(IntPtr converter, [MarshalAs(UnmanagedType.LPStr)] string str);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void VoidCallback(IntPtr converter); 
    }
}