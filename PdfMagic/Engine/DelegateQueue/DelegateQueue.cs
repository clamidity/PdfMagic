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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
//using Sanford.Collections.Generic;

namespace PdfMagic.Engine.DelegateQueue
{
    /// <summary>
    /// Represents an asynchronous queue of delegates.
    /// </summary>
    internal partial class DelegateQueue : SynchronizationContext, IComponent, ISynchronizeInvoke
    {
        #region DelegateQueue Members

        #region Fields

        // The thread for processing delegates.
        private Thread _delegateThread;

        // The deque for holding delegates.
		private readonly Queue<DelegateQueueAsyncResult> _delegateQueue = new Queue<DelegateQueueAsyncResult>();
        //private Deque<DelegateQueueAsyncResult> delegateDeque = new Deque<DelegateQueueAsyncResult>();

        // The object to use for locking.
        private readonly object _lockObject = new object();

        // The synchronization context in which this DelegateQueue was created.
        private readonly SynchronizationContext _context;

        // Inidicates whether the delegate queue has been disposed.
        private volatile bool _disposed;

        // Thread ID counter for all DelegateQueues.
        private volatile static uint _threadId;

        #endregion

        #region Events

        /// <summary>
        /// Occurs after a method has been invoked as a result of a call to 
        /// the BeginInvoke method.
        /// </summary>
        public event EventHandler<InvokeCompletedEventArgs> InvokeCompleted;

        /// <summary>
        /// Occurs after a method has been invoked as a result of a call to
        /// the Post and method.
        /// </summary>
        public event EventHandler<PostCompletedEventArgs> PostCompleted;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the DelegateQueue class.
        /// </summary>
        public DelegateQueue(string name)
        {
            InitializeDelegateQueue(name);

            _context = Current ?? new SynchronizationContext();
        }

        /// <summary>
        /// Initializes a new instance of the DelegateQueue class with the specified IContainer object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="container">
        /// The IContainer to which the DelegateQueue will add itself.
        /// </param>
        public DelegateQueue(string name, IContainer container)
        {
            // Required for Windows.Forms Class Composition Designer support
            container.Add(this);

            InitializeDelegateQueue(name);
        }

        ~DelegateQueue()
        {
            Dispose(false);
        }

        // Initializes the DelegateQueue.
        private void InitializeDelegateQueue(string name)
        {
            // Create thread for processing delegates.
            _delegateThread = new Thread(DelegateProcedure);
			_delegateThread.SetApartmentState(ApartmentState.STA);

            lock(_lockObject)
            {
                // Increment to next thread ID.
                _threadId++;

                // Create name for thread.
                _delegateThread.Name = string.Format("{0} Queue Thread: {1}", name, _threadId);

                // Start thread.
                _delegateThread.Start();

                Debug.WriteLine(_delegateThread.Name + " Started.");

                // Wait for signal from thread that it is running.
                Monitor.Wait(_lockObject);
            }
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                lock(_lockObject)
                {
                    _disposed = true;

                    Monitor.Pulse(_lockObject);

                    GC.SuppressFinalize(this);
                }
            }
        }

        // Processes and invokes delegates.
        private void DelegateProcedure()
        {
            lock(_lockObject)
            {
                // Signal the constructor that the thread is now running.
                Monitor.Pulse(_lockObject);
            }

            // Set this DelegateQueue as the SynchronizationContext for this thread.
            SetSynchronizationContext(this);

            // Placeholder for DelegateQueueAsyncResult objects.

            // While the DelegateQueue has not been disposed.
            while(true)
            {
                // Critical section.
                DelegateQueueAsyncResult result;
                lock(_lockObject)
                {
                    // If the DelegateQueue has been disposed, break out of loop; we're done.
                    if(_disposed)
                    {
                        break;
                    }

                    // If there are delegates waiting to be invoked.
                    if(_delegateQueue.Count > 0)
                    {
                        result = _delegateQueue.Dequeue();
                    }
                    // Else there are no delegates waiting to be invoked.
                    else
                    {
                        // Wait for next delegate.
                        Monitor.Wait(_lockObject);

                        // If the DelegateQueue has been disposed, break out of loop; we're done.
                        if(_disposed)
                        {
                            break;
                        }

                        Debug.Assert(_delegateQueue.Count > 0);

                        result = _delegateQueue.Dequeue();
                    }
                }

                Debug.Assert(result != null);

                // Invoke the delegate.
                result.Invoke();

                if(result.NotificationType == NotificationType.BeginInvokeCompleted)
                {
                    InvokeCompletedEventArgs e = new InvokeCompletedEventArgs(
                        result.Method,
                        result.GetArgs(),
                        result.ReturnValue,
                        result.Error);

                    OnInvokeCompleted(e);
                }
                else if(result.NotificationType == NotificationType.PostCompleted)
                {
                    object[] args = result.GetArgs();

                    Debug.Assert(args.Length == 1);
                    Debug.Assert(result.Method is SendOrPostCallback);

                    PostCompletedEventArgs e = new PostCompletedEventArgs(
                        (SendOrPostCallback)result.Method,
                        args[0],
                        result.Error);

                    OnPostCompleted(e);
                }
                else
                {
                    Debug.Assert(result.NotificationType == NotificationType.None);
                }
            }

            Debug.WriteLine(_delegateThread.Name + " Finished");
        }
        
