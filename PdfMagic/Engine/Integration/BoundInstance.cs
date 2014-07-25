using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using PdfMagic.Engine.Attributes;
using PdfMagic.Engine.Events;
using PdfMagic.Engine.Models;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using PdfMagic.Engine.Models.Abstract;
using PdfMagic.Exceptions;

namespace PdfMagic.Engine.Integration
{
    internal class BoundInstance : IDisposable
    {
        private int _currentPhase;
        private GlobalSettings _globalSettings;
        private ObjectSettings _objectSettings;
        private bool _disposed;

        public BoundInstance(bool useX11 = false)
        {
            if (NativeBindings.wkhtmltopdf_init(useX11 ? 1 : 0) == 0)
                throw new InvalidOperationException("Failed to initialize wkhtmltopdf!");
        }

        public BoundInstance(GlobalSettings globalSettings, bool useX11 = false) 
            : this(useX11)
        {
            _globalSettings = globalSettings;
        }

        public event EventHandler<EventArgs<int>> Begin = delegate { };

        public event EventHandler<EventArgs<string>> Error = delegate { };

        public event EventHandler<EventArgs<bool>> Finished = delegate { };

        public event EventHandler<EventArgs<int, string>> PhaseChanged = delegate { };

        public event EventHandler<EventArgs<int, string>> ProgressChanged = delegate { };

        public event EventHandler<EventArgs<string>> Warning = delegate { };

        public string ErrorMessage { get; set; }

        public void SetGlobalSettings(GlobalSettings settings)
        {
            _globalSettings = settings;
        }

        public void SetObjectSettings(ObjectSettings settings)
        {
            _objectSettings = settings;
        }

        public IntPtr SetGlobalSettings()
        {
            IntPtr ptr = NativeBindings.wkhtmltopdf_create_global_settings();

            foreach (var item in GetProperties(_globalSettings)
                .Where(item => NativeBindings.wkhtmltopdf_set_global_setting(ptr, item.Key, item.Value) == 0))
            {
                throw new ApplicationException(string.Format("Global Settings Failure: '{0}' as '{1}'", item.Key, item.Value));
            }

            return ptr;
        }

        public IntPtr SetObjectSettings()
        {
            IntPtr ptr = NativeBindings.wkhtmltopdf_create_object_settings();

            foreach (var item in GetProperties(_objectSettings)
                .Where(item => NativeBindings.wkhtmltopdf_set_object_setting(ptr, item.Key, item.Value) == 0))
            {
                throw new ApplicationException(string.Format("Object Settings Failure: '{0}' as '{1}'", item.Key, item.Value));
            }

            return ptr;
        }

        public void OnBegin(int expectedPhases)
        {
            Begin(this, new EventArgs<int>(expectedPhases));
        }

        public void OnError(IntPtr ptr, string error)
        {
            ErrorMessage = string.Format("{0}{1}", error, Environment.NewLine);
            Error(this, new EventArgs<string>(error));
        }

        public void OnFinished(IntPtr converter, bool success)
        {
            Finished(this, new EventArgs<bool>(success));
        }

        public void OnPhaseChanged(IntPtr converter)
        {
            IntPtr response = NativeBindings.wkhtmltopdf_phase_description(converter, _currentPhase);
            string str = Marshal.PtrToStringAnsi(response);
            PhaseChanged(this, new EventArgs<int, string>(++_currentPhase, str));
        }

        public void OnProgressChanged(IntPtr converter, int progress)
        {
            IntPtr response = NativeBindings.wkhtmltopdf_progress_string(converter);
            string str = Marshaler.GetInstance(null).MarshalNativeToManaged(response) as string;
            ProgressChanged(this, new EventArgs<int, string>(progress, str));
        }

        public void OnWarning(IntPtr ptr, string warn)
        {
            Warning(this, new EventArgs<string>(warn));
        }

