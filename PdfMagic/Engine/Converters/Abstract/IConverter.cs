using System;
using PdfMagic.Engine.Events;
using PdfMagic.Engine.Models;

namespace PdfMagic.Engine.Converters.Abstract
{
	/// <summary>
	/// Generic PDF conversion interface.
	/// </summary>
    internal interface IConverter : IDisposable
	{
		event EventHandler<EventArgs<int>> Begin;
		event EventHandler<EventArgs<string>> Error;
		event EventHandler<EventArgs<bool>> Finished;
		event EventHandler<EventArgs<int, string>> PhaseChanged;
		event EventHandler<EventArgs<int, string>> ProgressChanged;
		event EventHandler<EventArgs<string>> Warning;

		GlobalSettings GlobalSettings { get; set; }
		ObjectSettings ObjectSettings { get; set;  }

		byte[] Convert(string inputHtml);
	}
}
