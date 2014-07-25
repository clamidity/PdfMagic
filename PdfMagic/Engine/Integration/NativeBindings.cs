using System;
using System.Runtime.InteropServices;
using PdfMagic.Engine.Events;

namespace PdfMagic.Engine.Integration
{
    internal static class NativeBindings
    {
        #region Library Methods

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern int wkhtmltopdf_deinit();

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern int wkhtmltopdf_extended_qt();

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern int wkhtmltopdf_init(int useGraphics);

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern IntPtr wkhtmltopdf_version();
        #endregion Library Methods

        #region Settings Methods

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern void wkhtmltopdf_add_object(IntPtr converter, IntPtr objectSettings,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshaler))] string htmlData);

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern IntPtr wkhtmltopdf_create_converter(IntPtr globalSettings);

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern IntPtr wkhtmltopdf_create_global_settings();
        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern IntPtr wkhtmltopdf_create_object_settings();

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern int wkhtmltopdf_set_global_setting(IntPtr globalSettings,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshaler))] string name,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshaler))] string value);

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern int wkhtmltopdf_set_object_setting(IntPtr objectSettings,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshaler))] string name,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Marshaler))] string value);
        #endregion Settings Methods

        #region Converter Methods

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern int wkhtmltopdf_convert(IntPtr converter);

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern void wkhtmltopdf_destroy_converter(IntPtr converter);

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern IntPtr wkhtmltopdf_get_output(IntPtr converter, out IntPtr data);
        #endregion Converter Methods

        #region Phase Methods

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern int wkhtmltopdf_current_phase(IntPtr converter);

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern int wkhtmltopdf_phase_count(IntPtr converter);

        [DllImport(@"lib\wkhtmltox0.dll")]
        // NOTE: Using IntPtr as return to avoid runtime from freeing returned string. (pruiz)
        public static extern IntPtr wkhtmltopdf_phase_description(IntPtr converter, int phase);

        #endregion Phase Methods

        #region Progress Methods

        [DllImport(@"lib\wkhtmltox0.dll")]
        // NOTE: Using IntPtr as return to avoid runtime from freeing returned string. (pruiz)
        public static extern IntPtr wkhtmltopdf_progress_string(IntPtr converter);

        #endregion Progress Methods

        #region Error Codes

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern int wkhtmltopdf_http_error_code(IntPtr converter);

        #endregion Error Codes

        #region Callbacks

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern void wkhtmltopdf_set_error_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] Callbacks.StringCallback cb);

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern void wkhtmltopdf_set_finished_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] Callbacks.BoolCallback cb);

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern void wkhtmltopdf_set_phase_changed_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] Callbacks.VoidCallback cb);

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern void wkhtmltopdf_set_progress_changed_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] Callbacks.IntCallback cb);

        [DllImport(@"lib\wkhtmltox0.dll")]
        public static extern void wkhtmltopdf_set_warning_callback(IntPtr converter, [MarshalAs(UnmanagedType.FunctionPtr)] Callbacks.StringCallback cb);
        #endregion Callbacks

        #region WkHtmlToPdf Call Wrappers

        public static string WkHtmlToPdfVersion()
        {
            var ptr = wkhtmltopdf_version();
            return Marshal.PtrToStringAnsi(ptr);
        }

        #endregion WkHtmlToPdf Call Wrappers
    }
}