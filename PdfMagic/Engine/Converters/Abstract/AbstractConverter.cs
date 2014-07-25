using PdfMagic.Engine.Events;
using PdfMagic.Engine.Integration;
using PdfMagic.Engine.Models;
using System;

namespace PdfMagic.Engine.Converters.Abstract
{
    internal abstract class AbstractConverter : IConverter
    {
        protected BoundInstance BoundInstance;

        public event EventHandler<EventArgs<int>> Begin
        {
            add { BoundInstance.Begin += value; }
            remove { BoundInstance.Begin -= value; }
        }

        public event EventHandler<EventArgs<string>> Error
        {
            add { BoundInstance.Error += value; }
            remove { BoundInstance.Error -= value; }
        }

        public event EventHandler<EventArgs<bool>> Finished
        {
            add { BoundInstance.Finished += value; }
            remove { BoundInstance.Finished -= value; }
        }

        public event EventHandler<EventArgs<int, string>> PhaseChanged
        {
            add { BoundInstance.PhaseChanged += value; }
            remove { BoundInstance.PhaseChanged -= value; }
        }

        public event EventHandler<EventArgs<int, string>> ProgressChanged
        {
            add { BoundInstance.ProgressChanged += value; }
            remove { BoundInstance.ProgressChanged -= value; }
        }

        public event EventHandler<EventArgs<string>> Warning
        {
            add { BoundInstance.Warning += value; }
            remove { BoundInstance.Warning -= value; }
        }

        public GlobalSettings GlobalSettings { get; set; }

        public ObjectSettings ObjectSettings { get; set; }

        public abstract byte[] Convert(string inputHtml);

        public abstract void Dispose();
    }
}