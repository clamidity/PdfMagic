using System;
using System.IO;
using PdfMagic.Engine.Converters;
using PdfMagic.Engine.Converters.Abstract;
using PdfMagic.ViewEngine;

namespace PdfMagic
{
    public sealed class PdfMagicManager : IDisposable
    {
        private readonly IConverter _converter;
        private readonly ViewCompiler _viewCompiler;
        private bool _disposed;
        public GlobalSettings GlobalSettings { get; set; }
        public ObjectSettings ObjectSettings { get; set; }


        public PdfMagicManager(bool async = true)
        {
            _converter = async ? (IConverter) new AsynchronousConverter() : new DefaultConverter();
            _viewCompiler = new ViewCompiler();
            GlobalSettings = new GlobalSettings();
            ObjectSettings = new ObjectSettings();
        }

        public bool ConvertTemplate(string templateName, string basePath, string destinationPath)
        {
            string html = _viewCompiler.RenderTemplate(templateName, basePath);
            return ConvertString(html, destinationPath);
        }

        public bool ConvertString(string html, string path)
        {
            if (GlobalSettings == null)
                throw new InvalidOperationException("GlobalSettings must be set in order to proceed with conversion.");
            if (ObjectSettings == null)
                throw new InvalidOperationException("ObjectSettings must be set in order to proceed with conversion.");

            _converter.GlobalSettings = GlobalSettings;
            _converter.ObjectSettings = ObjectSettings;

            return SaveFile(_converter.Convert(html), path);
        }

        public bool ConvertString(string html, string path, ObjectSettings objectSettings)
        {
            ObjectSettings = objectSettings;
            return ConvertString(html, path);
        }

        public bool ConvertString(string html, string path, GlobalSettings globalSettings)
        {
            GlobalSettings = globalSettings;
            return ConvertString(html, path);
        }

        public bool ConvertString(string html, string path, GlobalSettings globalSettings, ObjectSettings objectSettings)
        {
            GlobalSettings = globalSettings;
            ObjectSettings = objectSettings;

            return ConvertString(html, path);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            _converter.Dispose();
            _disposed = true;
        }

        private bool SaveFile(byte[] bytes, string path)
        {
            if (bytes == null) return false;

            File.WriteAllBytes(path, bytes);
            return true;
        }

        ~PdfMagicManager()
        {
            Dispose(false);
        }
    }
}
