using API.Dtos;
using AutoMapper;
using Core.Entities;
using Microsoft.Extensions.Configuration;

namespace API.Helpers
{
    public class ProductUrlResolver : IValueResolver<Product,ProductToReturnDto,string>
    {
        private readonly IConfiguration _configuration;

        public ProductUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <inheritdoc />
        public string Resolve(Product source, ProductToReturnDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
            {
                // same as _configuration.GetSection("ApiUrl").Value
                return _configuration["ApiUrl"] + source.PictureUrl; // _configuration["ApiUrl"]
            }

            return null;
        }
    }
}
