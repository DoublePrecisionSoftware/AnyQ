using AnyQ.Jobs;
using AnyQ.Queues;
using AnyQ.Queues.Status;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnyQ {
    /// <summary>
    /// Represents a process for handling incoming <see cref="JobRequest"/> objects and directing them to the appropriate <see cref="JobHandler"/>
    /// </summary>
    public sealed class JobQueueListener : IDisposable {

        private readonly InternalJobQueueFactory _jobQueueFactory;
        private readonly ListenerConfiguration _config;
        private readonly List<IStatusProvider> _statusProviders = new List<IStatusProvider>();
        private readonly InternalJobHandlerLocator _handlerLocator;
        private readonly Dictionary<string, JobQueue> _queues = new Dictionary<string, JobQueue>();
        private readonly Dictionary<string, RedirectStrategy> _queueRedirects = new Dictionary<string, RedirectStrategy>();
        private bool _disposedValue = false;
        private bool _cancelProcessing = false;
        
        /// <summary>
        /// Create a new instance of <see cref="JobQueueListener"/> with all required dependencies
        /// </summary>
        /// <param name="jobQueueFactory">Factory for creating appropriate <see cref="JobQueue"/> objects</param>
        /// <param name="config">Configuration value for this listener</param>
        public JobQueueListener(IJobQueueFactory jobQueueFactory, ListenerConfiguration config = null) {
            if (jobQueueFactory == null) {
                throw new ArgumentNullException(nameof(jobQueueFactory));
            }

            _config = config ?? new ListenerConfiguration();
            _jobQueueFactory = new InternalJobQueueFactory(_queues, jobQueueFactory, _config);
            _handlerLocator = new InternalJobHandlerLocator(_config);
        }

        /// <summary>
        /// Indicates that the listener is listening for jobs
        /// </summary>
        public bool Listening { get; private set; }

        /// <summary>
        /// Fired when a <see cref="ProcessingRequest"/> is redirected to another queue
        /// </summary>
        public event EventHandler<RequestRedirectedEventArgs> RequestRedirected;
        /// <summary>
        /// Fired when a <see cref="ProcessingRequest"/> is processed successfully
        /// </summary>
        public event EventHandler<ProcessingCompletedEventArgs> ProcessingCompleted;
        /// <summary>
        /// Fired when a <see cref="JobHandler.ProcessAsync(ProcessingRequest, CancellationToken)"/> call times out<para />
        /// (see <see cref="ListenerConfiguration.JobTimeout"/>)
        /// </summary>
        public event EventHandler<ProcessingFailedEventArgs> ProcessingTimedOut;
        /// <summary>
        /// Fired when the processing of a <see cref="ProcessingRequest"/> fails
        /// </summary>
        public event EventHandler<ProcessingFailedEventArgs> ProcessingFailed;
        /// <summary>
        /// Fired when a <see cref="JobStatus"/> report is generated
        /// </summary>
        public event EventHandler<StatusReportedEventArgs> StatusReported;
        
        private void OnStatusReported(JobStatus status) {
            StatusReported?.Invoke(this, new StatusReportedEventArgs(status));
        }

        /// <summary>
        /// Initiate queue listening on all <see cref="JobQueue"/>s
        /// </summary>
        public void Listen() {
            if (Listening) {
                return;
            }
            foreach (var queue in _queues.Values) {
                queue.StartListening();
            }
            Listening = true;
        }

        /// <summary>
        /// Halt queue listening on all <see cref="JobQueue"/>s
        /// </summary>
        public void Stop(bool cancelProcessing = false) {
            _cancelProcessing = cancelProcessing;
            foreach (var queue in _queues.Values) {
                queue.Close();
            }
            Listening = false;
        }

        /// <summary>
        /// Add or replace a <see cref="RedirectStrategy"/> on a specific <see cref="IMessageQueue"/>
        /// </summary>
        /// <param name="queueId">Identifier for <see cref="IMessageQueue"/> to redirect for</param>
        /// <param name="strategy">Strategy for redirection</param>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <remarks>
        /// TODO: remove this and go with optional global redirects.
        /// Only <see cref="JobHandler"/>s should provide queue-specific redirects.
        /// </remarks>
        public void AddRedirectStrategy(string queueId, RedirectStrategy strategy) {
            if (string.IsNullOrWhiteSpace(queueId)) {
                throw new ArgumentException(nameof(queueId));
            }
            
            if (strategy == null) {
                throw new ArgumentNullException(nameof(strategy));
            }

            if (_queueRedirects.ContainsKey(queueId)) {
                _queueRedirects.Remove(queueId);
                _queueRedirects.Add(queueId, strategy);
            } else {
                _queueRedirects.Add(queueId, strategy);
            }
        }

        /// <summary>
        /// Add a new <see cref="IStatusProvider"/> to writing job status
        /// </summary>
        /// <param name="provider">Instance of an <see cref="IStatusProvider"/></param>
        /// <exception cref="ArgumentNullException" />
        public void AddStatusProvider(IStatusProvider provider) {
            if (provider == null) {
                throw new ArgumentNullException(nameof(provider));
            }
            _statusProviders.Add(provider);
        }
        
        private void AddRedirectStrategy(JobHandler handler, RedirectStrategy strategy) {
            AddRedirectStrategy(
                handler?.Configuration.QueueId ?? throw new ArgumentNullException(nameof(handler)), 
                strategy ?? throw new ArgumentNullException(nameof(strategy)));
        }

        /// <summary>
        /// Add an <see cref="IJobHandlerLocator"/> for loading handlers
        /// </summary>
        /// <param name="locator">Locator to add</param>
        public void AddHandlerLocator(IJobHandlerLocator locator) {
            if (locator == null) {
                throw new ArgumentNullException(nameof(locator));
            }

            _handlerLocator.AddLocator(locator);

            foreach (var handler in locator.GetHandlers()) {
                AddHandler(handler, false);
            }
        }

        /// <summary>
        /// Add a new <see cref="JobHandler"/> for processing of new jobs
        /// </summary>
        /// <param name="handler">Handler for new requests</param>
        /// <exception cref="ArgumentNullException" />
        public void AddHandler(JobHandler handler) {
            AddHandler(handler, true);
        }

        private void AddHandler(JobHandler handler, bool loose) {
            if (handler == null) {
                throw new ArgumentNullException(nameof(handler));
            }

            if (_queues.TryGetValue(handler.Configuration.QueueId, out var queue)) {
                Trace.TraceWarning($"A handler for {queue} already loaded.  Remove handler before loading new one.");
                return;
            }

            Trace.TraceInformation($"Loading handler for queue {handler.Configuration.QueueName} ({handler.Configuration.QueueId})...");

            RegisterQueueForHandler(handler);
            if (loose) {
                _handlerLocator.AddHandler(handler);
            }
        }

        private void RegisterQueueForHandler(JobHandler handler) {

            handler.ProcessingCompleted += OnProcessingCompleted;
            handler.ProcessingFailed += OnProcessingFailed;

            foreach (var strategy in handler.GetRedirectStrategies().Where(h => h != null)) {
                AddRedirectStrategy(handler, strategy);
            }

            _jobQueueFactory.Create(handler.Configuration).JobReceived += Listener_JobReceived;
        }

        #region Queue Interaction
        /// <summary>
        /// Get a specific message from the specified queue without removing it from the queue
        /// </summary>
        /// <param name="queueId">Unique identifier for the queue</param>
        /// <param name="messageId">Unique identifier for the message</param>
        public IMessage GetMessage(string queueId, string messageId) {
            return _jobQueueFactory.Create(queueId).GetMessage(messageId);
        }

        /// <summary>
        /// Get a specific message from the specified queue without removing them from the queue
        /// </summary>
        /// <param name="queueId">Unique identifier for the queue</param>
        public IEnumerable<IMessage> GetMessages(string queueId) {
            return _jobQueueFactory.Create(queueId).GetMessages();
        }

        /// <summary>
        /// Get a specific message from the specified queue without removing them from the queue
        /// </summary>
        /// <param name="queueId">Unique identifier for the queue</param>
        /// <param name="messageId">Unique identifier for the message</param>
        public void ReceiveMessage(string queueId, string messageId) {
            _jobQueueFactory.Create(queueId).ReceiveMessage(messageId);
        }

        /// <summary>
        /// Get a specific message from the specified queue without removing them from the queue
        /// </summary>
        /// <param name="queueId">Unique identifier for the queue</param>
        public void ReceiveMessage(string queueId) {
            _jobQueueFactory.Create(queueId).ReceiveMessage();
        }

        /// <summary>
        /// Synchronously execute a job from the specified queue
        /// </summary>
        /// <param name="queueId">Unique identifier for the queue</param>
        /// <param name="messageId">Unique identifier for the message</param>
        public Task ExecuteJobAsync(string queueId, string messageId) {
            var request = _jobQueueFactory.Create(queueId).GetProcessingRequest(messageId);
            return HandleRequestAsync(request, false);
        }

        /// <summary>
        /// Delete all messages from the specified queue
        /// </summary>
        /// <param name="queueId">Unique identifier for the queue</param>
        public void PurgeQueue(string queueId) {
            _jobQueueFactory.Create(queueId).Purge();
        }

        /// <summary>
        /// Delete a specific message from the specified queue
        /// </summary>
        /// <param name="queueId">Unique identifier for the queue</param>
        /// <param name="messageId">Unique identifier for the message</param>
        public void DeleteMessage(string queueId, string messageId) {
            _jobQueueFactory.Create(queueId).DeleteMessage(messageId);
        }
        #endregion

        private void Listener_JobReceived(object sender, JobReceivedEventArgs e) {
            HandleRequestAsync(e.ProcessingRequest, true).Wait(); // TODO: Wait() is just fine, but can I do more?
        }

        private async Task HandleRequestAsync(ProcessingRequest request, bool processEvents) {
            WriteStatus(request, JobStatus.Received, issueRedirect: false);

            if (_cancelProcessing) {
                WriteStatus(request, JobStatus.Canceled);
                return;
            }

            if (!_handlerLocator.TryGetHandlerByQueueId(request.QueueId, out var handler)) {
                // NOTE: this will never be reached due to the throw in InternalJobQueueFactory.Create(),
                // which is called by all entrypoints into this method.
                WriteStatus(request, JobStatus.Skipped, $"No handler found for {request}");
                return;
            }

            if (!handler.CanProcess(request)) {
                WriteStatus(request, JobStatus.Skipped, $"No handler could process request '{request}'");
                return;
            }

            WriteStatus(request, JobStatus.Processing, issueRedirect: false);

            if (!processEvents) {
                await HandleJob(request, handler);
                return;
            }

            try {
                await HandleJob(request, handler);
            } catch (OperationCanceledException ex) {
                //? JobStatus.Cancelled?
                OnProcessingTimedOut(handler, new ProcessingFailedEventArgs(request, ex));
            } catch (Exception ex) {
                OnProcessingFailed(handler, new ProcessingFailedEventArgs(request, ex));
            }
        }

        /// <summary>
        /// Execute <see cref="JobHandler.ProcessAsync(ProcessingRequest, CancellationToken)"/> for the specified <see cref="ProcessingRequest"/>
        /// </summary>
        /// <param name="request">Request to process</param>
        /// <param name="handler">Handler to handle the job</param>
        private Task HandleJob(ProcessingRequest request, JobHandler handler) {

            var cts = new CancellationTokenSource();
            if (_config.JobTimeout > 0) {
                cts.CancelAfter(_config.JobTimeout);
            }

            using (cts) {
                return handler.ProcessAsync(request, cts.Token);
            }
        }

        private void OnProcessingTimedOut(object sender, ProcessingFailedEventArgs e) {
            var status = JobStatus.TimedOut;
            WriteStatus(e.ProcessingRequest, status, exception: e.Exception);
            ProcessingTimedOut?.Invoke(this, e);
        }

        private void OnProcessingFailed(object sender, ProcessingFailedEventArgs e) {
            var status = JobStatus.Failed;
            WriteStatus(e.ProcessingRequest, status, e.Exception.ToString(), exception: e.Exception);
            ProcessingFailed?.Invoke(this, e);
        }

        private void OnProcessingCompleted(object sender, ProcessingCompletedEventArgs e) {
            var status = JobStatus.Complete;
            WriteStatus(e.ProcessingRequest, status, e.ResultBody);
            ProcessingCompleted?.Invoke(this, e);
        }
            
        private void WriteStatus(ProcessingRequest request, string status, string details = null, bool issueRedirect = true, Exception exception = null) {
            var jobStatus = new JobStatus {
                JobId = request.JobId,
                JobName = request.Name,
                Status = status,
                QueueId = request.QueueId,
                QueueName = request.QueueName,
                Details = details,
            };

            foreach (var provider in _statusProviders) {
                try {
                    provider.WriteStatus(jobStatus);
                } catch (Exception ex) {
                    Trace.TraceError($"Error recording status: {ex}");
                }
            }

            OnStatusReported(jobStatus);

            if (issueRedirect) {
                Redirect(jobStatus, request, exception);
            }
        }

        private void Redirect(JobStatus status, ProcessingRequest request, Exception exception) {
            if (_queueRedirects.TryGetValue(request.QueueId, out var strategy)) {
                var result = ExecuteRedirect(strategy, status, request, exception);
                if (result != null) {
                    RequestRedirected?.Invoke(this, new RequestRedirectedEventArgs(result.Message, request.QueueId, result.NewQueue));
                }
            }
        }

        /// <remarks>Returning null from this method causes the calling code to ignore the redirect</remarks>
        private RedirectResult ExecuteRedirect(RedirectStrategy strategy, JobStatus status, ProcessingRequest request, Exception exception) {
            if (!strategy.Decider(status, exception)) {
                return null;
            }
            var newQueue = strategy.Redirect(status);
            if (string.IsNullOrWhiteSpace(newQueue)) {
                return null;
            }

            var message = SendJob(newQueue, request.JobRequest.Type, request.JobRequest.Payload, request.Name);
            WriteStatus(request, JobStatus.Redirected, $"New Queue: {newQueue} Message Id: {message.Id}", false);
            return new RedirectResult {
                Message = message,
                NewQueue = newQueue
            };
        }


        /// <summary>
        /// Send a new job to the specified queue
        /// </summary>
        /// <param name="queueId">Id of the queue</param>
        /// <param name="type">What to set the <see cref="JobRequest.Type"/> value to</param>
        /// <param name="payload">Message payload</param>
        /// <param name="label">Human-readable title for the job</param>
        public IMessage SendJob(string queueId, string type, object payload, string label) {
            return _jobQueueFactory.Create(queueId).SendJob(type, payload, label);
        }

        /// <summary>
        /// Execute a specific job on a queue
        /// </summary>
        /// <param name="queueId">Id of the queue</param>
        /// <param name="messageId">Id of the message defining the job</param>
        public void ExecuteJob(string queueId, string messageId) {
            _jobQueueFactory.Create(queueId).ReceiveMessage(messageId);
        }

        /// <summary>
        /// Execute the most recent job on the queue
        /// </summary>
        /// <param name="queueId">Id of the queue</param>
        public void ExecuteJob(string queueId) {
            _jobQueueFactory.Create(queueId).ReceiveMessage();
        }

        #region IDisposable Support

        /// <summary>
        /// Releases all resources used by the <see cref="JobQueueListener"/>
        /// </summary>
        /// <param name="disposing">Whether or not the object is already being disposed</param>
        private void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    foreach (var queue in _queues.Values) {
                        queue.Dispose();
                    }
                    // TODO: client should be reponsible for disposing of handlers
                }
                _disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="JobQueueListener"/>
        /// </summary>
        public void Dispose() => Dispose(true);
        #endregion

        private class InternalJobQueueFactory : IJobQueueFactory {
            private readonly Dictionary<string, JobQueue> _queues;
            private readonly IJobQueueFactory _jobQueueFactory;
            private readonly ListenerConfiguration _listenerConfiguration;

            public InternalJobQueueFactory(Dictionary<string, JobQueue> queues, IJobQueueFactory depFactory, ListenerConfiguration listenerConfiguration) {
                _queues = queues;
                _jobQueueFactory = depFactory;
                _listenerConfiguration = listenerConfiguration;
            }

            public JobQueue Create(string queueId) {
                if (_queues.TryGetValue(queueId, out var queue)) {
                    return queue;
                }
                throw new QueueNotAvailableException($"No queue with Id {queueId} exists.");
            }

            public JobQueue Create(HandlerConfiguration configuration) {
                try {
                    var newQueue = _jobQueueFactory.Create(configuration);
                    _queues.Add(newQueue.QueueId, newQueue);
                    return newQueue;
                } catch (Exception ex) {
                    Trace.TraceError($"Error creating queue '{configuration.QueueId}'.{Environment.NewLine}{ex}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Consolidates <see cref="IJobHandlerLocator"/>s and "loose" handlers into one place
        /// </summary>
        private class InternalJobHandlerLocator : IJobHandlerLocator {
            private readonly List<IJobHandlerLocator> _locators = new List<IJobHandlerLocator>();
            private readonly List<JobHandler> _handlers = new List<JobHandler>();
            private readonly ListenerConfiguration _listenerConfiguration;

            public InternalJobHandlerLocator(ListenerConfiguration listenerConfiguration) {
                _listenerConfiguration = listenerConfiguration;
            }

            public void AddLocator(IJobHandlerLocator locator) {
                _locators.Add(locator);
            }

            public void AddHandler(JobHandler handler) {
                _handlers.Add(handler);
            }

            public IEnumerable<JobHandler> GetHandlers() {
                return _locators.SelectMany(l => l.GetHandlers()).Union(_handlers);
            }

            public bool TryGetHandlerByQueueId(string queueId, out JobHandler outHandler) {

                foreach (var locator in _locators) {
                    if (locator.TryGetHandlerByQueueId(queueId, out outHandler)) {
                        return true;
                    }
                }

                foreach (var handler in _handlers) {
                    if (handler.Configuration.QueueId == queueId) {
                        outHandler = handler;
                        return true;
                    }
                }

                Trace.TraceWarning($"Handler for queue '{queueId}' not found.");
                outHandler = null;
                return false;
            }
        }

        private class RedirectResult {
            public IMessage Message { get; set; }
            public string NewQueue { get; set; }
        }
    }
}
