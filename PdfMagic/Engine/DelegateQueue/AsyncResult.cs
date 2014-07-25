#region License

/* Copyright (c) 2006 Leslie Sanford
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

using System;
using System.Threading;

namespace PdfMagic.Engine.DelegateQueue
{
    /// <summary>
    /// Provides basic implementation of the IAsyncResult interface.
    /// </summary>
    internal class AsyncResult : IAsyncResult
    {
        #region AsyncResult Members

        #region Fields

        // The owner of this AsyncResult object.
        private readonly object _owner;

        // The callback to be invoked when the operation completes.
        private readonly AsyncCallback _callback;

        // User state information.
        private readonly object _state;

        // For signaling when the operation has completed.
        private readonly ManualResetEvent _waitHandle = new ManualResetEvent(false);

        // A value indicating whether the operation completed synchronously.
        private bool _completedSynchronously;

        // A value indicating whether the operation has completed.
        private bool _isCompleted;

        // The ID of the thread this AsyncResult object originated on.
        private readonly int _threadId;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the AsyncResult object with the
        /// specified owner of the AsyncResult object, the optional callback
        /// delegate, and optional state object.
        /// </summary>
        /// <param name="owner">
        /// The owner of the AsyncResult object.
        /// </param>
        /// <param name="callback">
        /// An optional asynchronous callback, to be called when the 
        /// operation is complete. 
        /// </param>
        /// <param name="state">
        /// A user-provided object that distinguishes this particular 
        /// asynchronous request from other requests. 
        /// </param>
        public AsyncResult(object owner, AsyncCallback callback, object state)
        {
            _owner = owner;
            _callback = callback;
            _state = state;

            // Get the current thread ID. This will be used later to determine
            // if the operation completed synchronously.
            _threadId = Thread.CurrentThread.ManagedThreadId;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Signals that the operation has completed.
        /// </summary>
        public void Signal()
        {
            _isCompleted = true;

            _completedSynchronously = _threadId == Thread.CurrentThread.ManagedThreadId;

            _waitHandle.Set();

            if(_callback != null)
            {
                _callback(this);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the owner of this AsyncResult object.
        /// </summary>
        public object Owner
        {
            get
            {
                return _owner;
            }
        }

        #endregion

        #endregion

        #region IAsyncResult Members

        public object AsyncState
        {
            get             
            {
                return _state;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get 
            {
                return _waitHandle;
            }
        }

        public bool CompletedSynchronously
        {
            get 
            {
                return _completedSynchronously;
            }
        }

        public bool IsCompleted
        {
            get 
            { 
                return _isCompleted; 
            }
        }

        #endregion
    }
}
