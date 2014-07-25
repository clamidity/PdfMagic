using PdfMagic.Engine.Attributes;
using PdfMagic.Engine.Models.Abstract;
using PdfMagic.Enums;

namespace PdfMagic
{
    public sealed class GlobalSettings : ISettingsModel
    {
        /// <summary>
        ///     DPI to use when printing
        /// </summary>
        /// <example>80</example>
        public int? Dpi { get; set; }

        /// <summary>
        ///     Maximum DPI for images
        /// </summary>
        public int? ImageDpi { get; set; }

        /// <summary>
        ///     Color or Greyscale
        /// </summary>
        public string ColorMode { get; set; }

        /// <summary>
        ///     Overall quality of images dictated
        ///     by compression factor.
        /// </summary>
        /// <example>0 - 100</example>
        public int? ImageQuality { get; set; }

        /// <summary>
        ///     Used to generate a table of contents
        /// </summary>
        public bool? Outline { get; set; }

        /// <summary>
        ///     How deep the outline traverses
        /// </summary>
        public int? OutlineDepth { get; set; }

        /// <summary>
        ///     Title of the document
        /// </summary>
        public string DocumentTitle { get; set; }

        /// <summary>
        ///     Size of paper to use
        /// </summary>
        /// <example>A4</example>
        [SettingNameOverride("size.pageSize")]
        public string PaperSize { get; set; } // TODO: enum?

        /// <summary>
        ///     Width of the paper
        /// </summary>
        [SettingNameOverride("size.width")]
        public string Width { get; set; }

        /// <summary>
        ///     Height of the paper
        /// </summary>
        [SettingNameOverride("size.height")]
        public string Height { get; set; }

        /// <summary>
        ///     Enables lossless compression
        /// </summary>
        public bool? UseCompression { get; set; }

        /// <summary>
        ///     Top margin
        /// </summary>
        /// <example>2cm</example>
        [SettingNameOverride("margin.top")]
        public string MarginTop { get; set; }

        /// <summary>
        ///     Right margin
        /// </summary>
        /// <example>2cm</example>
        [SettingNameOverride("margin.right")]
        public string MarginRight { get; set; }

        /// <summary>
        ///     Left margin
        /// </summary>
        /// <example>2cm</example>
        [SettingNameOverride("margin.left")]
        public string MarginLeft { get; set; }

        /// <summary>
        ///     Bottom margin
        /// </summary>
        /// <example>2cm</example>
        [SettingNameOverride("margin.bottom")]
        public string MarginBottom { get; set; }

        /// <summary>
        ///     Output
        /// </summary>
        public string Out { get; set; }
        
        /// <summary>
        ///     Page offset
        /// </summary>
        public int? PageOffset { get; set; }

        /// <summary>
        ///     How many copies to include
        /// </summary>
        public int? Copies { get; set; }

        /// <summary>
        ///     Define if collation should be used
        /// </summary>
        public bool? Collate { get; set; }

        /// <summary>
        ///     Desired format of the 
        ///     generated file
        /// </summary>
        /// <example>ps or pdf</example>
        public string OutputFormat { get; set; }

        /// <summary>
        ///     Path to cookie text file
        /// </summary>
        public string CookieJar { get; set; }

        /// <summary>
        ///     Page orientation
        /// </summary>
        public Orientation? Orientation { get; set; }

        
    }
}