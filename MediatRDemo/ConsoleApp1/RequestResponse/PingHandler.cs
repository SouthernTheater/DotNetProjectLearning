using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ConsoleApp1.RequestResponse
{
    public class PingHandler : IRequestHandler<Ping, string>
    {
        public Task<string> Handle(Ping request, CancellationToken cancellationToken)
        {
            return Task.FromResult("Get Request msg,the name is "+request.Name);
        }

    }
}