using System.IO;
using PdfMagic.ViewEngine.Abstract;

namespace PdfMagic.ViewEngine.Retrievers
{
    /// <summary>
    ///     Base implementation of the AbstractViewRetriever
    ///      - Used to finding views and reading the content
    /// </summary>
    internal class DefaultViewRetriever : AbstractViewRetriever
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public DefaultViewRetriever() : this(null)
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="path"></param>
        public DefaultViewRetriever(string path)
        {
            ViewBasePath = path;
        }

        /// <summary>
        ///     Function for retrieving the content of a view
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public override string RetrieveViewString(string view)
        {
            StreamReader reader = FindView(view);
            return ReadView(reader);
        }
    }
}