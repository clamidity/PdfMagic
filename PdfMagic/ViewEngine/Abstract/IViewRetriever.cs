namespace PdfMagic.ViewEngine.Abstract
{
    internal interface IViewRetriever
    {
        string RetrieveViewString(string view);
        void SetTemplatePath(string templatePath);
    }
}