using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
  public  interface IProductRepos
  {
      /// <summary>
      /// get specify product by an Id
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      Task<Product> GetProductByIdAsync(int id);
        /// <summary>
        /// get a bunch of products
        /// </summary>
        /// <returns></returns>
      Task<IReadOnlyList<Product>> GetProductsAsync();
      Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync();
      Task<IReadOnlyList<ProductType>> GetProductTypesAsync();

    }
}
