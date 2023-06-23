using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSE.Catalog.API.Models;
using NSE.Catalog.Repositories;
using NSE.WebAPI.Core.Controllers;
using NSE.WebAPI.Core.Identidade;

namespace NSE.Catalog.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class CatalogController : MainController
{
    private readonly IProductRepository _productRepository;

    public CatalogController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _productRepository.GetAll();
    }

    [ClaimsAuthorize("Catalogo","Ler")]
    [HttpGet("{id:guid}")]
    public async Task<Product?> GetProduct(Guid id)
    {
        return await _productRepository.GetById(id);
    }

}
