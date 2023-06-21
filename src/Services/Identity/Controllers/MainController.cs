using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NSE.Identity.API.Controllers;

[Route("api")]
[ApiController]
public abstract class MainController : Controller
{

  protected ICollection<string> Errors = new List<string>();

  protected IActionResult CustomResponse(object result = null)
  {
    if (OperacaoValida())
    {
      return Ok(result);
    }

    return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>{
      { "Mensagens", Errors.ToArray() }
    })
    );
  }

  protected IActionResult CustomResponse(ModelStateDictionary modelState)
  {
    var errors = modelState.Values.SelectMany(errors => errors.Errors);
    foreach (var error in errors)
    {
      AddErrorToStack(error.ErrorMessage);
    }
    return CustomResponse();
  }

  protected bool OperacaoValida()
  {
    return !Errors.Any();
  }

  protected void AddErrorToStack(string erro)
  {
    Errors.Add(erro);
  }

  protected void ClearErrors()
  {
    Errors.Clear();
  }
}