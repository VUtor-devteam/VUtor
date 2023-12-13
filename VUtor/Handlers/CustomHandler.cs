using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace VUtor.Handlers
{
    public class CustomHandler : CircuitHandler
    {
        private readonly ILogger<CustomHandler> _logger;

        public CustomHandler(ILogger<CustomHandler> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            // Log the disconnect event
            _logger.LogInformation("Connection down");
            return base.OnConnectionDownAsync(circuit, cancellationToken);
        }

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            // Log the reconnect event
            _logger.LogInformation("Connection up");
            return base.OnConnectionUpAsync(circuit, cancellationToken);
        }
    }
}
