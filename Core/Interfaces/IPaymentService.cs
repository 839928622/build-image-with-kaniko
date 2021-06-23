using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;

namespace Core.Interfaces
{
  public  interface IPaymentService
  {
      Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId);
      Task<Order> UpdateOrderOnPaymentSucceeded(string stripePaymentIntentId);
      Task<Order> UpdateOrderOnPaymentFailed(string stripePaymentIntentId);
    }
}
