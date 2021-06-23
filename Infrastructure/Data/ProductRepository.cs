using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
  public  class ProductRepository : IProductRepos
    {
        private readonly StoreContext _storeContext;

        public ProductRepository(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        // single vs firstOrDefault,single will throw an exception if list contains more than one record,but firstOrDefault won't
        public async Task<Product> GetProductByIdAsync(int id) => await _storeContext.Products
            .Include(p => p.ProductBrand)
            .Include(p => p.ProductType)
            .SingleOrDefaultAsync(p => p.Id == id);


        public async Task<IReadOnlyList<Product>> GetProductsAsync() => await _storeContext.Products
            .Include(p=> p.ProductBrand)
            .Include(p => p.ProductType)
            .ToListAsync();

        /// <inheritdoc />
        public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync()
        {
            return await _storeContext.ProductBrands.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync()
        {
            return await _storeContext.ProductTypes.ToListAsync();
        }
    }
}
