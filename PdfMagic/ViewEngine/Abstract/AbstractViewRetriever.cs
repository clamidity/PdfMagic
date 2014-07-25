using System;
using System.IO;

namespace PdfMagic.ViewEngine.Abstract
{
    internal abstract class AbstractViewRetriever : IViewRetriever
    {
        protected string ViewBasePath;

        public abstract string RetrieveViewString(string view);

        public void SetTemplatePath(string templatePath)
        {
            ViewBasePath = templatePath;
        }

        protected StreamReader FindView(string view)
        {
            if (string.IsNullOrEmpty(ViewBasePath)) throw new NullReferenceException("ViewBasePath");
            string viewPath = Path.Combine(ViewBasePath, view);
            return new StreamReader(viewPath);
        }

        protected string ReadView(StreamReader view)
        {
            return view.ReadToEnd();
        }
    }
}