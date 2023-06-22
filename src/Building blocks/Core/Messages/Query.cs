using MediatR;

namespace NSE.Core.Messages
{
    public abstract class Query : IRequest { }

    public abstract class Query<T> : IRequest<T> { } 
}
