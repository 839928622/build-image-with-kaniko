using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;

namespace Infrastructure.Services
{
  public  class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepo;
        private readonly IPaymentService _paymentService;

        public OrderService(IUnitOfWork unitOfWork, IBasketRepository basketRepo,
                            IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _basketRepo = basketRepo;
            _paymentService = paymentService;
        }
        /// <inheritdoc />
        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            // get basket from the repo
            var basket = await _basketRepo.GetBasketAsync(basketId);
            // get items from the product repo
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id,productItem.Name,productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered,productItem.Price,item.Quantity);
                items.Add(orderItem);
            }
            // ret delivery method from repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            // calculate subtotal
            var subTotal = items.Sum(item => item.Price);
            // check to see if order exists (same paymentIntentId)
            var spec = new OrderByPaymentIntentIdWithItemsSpecification(basket.PaymentIntentId);
          var existingOrder= await  _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
          if (existingOrder != null)
          {
              _unitOfWork.Repository<Order>().Delete(existingOrder);
              await _paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntentId);
          }
            // create order
            var order = new Order(items , buyerEmail, shippingAddress, deliveryMethod, subTotal,basket.PaymentIntentId);
            // save t db
             _unitOfWork.Repository<Order>().Add(order); // nothing saved to database at this point
              await _unitOfWork.Complete(); // if error , it will throw an error 
          
            // return order
            return order;
        }

        
        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);

            return await _unitOfWork.Repository<Order>().ListAsync(spec);
        }

        /// <inheritdoc />
        public async Task<Order> GetOrderByOrderIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }
    }
}
