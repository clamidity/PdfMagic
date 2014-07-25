using PdfMagic.ViewEngine.Abstract;
using PdfMagic.ViewEngine.Parsers;
using PdfMagic.ViewEngine.Retrievers;

namespace PdfMagic.ViewEngine
{
    internal class ViewCompiler
    {
        private readonly IViewRetriever _viewRetriever;
        private readonly IViewParser _viewParser;

        public ViewCompiler(IViewRetriever viewRetriever, IViewParser viewParser)
        {
            _viewRetriever = viewRetriever;
            _viewParser = viewParser;
        }

        public ViewCompiler() : this(new DefaultViewRetriever(), new RazorViewParser())
        {
        }

        public string RenderTemplate(string templateName, string templatePath)
        {
            _viewRetriever.SetTemplatePath(templatePath);
            _viewParser.SetTemplatePath(templatePath);
            if (!templateName.Contains(".cshtml")) templateName = string.Format("{0}.cshtml", templateName);

            return _viewParser.Parse(_viewRetriever.RetrieveViewString(templateName), null);
        }
    }
}
