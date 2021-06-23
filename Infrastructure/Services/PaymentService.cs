using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.Extensions.Configuration;
using Stripe;
using Order = Core.Entities.OrderAggregate.Order;
using Product = Core.Entities.Product;

namespace Infrastructure.Services
{
   public class PaymentService : IPaymentService
    {
        
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration; 
        public PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        /// <inheritdoc />
        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null) return null;
          
            var shippingPrice = 0m;
            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>()
                    .GetByIdAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod.Price;
            }

            foreach (var product in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(product.Id);
                if (product.Price != productItem.Price)
                {
                    product.Price = productItem.Price;
                }
            }
            var service = new PaymentIntentService();
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long) basket.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)shippingPrice,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>(){"card"}
                };

                var intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;

            }
            else
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)shippingPrice,
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            await _basketRepository.UpdateOrCreateBasketAsync(basket);
            return basket;
        }

        /// <inheritdoc />
        public async Task<Order> UpdateOrderOnPaymentSucceeded(string stripePaymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdWithItemsSpecification(stripePaymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
            if (order == null) return null;

            order.OrderStatus = OrderStatus.PaymentReceived;
            _unitOfWork.Repository<Order>().Update(order);
          await  _unitOfWork.Complete();
          return order;
        }

        /// <inheritdoc />
        public async Task<Order> UpdateOrderOnPaymentFailed(string stripePaymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdWithItemsSpecification(stripePaymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
            if (order == null) return null;

            order.OrderStatus = OrderStatus.PaymentFailed;
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.Complete();
            return order;
        }
    }
}
