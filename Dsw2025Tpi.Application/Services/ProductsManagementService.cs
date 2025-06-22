using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Application.Exceptions;
    


namespace Dsw2025Tpi.Application.Services
{
    public class ProductsManagementService
    {
        private readonly IRepository _repository;
        public ProductsManagementService(IRepository repository)
        {
            _repository = repository;
        }


        public async Task<ProductModel.Response> AddProduct(ProductModel.Request request)
        {  
             if (string.IsNullOrWhiteSpace(request.Sku) ||
                string.IsNullOrWhiteSpace(request.Name) ||
                request.CurrectUnitPrice < 0 || request.StockQuantity < 0)
            {
             throw new ArgumentException("Los valores del producto son invalidos.");
            }

            var exist = await _repository.First<Product>(p => p.Sku == request.Sku);

            // if (exist != null) throw new DuplicatedEntityException($"Ya existe un producto con el mismo SKU {request.Sku}");

            var product = new Product(
                request.Sku,
                request.Name,
                request.CurrectUnitPrice,
                request.StockQuantity,
                request.Descripcion);

            await _repository.Add(product);
            return new ProductModel.Response(
                product.Id,
                product.Sku, 
                product.Name,
                product.Description,
                product.CurrentUnitPrice,
                product.StockQuantity,
                product.IsActive);
        }
    }

        
}
