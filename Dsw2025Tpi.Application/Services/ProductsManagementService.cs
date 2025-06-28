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

            if (exist != null) throw new DuplicatedEntityException($"Ya existe un producto con el mismo SKU {request.Sku}");

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
        public async Task<ProductModel.Response?> GetProductById(Guid id)
        {
            var product = await _repository.GetById<Product>(id);
            return product != null ?
                new ProductModel.Response(
                product.Id,
                product.Sku,
                product.Name,
                product.Description,
                product.CurrentUnitPrice,
                product.StockQuantity,
                product.IsActive) : null;

        }

        public async Task<IEnumerable<ProductModel.Response>?> GetProducts()
        {

            return (await _repository
                .GetFiltered<Product>(p => p.IsActive))?
                .Select(p => new ProductModel.Response(
                p.Id,
                p.Sku,
                p.Name,
                p.Description,
                p.CurrentUnitPrice,
                p.StockQuantity,
                p.IsActive));
        }
    

    public async Task<ProductModel.Response> UpdateProduct(Guid id, ProductModel.Request request)
        {
            if (string.IsNullOrWhiteSpace(request.Sku) ||
                string.IsNullOrWhiteSpace(request.Name) ||
                request.CurrectUnitPrice <= 0 ||
                request.StockQuantity < 0)
            {
                throw new ArgumentException("Datos de producto inválidos.");
            }

            var existing = await _repository.GetById<Product>(id);
            if (existing == null)
                throw new KeyNotFoundException($"No se encontró producto con Id={id}.");

            
            var mismoSku = await _repository.First<Product>(
                p => p.Sku == request.Sku && p.Id != id 
            );
            if (mismoSku != null)
                throw new ApplicationException($"Ya existe otro producto con SKU='{request.Sku}'.");

            
            existing.Sku = request.Sku;  
            existing.Name = request.Name;
            existing.Description = request.Descripcion;
            existing.CurrentUnitPrice = request.CurrectUnitPrice;
            existing.StockQuantity = request.StockQuantity;


            var updated = await _repository.Update(existing); 

        
            return new ProductModel.Response(
                updated.Id,
                updated.Sku, 
                updated.Name,
                updated.Description,
                updated.CurrentUnitPrice,
                updated.StockQuantity,
                updated.IsActive
            );
        }

        public async Task DisableProduct(Guid id)
        {
            
            var existing = await _repository.GetById<Product>(id);
            if (existing == null)
                throw new KeyNotFoundException($"No se encontró producto con Id={id}.");

                        existing.IsActive = false;

            
            await _repository.Update(existing);
        }

    }
}