        public byte[] Convert(string inputHtml)
        {
            IntPtr converter = IntPtr.Zero;

            try
            {
                IntPtr globalSettingsResponse = SetGlobalSettings();
                IntPtr objectSettingsResponse = SetObjectSettings();
                converter = BuildConverter(globalSettingsResponse, objectSettingsResponse, inputHtml);
//                SetCallbacks(converter); // TODO: Find out why this is failing...

                if (NativeBindings.wkhtmltopdf_convert(converter) == 0)
                {
                    var msg = string.Format("HtmlToPdf conversion failed: {0}", ErrorMessage);
                    throw new PdfMagicException(msg);
                }

                if (!string.IsNullOrEmpty(_globalSettings.Out))
                    return null;

                IntPtr tmp;
                IntPtr ret = NativeBindings.wkhtmltopdf_get_output(converter, out tmp);
                byte[] output = new byte[ret.ToInt32()];
                Marshal.Copy(tmp, output, 0, output.Length);

                return output;
            }
            finally
            {
                if (converter != IntPtr.Zero)
                {
//                    SetCallbacks(converter, true); // TODO: Find out why this is failing...
                }
            }
        }

        private IntPtr BuildConverter(IntPtr globalSettings, IntPtr objectSettings, string inputHtml)
        {
            var converter = NativeBindings.wkhtmltopdf_create_converter(globalSettings);
            NativeBindings.wkhtmltopdf_add_object(converter, objectSettings, inputHtml);

            return converter;
        }

        private void SetCallbacks(IntPtr converter, bool nulled = false)
        {
            Callbacks.StringCallback errorCallback = OnError; 
            Callbacks.StringCallback warningCallback = OnWarning;
            Callbacks.VoidCallback phaseCallback = OnPhaseChanged;
            Callbacks.IntCallback progressCallback = OnProgressChanged;
            Callbacks.BoolCallback finishCallback = OnFinished;

            NativeBindings.wkhtmltopdf_set_error_callback(converter, !nulled ? errorCallback : null);
            NativeBindings.wkhtmltopdf_set_warning_callback(converter, !nulled ? warningCallback : null);
            NativeBindings.wkhtmltopdf_set_phase_changed_callback(converter, !nulled ? phaseCallback : null);
            NativeBindings.wkhtmltopdf_set_progress_changed_callback(converter, !nulled ? progressCallback : null);
            NativeBindings.wkhtmltopdf_set_finished_callback(converter, !nulled ? finishCallback : null);
            NativeBindings.wkhtmltopdf_destroy_converter(converter);
        }

        private string GetStringValue(object value)
        {
            if (value == null) return null;

            var tmp = value is string ? value as string : System.Convert.ToString(value, CultureInfo.InvariantCulture);
            tmp = tmp == "True" ? "true" : tmp;
            tmp = tmp == "False" ? "false" : tmp;

            return tmp;
        }

        private IEnumerable<KeyValuePair<string, string>> GetProperties(ISettingsModel settings)
        {
            var dict = new Dictionary<string, string>();
            var type = settings.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                SettingNameOverrideAttribute nameOverrideAttribute = property.GetCustomAttribute<SettingNameOverrideAttribute>();
                object value = property.GetValue(settings, null);
                if (value == null) continue;

                if (nameOverrideAttribute != null)
                {
                    dict.Add(nameOverrideAttribute.TargetName, GetStringValue(value));
                }
                else
                {
                    dict.Add(Char.ToLower(property.Name[0]) + property.Name.Substring(1), GetStringValue(value));
                }
            }

            return dict;
        }

        private void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				Begin = null;
				PhaseChanged = null;
				ProgressChanged = null;
				Finished = null;
				Error = null;
				Warning = null;
			}

			try {
                NativeBindings.wkhtmltopdf_deinit();
			}
			catch (DllNotFoundException) {
			}

			_disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~BoundInstance()
		{
			Dispose(false);
		}
    }
}