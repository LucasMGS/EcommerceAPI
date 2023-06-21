using Microsoft.AspNetCore.Mvc;
using NSE.Catalog.API.Models;
using NSE.Catalog.Repositories;


namespace NSE.Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : Controller
{
    private readonly IProductRepository _productRepository;

    public CatalogController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _productRepository.GetAll();
    }

    [HttpGet("{id:guid}")]
    public async Task<Product?> GetProduct(Guid id)
    {
        return await _productRepository.GetById(id);
    }

}
