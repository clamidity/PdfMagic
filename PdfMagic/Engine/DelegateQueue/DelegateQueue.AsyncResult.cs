using System;

namespace PdfMagic.Engine.DelegateQueue
{
    internal partial class DelegateQueue
    {
        private enum NotificationType
        {
            None,
            BeginInvokeCompleted,
            PostCompleted
        }

        /// <summary>
        /// Implements the IAsyncResult interface for the DelegateQueue class.
        /// </summary>
        private class DelegateQueueAsyncResult : AsyncResult
        {
            // The delegate to be invoked.
            private readonly Delegate _method;

            // Args to be passed to the delegate.
            private readonly object[] _args;

            // The object returned from the delegate.
            private object _returnValue;

            // Represents a possible exception thrown by invoking the method.
            private Exception _error;

            private readonly NotificationType _notificationType;

            public DelegateQueueAsyncResult(
                object owner, 
                Delegate method, 
                object[] args, 
                bool synchronously, 
                NotificationType notificationType) 
                : base(owner, null, null)
            {
                _method = method;
                _args = args;
                _notificationType = notificationType;
            }

            public void Invoke()
            {
                try
                {
                    _returnValue = _method.DynamicInvoke(_args);
                }
                catch(Exception ex)
                {
                    _error = ex;
                }
                finally
                {
                    Signal();
                }
            }

            public object[] GetArgs()
            {
                return _args;
            }

            public object ReturnValue
            {
                get
                {
                    return _returnValue;
                }
            }

            public Exception Error
            {
                get
                {
                    return _error;
                }
                set
                {
                    _error = value;
                }
            }

            public Delegate Method
            {
                get
                {
                    return _method;
                }
            }

            public NotificationType NotificationType
            {
                get
                {
                    return _notificationType;
                }
            }
        }
    }
}
