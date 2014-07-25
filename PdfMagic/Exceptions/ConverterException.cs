using System;

namespace PdfMagic.Exceptions
{
	[Serializable]
	public class PdfMagicException : Exception
	{
		public PdfMagicException() { }
		public PdfMagicException(string message) : base(message) { }
        public PdfMagicException(string message, Exception innerException) : base(message, innerException) { }
        protected PdfMagicException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
