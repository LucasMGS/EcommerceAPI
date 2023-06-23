using MediatR;

namespace NSE.Core.Mediator
{
    public abstract class Query : IRequest { }

    public abstract class Query<T> : IRequest<T> { }
}
