using System;

namespace PdfMagic.Engine.Events
{
    internal class EventArgs<T> : EventArgs
	{
		public EventArgs(T value)
		{
			_mValue = value;
		}

		private readonly T _mValue;

		public T Value
		{
			get { return _mValue; }
		}
	}

    internal class EventArgs<T1, T2> : EventArgs<T1>
	{
		public T2 Value2 { get; private set; }

		public EventArgs(T1 value, T2 value2)
			: base(value)
		{
			Value2 = value2;
		}
	}
}