        // Raises the InvokeCompleted event.
        protected virtual void OnInvokeCompleted(InvokeCompletedEventArgs e)
        {
            EventHandler<InvokeCompletedEventArgs> handler = InvokeCompleted;

            if(handler != null)
            {
                _context.Post(state => handler(this, e), null);
            }
        }

        // Raises the PostCompleted event.
        protected virtual void OnPostCompleted(PostCompletedEventArgs e)
        {
            EventHandler<PostCompletedEventArgs> handler = PostCompleted;

            if(handler != null)
            {
                _context.Post(state => handler(this, e), null);
            }
        }

        // Raises the Disposed event.
        protected virtual void OnDisposed(EventArgs e)
        {
            EventHandler handler = Disposed;

            if(handler != null)
            {
                _context.Post(state => handler(this, e), null);
            }
        }

        #endregion        

        #endregion

        #region SynchronizationContext Overrides

        /// <summary>
        /// Dispatches a synchronous message to this synchronization context. 
        /// </summary>
        /// <param name="d">
        /// The SendOrPostCallback delegate to call.
        /// </param>
        /// <param name="state">
        /// The object passed to the delegate.
        /// </param>
        /// <remarks>
        /// The Send method starts an synchronous request to send a message. 
        /// </remarks>
        public override void Send(SendOrPostCallback d, object state)
        {
            Invoke(d, state);
        }

        /// <summary>
        /// Dispatches an asynchronous message to this synchronization context. 
        /// </summary>
        /// <param name="d">
        /// The SendOrPostCallback delegate to call.
        /// </param>
        /// <param name="state">
        /// The object passed to the delegate.
        /// </param>
        /// <remarks>
        /// The Post method starts an asynchronous request to post a message. 
        /// </remarks>
        public override void Post(SendOrPostCallback d, object state)
        {
            #region Require

            if(_disposed)
            {
                throw new ObjectDisposedException("DelegateQueue");
            }
            if(d == null)
            {
                throw new ArgumentNullException();
            }

            #endregion

            lock(_lockObject)
            {
                _delegateQueue.Enqueue(new DelegateQueueAsyncResult(this, d, new[] { state }, false, NotificationType.PostCompleted));

                Monitor.Pulse(_lockObject);
            }
        }

        #endregion

        #region IComponent Members

        /// <summary>
        /// Represents the method that handles the Disposed delegate of a DelegateQueue.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets or sets the ISite associated with the DelegateQueue.
        /// </summary>
        public ISite Site { get; set; }

        #endregion

        #region ISynchronizeInvoke Members

        /// <summary>
        /// Executes the delegate on the main thread that this DelegateQueue executes on.
        /// </summary>
        /// <param name="method">
        /// A Delegate to a method that takes parameters of the same number and type that 
        /// are contained in args. 
        /// </param>
        /// <param name="args">
        /// An array of type Object to pass as arguments to the given method. This can be 
        /// a null reference (Nothing in Visual Basic) if no arguments are needed. 
        /// </param>
        /// <returns>
        /// An IAsyncResult interface that represents the asynchronous operation started 
        /// by calling this method.
        /// </returns>
        /// <remarks>
        /// <para>The delegate is called asynchronously, and this method returns immediately. 
        /// You can call this method from any thread. If you need the return value from a process 
        /// started with this method, call EndInvoke to get the value.</para>
        /// <para>If you need to call the delegate synchronously, use the Invoke method instead.</para>
        /// </remarks>
        public IAsyncResult BeginInvoke(Delegate method, params object[] args)
        {
            #region Require

            if(_disposed)
            {
                throw new ObjectDisposedException("DelegateQueue");
            }
            if(method == null)
            {
                throw new ArgumentNullException();
            }

            #endregion

            DelegateQueueAsyncResult result;

            if(InvokeRequired)
            {
                result = new DelegateQueueAsyncResult(this, method, args, false, NotificationType.BeginInvokeCompleted);

                lock(_lockObject)
                {
                    _delegateQueue.Enqueue(result);

                    Monitor.Pulse(_lockObject);
                }
            }
            else
            {
                result = new DelegateQueueAsyncResult(this, method, args, false, NotificationType.None);

                result.Invoke();
            }

            return result;
        }

