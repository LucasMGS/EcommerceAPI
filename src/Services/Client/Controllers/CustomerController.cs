using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Core.Mediator;
using NSE.Customers.API.Application.Commands;
using NSE.WebAPI.Core.Controllers;

namespace NSE.Customers.API.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : MainController
    {
        private readonly IMediatorHandler _mediatorHandler;

        public CustomerController(IMediatorHandler mediatorHandler)
        {
            _mediatorHandler = mediatorHandler;
        }

        [HttpGet("register")]
        public async Task<IActionResult> RegisterCustomer()
        {
            var command = new CreateCustomerCommand(Guid.NewGuid(), "Lucas", "lucasmgs212@gmail.com", "97685427052");
            var result = await _mediatorHandler
                .SendCommand(command);

            return CustomResponse(result);
        }
    }
}
