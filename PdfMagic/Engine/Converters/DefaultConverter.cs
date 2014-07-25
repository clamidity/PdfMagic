using System;
using System.Text;
using PdfMagic.Engine.Converters.Abstract;
using PdfMagic.Engine.Integration;

namespace PdfMagic.Engine.Converters
{
    internal sealed class DefaultConverter : AbstractConverter
	{
        public StringBuilder ErrorString { get; set; }
		private bool _disposed;

        public DefaultConverter()
        {
            BoundInstance = new BoundInstance();
        }

        public override byte[] Convert(string inputHtml)
		{
			if (inputHtml == null)
				throw new ArgumentNullException("inputHtml");

            BoundInstance.SetGlobalSettings(GlobalSettings);
            BoundInstance.SetObjectSettings(ObjectSettings);

			return BoundInstance.Convert(inputHtml);
		}

        private void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
                BoundInstance.Dispose();
			}

			_disposed = true;
		}

		public override void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~DefaultConverter()
		{
			Dispose(false);
		}
	}
}
