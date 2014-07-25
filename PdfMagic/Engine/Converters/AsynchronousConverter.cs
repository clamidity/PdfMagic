using System;
using PdfMagic.Engine.Converters.Abstract;
using PdfMagic.Engine.Integration;

namespace PdfMagic.Engine.Converters
{
    internal class AsynchronousConverter : AbstractConverter
    {
        private static readonly DelegateQueue.DelegateQueue Worker = new DelegateQueue.DelegateQueue("WkHtmlToPdf");
        private static BoundInstance _initializedInstance;

        public AsynchronousConverter()
        {
            lock (Worker)
            {
                if (_initializedInstance == null)
                {
                    Worker.Invoke((Action)(() => _initializedInstance = new BoundInstance()));

                    AppDomain.CurrentDomain.ProcessExit += (o, e) =>
                        Worker.Invoke((Action)(() =>
                        {
                            _initializedInstance.Dispose();
                            _initializedInstance = null;
                        }));
                }
            }

            Worker.Invoke((Action)(() => BoundInstance = new BoundInstance()));
        }

        public override byte[] Convert(string inputHtml)
        {
            return (byte[])Worker.Invoke((Func<string, byte[]>)(x =>
            {
                BoundInstance.SetGlobalSettings(GlobalSettings);
                BoundInstance.SetObjectSettings(ObjectSettings);
                return BoundInstance.Convert(x);
            }), inputHtml);
        }

        public override void Dispose()
        {
            if (BoundInstance != null)
                Worker.Invoke((Action)(() => BoundInstance.Dispose()));

            BoundInstance = null;
        }
    }
}