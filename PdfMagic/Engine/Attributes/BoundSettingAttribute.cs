using System;

namespace PdfMagic.Engine.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal class SettingNameOverrideAttribute : Attribute
    {
        public string TargetName { get; set; }

        public SettingNameOverrideAttribute()
        {
        }

        public SettingNameOverrideAttribute(string targetName)
        {
            TargetName = targetName;
        }
    }
}