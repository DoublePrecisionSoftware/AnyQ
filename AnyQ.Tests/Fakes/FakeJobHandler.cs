using AnyQ.Jobs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnyQ.Tests.Fakes {

    public class FakeJobHandler : JobHandler {
        private readonly bool _canProcess;
        private readonly bool _hangOnProcessing;
        private readonly bool _throwOnProcessing;
        private readonly HandlerConfiguration _configuration;
        private readonly List<RedirectStrategy> _redirects = new List<RedirectStrategy>();

        public FakeJobHandler(HandlerConfiguration handlerConfiguration, bool canProcess, bool hangOnProcessing, bool throwOnProcessing) {
            _canProcess = canProcess;
            _hangOnProcessing = hangOnProcessing;
            _throwOnProcessing = throwOnProcessing;
            _configuration = handlerConfiguration;
        }

        public void AddRedirect(RedirectStrategy redirectStrategy) {
            _redirects.Add(redirectStrategy);
        }

        public override IEnumerable<RedirectStrategy> GetRedirectStrategies() => _redirects;

        public override HandlerConfiguration Configuration => _configuration;

        public override bool CanProcess(ProcessingRequest request) => _canProcess;
        public override Task ProcessAsync(ProcessingRequest request, CancellationToken cancellationToken) {
            if (_hangOnProcessing) {
                while (true) {
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            if (_throwOnProcessing) {
                throw new System.Exception("test");
            }

            base.OnProcessingCompleted(request, "test");
            return Task.FromResult(true);
        }
    }
}
