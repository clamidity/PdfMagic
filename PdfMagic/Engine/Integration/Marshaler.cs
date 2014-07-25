using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PdfMagic.Engine.Integration
{
    /// <summary>
    /// Marshaller needed to correctly pass values between us and QT APIs.
    /// </summary>
    internal class Marshaler : ICustomMarshaler
    {
        public static readonly Marshaler Instance = new Marshaler();

        #region Helper Methods

        public static void FreeUtf8Ptr(IntPtr ptr)
        {
            Instance.CleanUpNativeData(ptr);
        }

        public static IntPtr StringToUtf8Ptr(string str)
        {
            return Instance.MarshalManagedToNative(str);
        }
        #endregion Helper Methods

        #region ICusomMarshaler

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return Instance;
        }

        public void CleanUpManagedData(object managedObj)
        {
        }

        public void CleanUpNativeData(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return;

            Marshal.FreeHGlobal(ptr);
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public IntPtr MarshalManagedToNative(object obj)
        {
            if (obj == null)
                return IntPtr.Zero;

            if (!(obj is string))
                throw new MarshalDirectiveException("Object must be a string.");

            // not null terminated
            byte[] strbuf = Encoding.UTF8.GetBytes((string)obj);
            var buffer = Marshal.AllocHGlobal(strbuf.Length + 1);
            Marshal.Copy(strbuf, 0, buffer, strbuf.Length);

            // append final null
            Marshal.WriteByte(buffer, strbuf.Length, 0);

            return buffer;
        }

        public unsafe object MarshalNativeToManaged(IntPtr ptr)
        {
            byte* walk = (byte*)ptr;

            // find the end of the string
            while (*walk != 0)
            {
                walk++;
            }
            int length = (int)(walk - (byte*)ptr);

            // should not be null terminated
            byte[] strbuf = new byte[length];

            // skip the trailing null
            Marshal.Copy(ptr, strbuf, 0, length);
            return Encoding.UTF8.GetString(strbuf);
        }
        #endregion ICusomMarshaler
    }
}