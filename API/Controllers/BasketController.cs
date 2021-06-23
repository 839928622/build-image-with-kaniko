using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _autoMapper;

        public BasketController(IBasketRepository basketRepository, IMapper autoMapper)
        {
            _basketRepository = basketRepository;
            _autoMapper = autoMapper;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);
            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var customerBasket = _autoMapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var update = await
                _basketRepository.UpdateOrCreateBasketAsync(customerBasket);
            return Ok(update);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteBasketAsync(string basketId)
        {
            await _basketRepository.DeleteBasketAsync(basketId);
            return NoContent();
        }
    }
}