        /// <summary>
        /// Waits until the process started by calling BeginInvoke completes, and then returns 
        /// the value generated by the process.
        /// </summary>
        /// <param name="result">
        /// An IAsyncResult interface that represents the asynchronous operation started 
        /// by calling BeginInvoke. 
        /// </param>
        /// <returns>
        /// An Object that represents the return value generated by the asynchronous operation.
        /// </returns>
        /// <remarks>
        /// This method gets the return value of the asynchronous operation represented by the 
        /// IAsyncResult passed by this interface. If the asynchronous operation has not completed, this method will wait until the result is available.
        /// </remarks>
        public object EndInvoke(IAsyncResult result)
        {
            #region Require

            if(_disposed)
            {
                throw new ObjectDisposedException("DelegateQueue");
            }
            if(!(result is DelegateQueueAsyncResult))
            {
                throw new ArgumentException();
            }
            if(((DelegateQueueAsyncResult)result).Owner != this)
            {
                throw new ArgumentException();
            }

            #endregion

            result.AsyncWaitHandle.WaitOne();

            DelegateQueueAsyncResult r = (DelegateQueueAsyncResult)result;

            if(r.Error != null)
            {
                throw r.Error;
            }

            return r.ReturnValue;
        }

        /// <summary>
        /// Executes the delegate on the main thread that this DelegateQueue executes on.
        /// </summary>
        /// <param name="method">
        /// A Delegate that contains a method to call, in the context of the thread for the DelegateQueue.
        /// </param>
        /// <param name="args">
        /// An array of type Object that represents the arguments to pass to the given method.
        /// </param>
        /// <returns>
        /// An Object that represents the return value from the delegate being invoked, or a 
        /// null reference (Nothing in Visual Basic) if the delegate has no return value.
        /// </returns>
        /// <remarks>
        /// <para>Unlike BeginInvoke, this method operates synchronously, that is, it waits until 
        /// the process completes before returning. Exceptions raised during the call are propagated 
        /// back to the caller.</para>
        /// <para>Use this method when calling a method from a different thread to marshal the call 
        /// to the proper thread.</para>
        /// </remarks>
        public object Invoke(Delegate method, params object[] args)
        {
            #region Require

            if(_disposed)
            {
                throw new ObjectDisposedException("DelegateQueue");
            }
            if(method == null)
            {
                throw new ArgumentNullException();
            }

            #endregion

            object returnValue;

            if(InvokeRequired)
            {
                DelegateQueueAsyncResult result = new DelegateQueueAsyncResult(this, method, args, false, NotificationType.None);

                lock(_lockObject)
                {
                    _delegateQueue.Enqueue(result);

                    Monitor.Pulse(_lockObject);
                }

                returnValue = EndInvoke(result);
            }
            else
            {
                // Invoke the method here rather than placing it in the queue.
                returnValue = method.DynamicInvoke(args);
            }

            return returnValue;
        }

        /// <summary>
        /// Gets a value indicating whether the caller must call Invoke.
        /// </summary>
        /// <value>
        /// <b>true</b> if the caller must call Invoke; otherwise, <b>false</b>.
        /// </value>
        /// <remarks>
        /// This property determines whether the caller must call Invoke when making 
        /// method calls to this DelegateQueue. If you are calling a method from a different 
        /// thread, you must use the Invoke method to marshal the call to the proper thread.
        /// </remarks>
        public bool InvokeRequired
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId != _delegateThread.ManagedThreadId;
            }
        }        

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes of the DelegateQueue.
        /// </summary>
        public void Dispose()
        {
            #region Guards

            if(_disposed)
            {
                return;
            }

            #endregion

            Dispose(true);

            OnDisposed(EventArgs.Empty);
        }

        #endregion                
    }
}
