using Core.Entities;

namespace Core.Specifications
{
  public  class ProductWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductWithFiltersForCountSpecification(ProductSpecParam productParameter) : base(x =>
            (!productParameter.BrandId.HasValue || x.ProductBrandId == productParameter.BrandId) &&
            (!productParameter.TypeId.HasValue || x.ProductTypeId == productParameter.TypeId) &&
            (string.IsNullOrEmpty(productParameter.Search) || x.Name.ToLower().Contains(productParameter.Search))
        )
        {
            
        }
    }
}
