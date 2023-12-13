using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Threading;
using System.Threading.Tasks;

namespace VUtor.Handlers
{
    public class CustomHandler : CircuitHandler
    {
        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            // Log the disconnect event or show a notification to the user
            Console.WriteLine("Connection down");
            return base.OnConnectionDownAsync(circuit, cancellationToken);
        }

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            // Log the reconnect event or show a notification to the user
            Console.WriteLine("Connection up");
            return base.OnConnectionUpAsync(circuit, cancellationToken);
        }
    }
}
