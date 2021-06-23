
using Core.Entities;

namespace Core.Specifications
{
   public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
    {
        public ProductsWithTypesAndBrandsSpecification(ProductSpecParam productParameter):base(x=>
            (!productParameter.BrandId.HasValue || x.ProductBrandId == productParameter.BrandId) &&
            (!productParameter.TypeId.HasValue || x.ProductTypeId == productParameter.TypeId) &&
            (string.IsNullOrEmpty(productParameter.Search) || x.Name.ToLower().Contains(productParameter.Search))
            )
        {
            AddInclude(x => x.ProductBrand);
            AddInclude(x => x.ProductType);
            AddOrderBy(x => x.Name);
            ApplyPaging(productParameter.PageSize *(productParameter.PageIndex -1),productParameter.PageSize );
        }

        public ProductsWithTypesAndBrandsSpecification(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.ProductBrand);
            AddInclude(x => x.ProductType);
        }
    }
}
