using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using PdfMagic.ViewEngine.Abstract;
using RazorEngine;

namespace PdfMagic.ViewEngine.Parsers
{
    /// <summary>
    ///     Basic View Parser
    ///      - Users Razor to parse/fill views for use
    ///        in rendering.
    /// </summary>
    internal class RazorViewParser : IViewParser
    {
        private string _basePath;
        private string _basePathModifier;

        public RazorViewParser() : this(null)
        {
        }

        public RazorViewParser(string basePath, string basePathModifier = null)
        {
            _basePath = basePath;
            _basePathModifier = basePathModifier;
        }

        /// <summary>
        ///     Function for parsing a view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string Parse(string view, object model)
        {
            string html = Razor.Parse(view, model);
            ExtractStyles(ref html);
            ExtractImages(ref html);

            return html;
        }

        public void SetTemplatePath(string templatePath, string basePathModifier = null)
        {
            _basePath = templatePath;
            _basePathModifier = basePathModifier;
        }

        private void ExtractStyles(ref string html)
        {
            HtmlDocument documentRenderer = new HtmlDocument();
            documentRenderer.LoadHtml(html);
            HtmlNode document = documentRenderer.DocumentNode;
            IEnumerable<HtmlNode> links = document.QuerySelectorAll("link");
            if (!links.Any()) return;

            foreach (HtmlNode link in links)
            {
                string href = link.GetAttributeValue("href", null);
                if (string.IsNullOrEmpty(href)) continue;
                if (href.StartsWith("/")) href = href.Substring(1);
                if (href.Contains("http")) continue;

                string path = Path.GetFullPath(Path.Combine(_basePath, string.Format("{0}{1}", _basePathModifier, href)));
                link.SetAttributeValue("href", path);
            }

            html = document.OuterHtml;
        }

        private void ExtractImages(ref string html)
        {
            HtmlDocument documentRenderer = new HtmlDocument();
            documentRenderer.LoadHtml(html);
            HtmlNode document = documentRenderer.DocumentNode;
            IEnumerable<HtmlNode> images = document.QuerySelectorAll("img");
            if (!images.Any()) return;

            foreach (HtmlNode image in images)
            {
                string href = image.GetAttributeValue("src", null);
                if (string.IsNullOrEmpty(href)) continue;
                if (href.StartsWith("/")) href = href.Substring(1);
                if (href.Contains("http")) continue;

                string path = Path.GetFullPath(Path.Combine(_basePath, string.Format("{0}{1}", _basePathModifier, href)));
                image.SetAttributeValue("src", path);
            }

            html = document.OuterHtml;
        }
    }
}