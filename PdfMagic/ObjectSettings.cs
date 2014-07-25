using PdfMagic.Engine.Attributes;
using PdfMagic.Engine.Models.Abstract;
using PdfMagic.Enums;

namespace PdfMagic
{
    public sealed class ObjectSettings : ISettingsModel
    {
        private bool _background = true;
        private string _defaultEncoding = "utf-8";
        private bool _enableIntelligentShrinking = false;
        private bool _enableJavascript = true;
        private int _jsdelay = 200;
        private LoadErrorHandlingType _loadErrorHandling = LoadErrorHandlingType.Abort;
        private bool _loadImages = true;
        private int _minimumFontSize = -1;
        private bool _stopSlowScripts = true;
        private string _userStyleSheet = "";
        private string _windowStatus = "";
        private double _zoomFactor = 1.0;
        private bool _useExternalLinks = true;
        private bool _useLocalLinks = true;
        private bool _produceForms = true;
        private bool _createLinks = true;
        private bool _repeatCustomHeaders = true;

        #region Page Settings
        public string Page { get; set; }
        public string PageUri { get; set; }

        public bool UseExternalLinks
        {
            get { return _useExternalLinks; }
            set { _useExternalLinks = value; }
        }

        public bool UseLocalLinks
        {
            get { return _useLocalLinks; }
            set { _useLocalLinks = value; }
        }

        public bool ProduceForms
        {
            get { return _produceForms; }
            set { _produceForms = value; }
        }

        #endregion

        #region Table of Contents Settings
        [SettingNameOverride("toc.useDottedLines")]
        public bool UseDottedLines { get; set; }

        [SettingNameOverride("toc.captionText")]
        public string Caption { get; set; }

        [SettingNameOverride("toc.forwardLinks")]
        public bool CreateForwardLinks
        {
            get { return _createLinks; }
            set { _createLinks = value; }
        }

        [SettingNameOverride("toc.backLinks")]
        public bool BackLinks { get; set; }

        [SettingNameOverride("toc.indentation")]
        public string Indentation { get; set; }

        [SettingNameOverride("toc.fontScale")]
        public decimal FontScale { get; set; }

        [SettingNameOverride("isTableOfContent")]
        public bool CreateToc { get; set; }

        [SettingNameOverride("includeInOutline")]
        public bool IncludeInOutline { get; set; }

        public int PagesCount { get; set; }

        public string TocXsl { get; set; }
        #endregion

        #region Web Settings
        [SettingNameOverride("web.background")]
        public bool Background
        {
            get { return _background; }
            set { _background = value; }
        }

        [SettingNameOverride("web.defaultEncoding")]
        public string DefaultEncoding
        {
            get { return _defaultEncoding; }
            set { _defaultEncoding = value; }
        }

        [SettingNameOverride("web.enableIntelligentShrinking")]
        public bool EnableIntelligentShrinking
        {
            get { return _enableIntelligentShrinking; }
            set { _enableIntelligentShrinking = value; }
        }

        [SettingNameOverride("web.enableJavascript")]
        public bool EnableJavascript
        {
            get { return _enableJavascript; }
            set { _enableJavascript = value; }
        }

        [SettingNameOverride("web.enablePlugins")]
        public bool EnablePlugins { get; set; }

        [SettingNameOverride("web.loadImages")]
        public bool LoadImages
        {
            get { return _loadImages; }
            set { _loadImages = value; }
        }

        [SettingNameOverride("web.minimumFontSize")]
        public int MinimumFontSize
        {
            get { return _minimumFontSize; }
            set { _minimumFontSize = value; }
        }

        [SettingNameOverride("web.printMediaType")]
        public bool PrintMediaType { get; set; }

        [SettingNameOverride("web.userStyleSheet")]
        public string UserStyleSheet
        {
            get { return _userStyleSheet; }
            set { _userStyleSheet = value; }
        }
        #endregion Web Settings

        #region Load Settings
        [SettingNameOverride("load.blockLocalFileAccess")]
        public bool BlockLocalFileAccess { get; set; }

        [SettingNameOverride("load.debugJavascript")]
        public bool DebugJavascript { get; set; }

        [SettingNameOverride("load.jsdelay")]
        public int Jsdelay
        {
            get { return _jsdelay; }
            set { _jsdelay = value; }
        }

        [SettingNameOverride("load.loadErrorHandling")]
        public LoadErrorHandlingType LoadErrorHandling
        {
            get { return _loadErrorHandling; }
            set { _loadErrorHandling = value; }
        }

        [SettingNameOverride("load.password")]
        public string Password { get; set; }

        [SettingNameOverride("load.proxy")]
        public string Proxy { get; set; }

        [SettingNameOverride("load.repeatCustomHeaders")]
        public bool RepeatCustomHeaders
        {
            get { return _repeatCustomHeaders; }
            set { _repeatCustomHeaders = value; }
        }

//        [SettingNameOverride("load.stopSlowScript")]
//        public bool StopSlowScripts
//        {
//            get { return _stopSlowScripts; }
//            set { _stopSlowScripts = value; }
//        } TODO: Find out why this is failing...

        [SettingNameOverride("load.username")]
        public string Username { get; set; }

        [SettingNameOverride("load.windowStatus")]
        public string WindowStatus
        {
            get { return _windowStatus; }
            set { _windowStatus = value; }
        }

        [SettingNameOverride("load.zoomFactor")]
        public double ZoomFactor
        {
            get { return _zoomFactor; }
            set { _zoomFactor = value; }
        }
        #endregion Load Settings
    }
}