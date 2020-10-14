using MediatR;

namespace ConsoleApp1.RequestResponse
{
    public class Ping : IRequest<string>
    {
        public string Name { get; set; }
    }
}