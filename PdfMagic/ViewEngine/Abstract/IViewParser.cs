namespace PdfMagic.ViewEngine.Abstract
{
    internal interface IViewParser
    {
        string Parse(string view, object model);
        void SetTemplatePath(string templatePath, string basePathModifier = null);
    }
}