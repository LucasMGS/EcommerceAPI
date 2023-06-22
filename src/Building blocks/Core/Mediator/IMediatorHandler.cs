using FluentValidation.Results;
using NSE.Core.Messages;

namespace NSE.Core.Mediator
{
    public interface IMediatorHandler
    {
        Task PublishEvents<T>(T evt) where T : Event;
        Task<ValidationResult> SendCommand<T>(T command) where T : Command;
    }
}
